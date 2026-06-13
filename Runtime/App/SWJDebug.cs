namespace SWeetJelly.Debug.App
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using SWeetJelly.Debug.Contract;

#nullable enable

    public static class SWJDebug
    {
        private static IDebugService? service;

        public static IDebugService? Service => service;

        public static void SetService(IDebugService? debugService)
        {
            service = debugService;
        }

        public static void Set(IDebugger debugger)
        {
            if (debugger == null)
                return;

            var debugService = service as DebugService ?? new DebugService();
            debugService.ClearDebuggers();
            debugService.AddDebugger(debugger);
            service = debugService;
        }

        public static void Log(string label, object? value = null)
        {
            service?.Log(DebugLevel.Info, DebugLabelType.Grouping, label, value);
        }

        public static void LogWarning(string label, object? value = null)
        {
            service?.Log(DebugLevel.Warning, DebugLabelType.Grouping, label, value);
        }

        public static void LogError(string label, object? value = null)
        {
            service?.Log(DebugLevel.Error, DebugLabelType.Grouping, label, value);
        }

        public static void LogException(string label, Exception? exception)
        {
            service?.LogException(DebugLabelType.Grouping, label, exception);
        }

        public static void TraceLog(
            object? value = null,
            [CallerMemberName] string? member = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0)
        {
            service?.Log(DebugLevel.Trace, DebugLabelType.Origin, AutoLabel(member, file, line), value);
        }

        public static void TraceWarning(
            object? value = null,
            [CallerMemberName] string? member = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0)
        {
            service?.Log(DebugLevel.Warning, DebugLabelType.Origin, AutoLabel(member, file, line), value);
        }

        public static void TraceError(
            object? value = null,
            [CallerMemberName] string? member = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0)
        {
            service?.Log(DebugLevel.Error, DebugLabelType.Origin, AutoLabel(member, file, line), value);
        }

        public static void TraceException(
            Exception? exception,
            [CallerMemberName] string? member = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0)
        {
            service?.LogException(DebugLabelType.Origin, AutoLabel(member, file, line), exception);
        }

        private static string AutoLabel(string? member, string? file, int line)
        {
            var fileName = file;

            try
            {
                fileName = string.IsNullOrWhiteSpace(file)
                    ? "Unknown"
                    : Path.GetFileNameWithoutExtension(file);
            }
            catch
            {
                fileName = "Unknown";
            }

            return $"{fileName}.{member ?? "Unknown"}:{line}";
        }
    }
}
