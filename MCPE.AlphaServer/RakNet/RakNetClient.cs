using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MCPE.AlphaServer.Network;
using MCPE.AlphaServer.Utils;

namespace MCPE.AlphaServer.RakNet;

public class RakNetClient {
    public enum ConnectionStatus {
        CONNECTING,
        CONNECTED,
        DISCONNECTING,
        DISCONNECTED
    }

    public RakNetClient(IPEndPoint endPoint, RakNetServer server) {
        IP = endPoint;
        LastPing = DateTime.Now;
        Status = ConnectionStatus.CONNECTING;
        OutgoingPackets = new List<ConnectedPacket>();
        NeedsACK = new SortedSet<int>();
        CurrentSequenceNumber = 0;
        LastReliablePacketIndex = 0;
        Server = server;
    }

    public IPEndPoint IP { get; init; }
    public DateTime LastPing { get; set; }
    public ulong ClientID { get; set; }
    public ConnectionStatus Status { get; set; }

    private List<ConnectedPacket> OutgoingPackets { get; }
    private SortedSet<int> NeedsACK { get; }
    private int CurrentSequenceNumber;
    private int LastReliablePacketIndex;

    internal RakNetServer Server;

    public bool IsTimedOut => DateTime.Now - LastPing > TimeSpan.FromSeconds(5);
    public bool IsConnected => !IsTimedOut && Status != ConnectionStatus.DISCONNECTED;

    internal void HandlePacket(byte[] data) {
        LastPing = DateTime.Now;

    //    Logger.Debug(
     //           $"{IP} PreProcess: IsACK={data[0] & UnconnectedPacket.IS_ACK}, IsNAK={data[0] & UnconnectedPacket.IS_NAK}, IsConnected={data[0] & UnconnectedPacket.IS_CONNECTED}");
     //    Logger.Debug(Formatters.AsHex(data));

        var reader = new DataReader(data);
        if ((data[0] & UnconnectedPacket.IS_ACK) != 0)
            HandleACK(ref reader);
        else if ((data[0] & UnconnectedPacket.IS_NAK) != 0)
            HandleNAK(ref reader);
        else if ((data[0] & UnconnectedPacket.IS_CONNECTED) != 0)
            HandleConnected(ref reader);
    }

    private void HandleACK(ref DataReader reader) {
        var packet = ConnectedPacket.ParseMeta(ref reader);
        //Logger.Warn($"TODO: HandleACK {packet}");
    }

    private void HandleNAK(ref DataReader reader) {
        var packet = ConnectedPacket.ParseMeta(ref reader);
        Logger.Warn($"TODO: HandleNAK {packet}");
    }

    private void HandleConnected(ref DataReader reader) {
        reader.Byte();
        var sequenceNumber = reader.Triad();
        NeedsACK.Add(sequenceNumber);

        do {
            switch (ConnectedPacket.Parse(ref reader)) {
                case ConnectedPingPacket ping:
                    Send(new ConnectedPongPacket {
                            TimeSinceStart = ping.TimeSinceStart,
                            TimeSinceServerStart = 0,
                        }, ConnectedPacket.RELIABLE
                    );
                    break;
                case ConnectionRequestPacket:
                    Send(new ConnectionRequestAcceptedPacket {
                            EndPoint = IP,
                            TimeSinceStart = 0 // TODO: Fix.
                        }, ConnectedPacket.RELIABLE
                    );
                    break;
                case NewIncomingConnectionPacket:
                    Status = ConnectionStatus.CONNECTED;
                    Server.OnOpen(this);
                    break;
                case UserPacket user:
                    Server.OnData(this, user.Data);
                    break;
                case PlayerDisconnectPacket:
                    Status = ConnectionStatus.DISCONNECTED;
                    break;
                case { } packet:
                    Logger.Warn($"Unhandled {packet}?");
                    break;
            }
        } while (!reader.IsEof);
    }
    public int bigCnt = -1;

