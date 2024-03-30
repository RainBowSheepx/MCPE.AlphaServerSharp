using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using System.Collections;

namespace SpoongePE.Core.Utils
{
    internal class YmlProp
    {
        public ServerProperties LoadServerProp()
        {
            ServerProperties prop = null;
            if (File.Exists("server.properties"))
            {
                var ysml = "";
                try
                {
                    ysml = File.ReadAllText("server.properties");
                    var deserializer = new DeserializerBuilder().Build();

                    prop = deserializer.Deserialize<ServerProperties>(ysml);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    prop = new ServerProperties();
                    var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
                    var yaml = serializer.Serialize(prop);
                    File.WriteAllText("server.properties", yaml);
                }

            }
            else
            {
                prop = new ServerProperties();
                var serializer = new SerializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();
                var yaml = serializer.Serialize(prop);
                File.WriteAllText("server.properties", yaml);
            }

            return prop;
        }
    }


    public class ServerProperties
    {
        public string serverName { get; set; } = "SpoongePE";
        public string description { get; set; } = "Best C# Core Ever";
        public string motd { get; set; } = "Welcome @player to @servername server!";

        public int serverPort { get; set; } = 19132;

        public int maxPlayers { get; set; } = 20;

        public bool gamemode { get; set; } = false;

        public string levelName { get; set; } = "world";

        public int levelSeed { get; set; }
        public string levelType { get; set; } = "flat";

        public int offsetMTU { get; set; } = 24;




    }
}
