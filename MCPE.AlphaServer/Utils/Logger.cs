using Serilog;

namespace SpoongePE.Core.Utils;

public static class Logger
{
    public static ILogger LogBackend;

    public static void Debug(string message) => LogBackend?.Debug(message);

    public static void PDebug(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") => LogBackend?.Debug($"[{memberName}] {message}");
    public static void Info(string message) => LogBackend?.Information(message);

    public static void PInfo(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") => LogBackend?.Information($"[{memberName}] {message}");
    public static void Warn(string message) => LogBackend?.Warning(message);

    public static void PWarn(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") => LogBackend?.Warning($"[{memberName}] {message}");
    public static void Error(string message) => LogBackend?.Error(message);

    public static void PError(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "") => LogBackend?.Error($"[{memberName}] {message}");
}
