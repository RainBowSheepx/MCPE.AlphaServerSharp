using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SpoongePE.Core.Game;
using SpoongePE.Core.Game.Generator;
using SpoongePE.Core.Network;
using SpoongePE.Core.RakNet;
using SpoongePE.Core.Utils;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace SpoongePE.Core;

internal static class Program
{
    private static async Task Main(string[] _)
    {
        //#if DEBUG
        // Directory.SetCurrentDirectory("work");
        //#endif
        ServerProperties prop = new YmlProp().LoadServerProp();
        var mainWorld = new World(666);
        Block.init();
        Logger.LogBackend = new LoggerConfiguration()
        .WriteTo.Console(theme: SystemConsoleTheme.Colored)
        .MinimumLevel.Debug()
        .CreateLogger();
        FlatWorldGenerator.generateChunks(mainWorld);

        //   mainWorld.PrintEntitiesData();
        Console.WriteLine("Level Data:");
        mainWorld.PrintLevelData();



        Logger.Info("SpoongePE.Core starting.");

        new RakNetServer(prop.serverPort)
        {
            Properties = prop
        }.Start(new GameServer(mainWorld));

        Logger.Info("SpoongePE.Core started.");

        await Task.Delay(Timeout.Infinite);
    }
}
