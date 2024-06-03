using SpoongePE.Core.Game.player;
using SpoongePE.Core.RakNet;
using SpoongePE.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Network
{
    public class QueryHandler
    {
        private RakNetServer Server;
        private IPEndPoint IP;

        public int lastToken;
        public int token;

        public Dictionary<string, string> query = new Dictionary<string, string>()
        {
            { "splitnum", ((char)128).ToString() },
            { "hostname", RakNetServer.Properties.serverName },
            { "gametype", RakNetServer.Properties.gamemode == false ? "SMP" : "CMP" },
            { "game_id", "MINECRAFTPE" },
            { "version", "v0.8.1 alpha" },
            { "server_engine", "SpoongePE.Core Best C# Core Ever" },
            { "plugins", "SpoongePE.Core Best C# Core Ever" },
            { "map", "world" },
            { "numplayers", "" },
            { "maxplayers", RakNetServer.Properties.maxPlayers.ToString() },
            { "whitelist", "off" },
            { "hostport", RakNetServer.Properties.serverPort.ToString() },
            { "tps", "999" },
        };
        public QueryHandler(IPEndPoint endPoint, RakNetServer server)
        {
            Server = server;
            IP = endPoint;
            RakNetServer.StartRepeatingTask(regenerateToken, TimeSpan.FromMilliseconds(30000));
            regenerateToken();
        }

        public Task regenerateToken()
        {
            Random rnd = new Random();
            byte[] b = new byte[4];
            rnd.NextBytes(b);
            b[0] = 0;
            lastToken = token;
            DataReader reader = new DataReader(b);
            token = reader.Int(); // Какая же залупа. Какой кретин писал этот аналог кода в pmmp?
            return Task.CompletedTask;
        }

        public string regenerateInfo()
        {
            string temp = "";

            this.query["numplayers"] = Server.GameHandler.ServerWorld.Players.Count().ToString();
            foreach (string key in query.Keys)
            {
                temp += key + "\x00" + query[key] + "\x00";
            }
            temp += "\x00\x01player_\x00\x00";
            foreach (Player pl in Server.GameHandler.ServerWorld.Players)
            {
                if (pl.Username != "")
                {
                    temp += pl.Username + "\x00";
                }
            }
            temp += "\x00";
            return temp;
        }
        public void QueryHandle(DataReader reader)
        {
            QueryPacket pk = new QueryPacket();
            pk.Decode(ref reader);
            if (pk.packetType == QueryPacket.HANDSHAKE)
            {
                QueryPacket pkk = new QueryPacket();
                pkk.packetType = QueryPacket.HANDSHAKE;
                pkk.sessionID = pk.sessionID;
                List<byte> temp = new List<byte>();
                foreach (char c in this.token + "\x00")
                {
                    temp.Add(System.Convert.ToByte(c));
                }
                pkk.payload = temp.ToArray();
                DataWriter dataWriter = new DataWriter();
                pkk.Encode(ref dataWriter);
                this.Server.UDP.Send(dataWriter.GetBytes(), dataWriter.GetBytes().Length, IP);
                Logger.PInfo("Sent handshake to " + IP.Address);
                var test2 = string.Join(" ", dataWriter.GetBytes());
                //     Logger.Info("test stack: " + test2 + " Size: " + dataWriter.GetBytes().Length);
            }
            else if (pk.packetType == QueryPacket.STATISTICS)
            {
                QueryPacket pkk = new QueryPacket();
                pkk.packetType = QueryPacket.STATISTICS;
                pkk.sessionID = pk.sessionID;

                List<byte> temp = new List<byte>();
                foreach (char c in regenerateInfo())
                {
                    if (!Char.IsAscii(c)) break;
                    temp.Add(System.Convert.ToByte(c));
                }
                pkk.payload = temp.ToArray();
                DataWriter dataWriter = new DataWriter();
                pkk.Encode(ref dataWriter);
                this.Server.UDP.Send(dataWriter.GetBytes(), dataWriter.GetBytes().Length, IP);
                Logger.PInfo("Sent stats to " + IP.Address);
                var test2 = string.Join(" ", dataWriter.GetBytes());
                //   Logger.Info("test stack: " + test2 + " Size: " + dataWriter.GetBytes().Length);
            }
            else
            {
                Logger.PWarn("Unknown message was get in query! " + pk.packetType);
            }
        }
    }


    public class QueryPacket
    {
        public const int HANDSHAKE = 9;
        public const int STATISTICS = 0;

        public byte packetType;
        public int sessionID;
        public byte[] payload;
        public void Decode(ref DataReader reader)
        {
            this.packetType = reader.Byte();
            this.sessionID = reader.Int();
            if (!reader.IsEof)
                this.payload = reader.ReadAll().ToArray();
        }
        public void Encode(ref DataWriter writer)
        {
            writer.Byte(packetType);
            writer.Int(sessionID);
            writer.RawData(payload);
        }
    }
}
