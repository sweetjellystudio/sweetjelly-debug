namespace SWeetJelly.Debug.Contract
{
    using System;

#nullable enable

    public interface IDebugService
    {
        bool Enabled { get; set; }
        DebugLevel MinimumLevel { get; set; }
        IDebugFormatter Formatter { get; }

        void SetFormatter(IDebugFormatter formatter);
        void AddDebugger(IDebugger debugger);
        void RemoveDebugger(IDebugger debugger);
        void ClearDebuggers();

        void Log(DebugLevel level, DebugLabelType labelType, string? label, object? value = null);
        void LogException(DebugLabelType labelType, string? label, Exception? exception);
    }
}
