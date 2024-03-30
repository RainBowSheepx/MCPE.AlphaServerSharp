using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SpoongePE.Core.Utils;

namespace SpoongePE.Core.RakNet;

public class RakNetServer
{
    private readonly Dictionary<IPEndPoint, RakNetClient> Connections;

    public RakNetServer(int port)
    {
        GUID = (ulong)Random.Shared.Next() & ((ulong)Random.Shared.Next() << 32);
        IP = new IPEndPoint(IPAddress.Any, port);
        UDP = new UdpClient(IP);
        Connections = new Dictionary<IPEndPoint, RakNetClient>();
        TaskCancellationToken = new CancellationTokenSource();
    }

    public ulong GUID { get; }
    public IPEndPoint IP { get; }
    internal UdpClient UDP { get; }
    public static ServerProperties Properties { get; set; }

    private IConnectionHandler ConnectionHandler { get; set; }
    private static CancellationTokenSource TaskCancellationToken { get; set; }
    private DateTime StartedOn { get; } = DateTime.Now;

    public ulong TimeSinceStart => (ulong)(DateTime.Now - StartedOn).TotalMilliseconds;

    public void Start(IConnectionHandler connectionHandler)
    {
        ConnectionHandler = connectionHandler;

        StartRepeatingTask(HandlePackets, TimeSpan.Zero);
        StartRepeatingTask(HandleConnections, TimeSpan.FromMilliseconds(1));
    }

    public void Stop()
    {
        TaskCancellationToken.Cancel();
        UDP.Close();
    }

    private async Task HandlePackets()
    {
        try
        {
            var receiveResult = await UDP.ReceiveAsync();

            // Try handling the connected packet, might fall through if the client reconnects?
            if (Connections.TryGetValue(receiveResult.RemoteEndPoint, out var existingConnection))
            {
                //   Logger.Debug($"Letting {existingConnection} handle packet");
                existingConnection.HandlePacket(receiveResult.Buffer);
                return;
            }

            // If we don't have a session for this IP yet.
            switch (UnconnectedPacket.Parse(receiveResult.Buffer))
            {
                case UnconnectedPingPacket:
                    await Send(receiveResult.RemoteEndPoint,
                        new UnconnectedPongPacket(TimeSinceStart, GUID, $"MCCPP;Demo;{Properties.serverName} [{Connections.Count}/{Properties.maxPlayers}]")
                    );
                    break;
                case OpenConnectionRequest1Packet openConnectionRequest1Packet:
                    Logger.Debug($"Received OpenConnectionRequest1Packet from {receiveResult.RemoteEndPoint} MTU: {openConnectionRequest1Packet.mtuSize}");
                    await Send(receiveResult.RemoteEndPoint,
                        new OpenConnectionReply1Packet(GUID, false, (ushort)Math.Min(openConnectionRequest1Packet.mtuSize, 1480)) // TODO: MTU Is hardcoded.
                    );
                    break;
                case OpenConnectionRequest2Packet request:
                    Logger.Debug($"Handling connection request from {receiveResult.RemoteEndPoint} MTU: {request.MtuSize}");
                    var newConnetion = new RakNetClient(receiveResult.RemoteEndPoint, this)
                    {
                        ClientID = request.ClientID,
                        mtuSize = request.MtuSize
                    };

                    Connections.Add(receiveResult.RemoteEndPoint, newConnetion);

                    await Send(receiveResult.RemoteEndPoint,
                        new OpenConnectionReply2Packet(GUID, newConnetion.IP, request.MtuSize, false)
                    );

                    break;
                case { } packet:
                    Logger.Warn($"Got unhandled unconnected packet {packet}? This is probably a bug.");
                    break;
            }
        }catch(Exception e)
        {
            Logger.Error(e.ToString());
        }

    }

    private async Task HandleConnections()
    {
        try
        {
            foreach (var (_, connection) in Connections)
                await connection.HandleResendPacketInstantly();
            foreach (var (_, connection) in Connections)
                await connection.HandleSplitPackets();
            foreach (var (_, connection) in Connections)
                await connection.HandleOutgoing();
            foreach (var (_, connection) in Connections)
                await connection.SendACKs();




            foreach (var (endpoint, client) in Connections.Where(x => !x.Value.IsConnected))
            {
                ConnectionHandler?.OnClose(client, client.IsTimedOut ? "Timed out" : "Disconnected");
                Connections.Remove(endpoint);
            }

            ConnectionHandler?.OnUpdate();
        } catch(Exception e)
        {
            Logger.Error(e.ToString());

        }


        //await Task.Delay(1); // Uncomment if u have troubles


    }

    // Public async task creator xd
    public static void StartRepeatingTask(Func<Task> action, TimeSpan interval)
    {
        Task.Run(async () =>
        {
            while (!TaskCancellationToken.IsCancellationRequested)
            {
                await action();
                await Task.Delay(interval);
            }
        }, TaskCancellationToken.Token
        );
    }

    private async Task Send(IPEndPoint endPoint, UnconnectedPacket packet)
    {
        var writer = new DataWriter();
        packet.Encode(ref writer);
        var buffer = writer.GetBytes();
        await UDP.SendAsync(buffer, buffer.Length, endPoint);
    }

    internal void OnOpen(RakNetClient connection) => ConnectionHandler?.OnOpen(connection);
    internal void OnClose(RakNetClient connection, string reason) => ConnectionHandler?.OnClose(connection, reason);

    internal void OnData(RakNetClient connection, ReadOnlyMemory<byte> data) =>
            ConnectionHandler?.OnData(connection, data);
}
