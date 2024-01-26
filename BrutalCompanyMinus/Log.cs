using BepInEx.Logging;

namespace BrutalCompanyMinus
{
    internal class Log
    {
        internal static ManualLogSource logSource;

        internal static void Initalize(ManualLogSource LogSource) => logSource = LogSource;
        internal static void LogDebug(object data) => logSource.LogDebug(data);
        internal static void LogError(object data) => logSource.LogError(data);
        internal static void LogFatal(object data) => logSource.LogFatal(data);
        internal static void LogInfo(object data) => logSource.LogInfo(data);
        internal static void LogMessage(object data) => logSource.LogMessage(data);
        internal static void LogWarning(object data) => logSource.LogWarning(data);
    }
}
