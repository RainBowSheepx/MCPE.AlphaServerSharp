using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MCPE.AlphaServer.Game;
using MCPE.AlphaServer.Game.Generator;
using MCPE.AlphaServer.Network;
using MCPE.AlphaServer.RakNet;
using MCPE.AlphaServer.Utils;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace MCPE.AlphaServer;

internal static class Program {
    private static async Task Main(string[] _) {
//#if DEBUG
       // Directory.SetCurrentDirectory("work");
        //#endif

        var mainWorld = new World(666);
        Block.init();
        FlatWorldGenerator.generateChunks(mainWorld);
        Logger.LogBackend = new LoggerConfiguration()
.WriteTo.Console(theme: SystemConsoleTheme.Colored)
.MinimumLevel.Debug()
.CreateLogger();
        //   mainWorld.PrintEntitiesData();
        Console.WriteLine("Level Data:");
        mainWorld.PrintLevelData();



        Logger.Info("MCPE.AlphaServer starting.");

        new RakNetServer(19132) {
            ServerName = "MCPE.AlphaServer"
        }.Start(new GameServer(mainWorld));

        Logger.Info("MCPE.AlphaServer started.");

        await Task.Delay(Timeout.Infinite);
    }
}