    internal async Task HandleOutgoing() {
        if (OutgoingPackets.Count < 1) {
            if (NeedsACK.Count < 1)
                return;

            // Send ACKs.
            var ackWriter = new DataWriter();
            ackWriter.Byte(UnconnectedPacket.IS_CONNECTED | UnconnectedPacket.IS_ACK);

            // TODO: Use the range feature from RakNet?
            ackWriter.Short((short)NeedsACK.Count);
            foreach (var sequence in NeedsACK) {
                ackWriter.Byte(1); // Min == max.
                ackWriter.Triad(sequence);
            }

            await Server.UDP.SendAsync(ackWriter.GetBytes(), IP);

            NeedsACK.Clear();
            return;
        }

        var writer = new DataWriter();
        writer.Byte(UnconnectedPacket.IS_CONNECTED); 
        writer.Triad(CurrentSequenceNumber++);

        UnknowPacket newpacket = null;
 
        foreach (var packet in OutgoingPackets) {
            var packetWriter = new DataWriter();
            packet.Encode(ref packetWriter);
            // Check split packet
            // 1480 is my MTU. Hardcoded
            int offsetMTU = 414;
            if (packetWriter.GetBytes().Length > 1480- offsetMTU)
            {


                writer = new DataWriter();
                writer.Byte(UnconnectedPacket.IS_CONNECTED);
                writer.Triad(CurrentSequenceNumber-1);
                List<byte[]> fragmented_body = new List<byte[]>();
                byte[] test = packetWriter.GetBytes();
                try
                {
                    for (int i = 0; i < test.Length; i += 1480 - offsetMTU)
                    {
                        if (i + (1480 - offsetMTU) > test.Length)
                        {
                            int t = (i + (1480 - offsetMTU)) - ((i + (1480 - offsetMTU)) - test.Length);
                            
                            fragmented_body.Add(test[i..t]);
                        }
                        else
                        {
                            fragmented_body.Add(test[i..(i + 1480 - offsetMTU)]);
                        }
                            

                        
                    }
                }
                catch(Exception e)
                {
                    Logger.Info(e.Message);
                }


                bigCnt = (bigCnt + 1) % 0x10000;
                for (int i = 0; i < fragmented_body.Count; i++)
                {
                    newpacket = new UnknowPacket();
                    newpacket.ReliableIndex = LastReliablePacketIndex++;
                    newpacket.packetID = (byte)MinecraftPacketType.ChunkData;
                    newpacket.hasSplit = true;
                    newpacket.Reliability = 2;
                    newpacket.splitID = (short)bigCnt;
                    newpacket.splitCount = fragmented_body.Count;
                    newpacket.splitIndex = i;


                    writer.Byte((byte)((newpacket.Reliability << 5) | (newpacket.hasSplit ? 0x10 : 0)));
                    writer.Short((short)(packetWriter.Length * 8));
                    switch (newpacket.Reliability)
                    {
                        case ConnectedPacket.RELIABLE:
                            writer.Triad(newpacket.ReliableIndex); // TODO Add priority for reliable index
                            break;
                    }
                    writer.Int(newpacket.splitCount);
                    writer.Short(newpacket.splitID);
                    writer.Int(newpacket.splitIndex);
                    writer.RawData(fragmented_body[i]);
                    var strss = string.Join(" ", writer.GetBytes());
                    Logger.Info(strss + " Size: " + writer.GetBytes().Length);
                    await Server.UDP.SendAsync(writer.GetBytes(), IP);
                    writer = new DataWriter();
                    writer.Byte(UnconnectedPacket.IS_CONNECTED);
                    writer.Triad(CurrentSequenceNumber++);
                }
             
            } 
            else
            {
                writer.Byte((byte)(packet.Reliability << 5));
                writer.Short((short)(packetWriter.Length << 3));

                switch (packet.Reliability)
                {
                    case ConnectedPacket.RELIABLE:
                        writer.Triad(packet.ReliableIndex);
                        break;
                    case ConnectedPacket.RELIABLE_ORDERED:
                        writer.Triad(packet.ReliableIndex);
                        writer.Triad(packet.OrderingIndex);
                        writer.Byte((byte)packet.OrderingChannel);
                        break;
                }

                writer.RawData(packetWriter.GetBytes());
            }
            

        }
        var strs = string.Join(" ", writer.GetBytes());
        Logger.Info(strs + " Size: " + writer.GetBytes().Length);
        var str = string.Join(" ", writer.GetBytes());
        if (writer.GetBytes().Length > 1480)
        {
            Logger.Info("Packet size is " + writer.GetBytes().Length);
            //     Logger.Info("Bigger packet!\n" + str);
            OutgoingPackets.Clear();
            return;
        }
        if(writer.GetBytes().Length == 4)
        {
            Logger.Info("FIXME");
            OutgoingPackets.Clear();
        }
        await Server.UDP.SendAsync(writer.GetBytes(), IP);
        OutgoingPackets.Clear();
    }



    public void Send(ConnectedPacket packet, int reliability = ConnectedPacket.RELIABLE) {
        if (reliability == ConnectedPacket.RELIABLE) {
            packet.Reliability = reliability;
            packet.ReliableIndex = LastReliablePacketIndex++;
        }

        OutgoingPackets.Add(packet);
    }

    public override string ToString() => $"RakNetConnection(IP={IP}, LastPing={LastPing}, ClientID={ClientID})";
}
