using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SpoongePE.Core.Network;
using SpoongePE.Core.Utils;

namespace SpoongePE.Core.RakNet;

public class RakNetClient
{
    public enum ConnectionStatus
    {
        CONNECTING,
        CONNECTED,
        DISCONNECTING,
        DISCONNECTED
    }

    public RakNetClient(IPEndPoint endPoint, RakNetServer server)
    {
        IP = endPoint;
        LastPing = DateTime.Now;
        Status = ConnectionStatus.CONNECTING;
        OutgoingPackets = new List<ConnectedPacket>();
        SplitPackets = new List<ConnectedPacket>();
        ResendPackets = new List<ConnectedPacket>();
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

    private List<ConnectedPacket> SplitPackets { get; }

    private List<ConnectedPacket> ResendPackets { get; }
    private SortedSet<int> NeedsACK { get; }
    private int CurrentSequenceNumber;
    private int LastReliablePacketIndex;
    public ushort mtuSize;
    public Dictionary<int, ConnectedPacket> resendQueue = new Dictionary<int, ConnectedPacket>();

    internal RakNetServer Server;

    public bool IsTimedOut => DateTime.Now - LastPing > TimeSpan.FromSeconds(5);
    public bool IsConnected => !IsTimedOut && Status != ConnectionStatus.DISCONNECTED;

    internal void HandlePacket(byte[] data)
    {
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

    private void HandleACK(ref DataReader reader)
    {
        var packet = ConnectedPacket.ParseMeta(ref reader);
        // Logger.Warn($"TODO: HandleACK {packet}");
        for (int i = 0; i < packet.Ranges.Length; ++i)
        {
            (int min, int max) = packet.Ranges[i];
            for (; min <= max; ++min)
            {
                if (!this.resendQueue.Remove(min))
                {
                    Logger.Warn($"HandleACK Failed to remove {min} from resendQueue {packet}");
                }
            }
        }
    }

    private void HandleNAK(ref DataReader reader)
    {
        var packet = ConnectedPacket.ParseMeta(ref reader);
        Logger.Warn($"TODO: HandleNAK {packet}");
        for (int i = 0; i < packet.Ranges.Length; ++i)
        {
            (int min, int max) = packet.Ranges[i];
            for (; min <= max; ++min)
            {
                var pk = this.resendQueue.GetValueOrDefault(min, null);
                if (pk == null)
                {
                    Logger.Warn($"HandleNAK Failed to resend {min} from resendQueue because it doesnt exist. {packet}");
                }
                else
                {
                    Logger.Warn($"Resending {min}");

                    this.ResendPackets.Add(pk); // yez
                }
            }
        }

    }

    private void HandleConnected(ref DataReader reader)
    {
        reader.Byte();
        var sequenceNumber = reader.Triad();
        NeedsACK.Add(sequenceNumber);

        do
        {
            switch (ConnectedPacket.Parse(ref reader))
            {
                case ConnectedPingPacket ping:
                    Send(new ConnectedPongPacket
                    {
                        TimeSinceStart = ping.TimeSinceStart,
                        TimeSinceServerStart = 0,
                    }, ConnectedPacket.RELIABLE
                    );
                    break;
                case ConnectionRequestPacket:
                    Send(new ConnectionRequestAcceptedPacket
                    {
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

    public bool wait = false;

    private async Task SendACKs()
    {
        if (NeedsACK.Count < 1)
            return;

        // Send ACKs.
        var ackWriter = new DataWriter();
        ackWriter.Byte(UnconnectedPacket.IS_CONNECTED | UnconnectedPacket.IS_ACK);

        // TODO: Use the range feature from RakNet?
        ackWriter.Short((short)NeedsACK.Count);
        foreach (var sequence in NeedsACK)
        {
            ackWriter.Byte(1); // Min == max.
            ackWriter.Triad(sequence);
        }

        await Server.UDP.SendAsync(ackWriter.GetBytes(), IP);
        NeedsACK.Clear();
    }

    internal async Task HandleResendPacketInstantly()
    {
        if (ResendPackets.Count < 1) {
            await SendACKs();
            return;
        }
        for (int i = 0; i < ResendPackets.Count; i++)
        {
            ConnectedPacket packet = ResendPackets[i];
            var writerResend = new DataWriter();
            int sequenceNumber = 0;
            var packetWriter = new DataWriter();

            writerResend = new DataWriter();
            writerResend.Byte(UnconnectedPacket.IS_CONNECTED);
            sequenceNumber = CurrentSequenceNumber++;
            writerResend.Triad(sequenceNumber);
            packetWriter = new DataWriter();
            packet.Encode(ref packetWriter);
            writerResend.Byte((byte)((packet.Reliability << 5) | (packet.hasSplit ? 0x10 : 0)));
            writerResend.Short((short)(packetWriter.Length << 3));

            switch (packet.Reliability)
            {
                case ConnectedPacket.RELIABLE_WITH_ACK_RECEIPT:
                    this.resendQueue.Add(sequenceNumber, packet);
                    writerResend.Triad(packet.ReliableIndex);
                    break;
                case ConnectedPacket.RELIABLE:
                    writerResend.Triad(packet.ReliableIndex);
                    break;
                case ConnectedPacket.RELIABLE_ORDERED:
                    writerResend.Triad(packet.ReliableIndex);
                    writerResend.Triad(packet.OrderingIndex);
                    writerResend.Byte((byte)packet.OrderingChannel);
                    break;
            }
            if (packet.hasSplit)
            {
                writerResend.Int(packet.splitCount);
                writerResend.Short(packet.splitID);
                writerResend.Int(packet.splitIndex);
            }


            writerResend.RawData(packetWriter.GetBytes());

            var test2 = string.Join(" ", writerResend.GetBytes());


            //Logger.Info("test stack: " + test2 + " Size: " + writerSplit.GetBytes().Length);
            await Server.UDP.SendAsync(writerResend.GetBytes(), IP);
            resendQueue.Add(sequenceNumber, packet);
            ResendPackets.Remove(packet);
        }
   
    }

    internal async Task HandleSplitPackets()
    {
        if (SplitPackets.Count < 1)
        {
            await SendACKs();
            return;
        }


        ConnectedPacket packet = SplitPackets.First();
        var writerSplit = new DataWriter();
        int sequenceNumber = 0;
        var packetWriter = new DataWriter();
        for (int i = 0; i < SplitPackets.Count; i++)
        {
            packet = SplitPackets[i];
            writerSplit = new DataWriter();
            writerSplit.Byte(UnconnectedPacket.IS_CONNECTED);
            sequenceNumber = CurrentSequenceNumber++;
            writerSplit.Triad(sequenceNumber);
            packetWriter = new DataWriter();
            packet.Encode(ref packetWriter);
            writerSplit.Byte((byte)((packet.Reliability << 5) | (packet.hasSplit ? 0x10 : 0)));
            writerSplit.Short((short)(packetWriter.Length << 3));

            switch (packet.Reliability)
            {
                case ConnectedPacket.RELIABLE_WITH_ACK_RECEIPT:
                    this.resendQueue.Add(sequenceNumber, packet);
                    writerSplit.Triad(packet.ReliableIndex);
                    break;
                case ConnectedPacket.RELIABLE:
                    writerSplit.Triad(packet.ReliableIndex);
                    break;
                case ConnectedPacket.RELIABLE_ORDERED:
                    writerSplit.Triad(packet.ReliableIndex);
                    writerSplit.Triad(packet.OrderingIndex);
                    writerSplit.Byte((byte)packet.OrderingChannel);
                    break;
            }
            writerSplit.Int(packet.splitCount);
            writerSplit.Short(packet.splitID);
            writerSplit.Int(packet.splitIndex);

            writerSplit.RawData(packetWriter.GetBytes());

            var test2 = string.Join(" ", writerSplit.GetBytes());


            //Logger.Info("test stack: " + test2 + " Size: " + writerSplit.GetBytes().Length);
            await Server.UDP.SendAsync(writerSplit.GetBytes(), IP);
            resendQueue.Add(sequenceNumber, packet);
            SplitPackets.Remove(packet);

        }

    }
    internal async Task HandleOutgoing()
    {
        if (OutgoingPackets.Count < 1)
        {
            await SendACKs();
            return;
        }


        var writer = new DataWriter();

        writer.Byte(UnconnectedPacket.IS_CONNECTED);
        int seq = CurrentSequenceNumber++;
        writer.Triad(seq);
        foreach (var packet in OutgoingPackets)
        {
            var packetWriter = new DataWriter();
            packet.Encode(ref packetWriter);
            writer.Byte((byte)((packet.Reliability << 5)));
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
        var strs = string.Join(" ", writer.GetBytes());
        // Logger.Info(strs + " Size: " + writer.GetBytes().Length);

        if (writer.GetBytes().Length > this.mtuSize)
        {
            Logger.Warn("Packet size is too big! Maybe connection troubles! ");
        }
        UnknowPacket pak = new UnknowPacket();
        pak.buffer = writer.GetBytes();
        await Server.UDP.SendAsync(writer.GetBytes(), IP);
        resendQueue.Add(seq, pak);
        OutgoingPackets.Clear();
    }


    private List<ConnectedPacket> Queue = new List<ConnectedPacket>();
    public void Send(ConnectedPacket packet, int reliability = ConnectedPacket.RELIABLE)
    {
        int offsetMTU = RakNetServer.Properties.offsetMTU; // 414 is default for pmmp. PMMP Bandwith is 1086 // Increase if you have connection trouble
        var packetChecker = new DataWriter();
        packet.Encode(ref packetChecker);
        if (packetChecker.GetBytes().Length > this.mtuSize - offsetMTU)
        {
            List<byte[]> fragmented_body = new List<byte[]>();
            byte[] test = packetChecker.GetBytes();
            try
            {
                for (int i = 0; i < test.Length; i += this.mtuSize - offsetMTU)
                {
                    if (i + (this.mtuSize - offsetMTU) > test.Length)
                    {
                        int t = (i + (this.mtuSize - offsetMTU)) - ((i + (this.mtuSize - offsetMTU)) - test.Length);

                        fragmented_body.Add(test[i..t]);
                    }
                    else
                    {
                        fragmented_body.Add(test[i..(i + this.mtuSize - offsetMTU)]);
                    }



                }
            }
            catch (Exception e)
            {
                Logger.Info(e.Message);
            }


            bigCnt = (bigCnt + 1) % 0x10000;

            for (int i = 0; i < fragmented_body.Count; i++)
            {
                UnknowPacket newpacket = new UnknowPacket();
                newpacket.ReliableIndex = LastReliablePacketIndex++;
                //newpacket.packetID = (byte)MinecraftPacketType.ChunkData;
                newpacket.hasSplit = true;
                newpacket.Reliability = ConnectedPacket.RELIABLE;
                newpacket.splitID = (short)bigCnt;
                newpacket.splitCount = fragmented_body.Count;
                newpacket.splitIndex = i;
                newpacket.buffer = fragmented_body[i];
                SplitPackets.Add(newpacket);
                // Logger.Info("Sending " + fragmented_body[i].Length);
            }
        }
        else
        {
            if (reliability == ConnectedPacket.RELIABLE)
            {
                packet.Reliability = reliability;
                packet.ReliableIndex = LastReliablePacketIndex++;

            }
            OutgoingPackets.Add(packet);
        }

    }

    public override string ToString() => $"RakNetConnection(IP={IP}, LastPing={LastPing}, ClientID={ClientID})";
}
