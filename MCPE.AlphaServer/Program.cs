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

        new RakNetServer(19132)
        {
            ServerName = "MCPE.AlphaServer"
        }.Start(new GameServer(mainWorld));

        Logger.Info("MCPE.AlphaServer started.");

        await Task.Delay(Timeout.Infinite);
    }
}
