namespace SWeetJelly.Debug.App
{
    using System;
    using System.Collections.Generic;
    using SWeetJelly.Debug.Contract;

#nullable enable

    public sealed class DebugService : IDebugService
    {
        private readonly List<IDebugger> debuggers = new List<IDebugger>();
        private bool isPublishing;

        public DebugService()
            : this(new DebugFormat())
        {
        }

        public DebugService(IDebugFormatter formatter)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            Enabled = true;
            MinimumLevel = DebugLevel.Trace;
        }

        public bool Enabled { get; set; }
        public DebugLevel MinimumLevel { get; set; }
        public IDebugFormatter Formatter { get; private set; }

        public void SetFormatter(IDebugFormatter formatter)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public void AddDebugger(IDebugger debugger)
        {
            if (debugger == null)
                throw new ArgumentNullException(nameof(debugger));

            if (!debuggers.Contains(debugger))
                debuggers.Add(debugger);
        }

        public void RemoveDebugger(IDebugger debugger)
        {
            if (debugger == null)
                return;

            debuggers.Remove(debugger);
        }

        public void ClearDebuggers()
        {
            debuggers.Clear();
        }

        public void Log(DebugLevel level, DebugLabelType labelType, string? label, object? value = null)
        {
            Publish(new DebugEntry(level, labelType, label, value, null));
        }

        public void LogException(DebugLabelType labelType, string? label, Exception? exception)
        {
            Publish(new DebugEntry(DebugLevel.Exception, labelType, label, null, exception));
        }

        private void Publish(DebugEntry entry)
        {
            if (!Enabled || entry.Level < MinimumLevel)
                return;

            if (isPublishing)
                throw new InvalidOperationException("Logging cannot reenter the same debug service.");

            isPublishing = true;
            try
            {
                var targets = debuggers.ToArray();
                var formattedMessage = Formatter.Format(entry);

                for (var i = 0; i < targets.Length; i++)
                    targets[i].Publish(entry, formattedMessage);
            }
            finally { isPublishing = false; }
        }
    }
}
