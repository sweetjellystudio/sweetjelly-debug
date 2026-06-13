namespace SWeetJelly.Debug.Contract
{
    using System;

#nullable enable

    public readonly struct DebugEntry
    {
        public DebugEntry(
            DebugLevel level,
            DebugLabelType labelType,
            string? label,
            object? value,
            Exception? exception)
        {
            Level = level;
            LabelType = labelType;
            Label = label;
            Value = value;
            Exception = exception;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public DebugLevel Level { get; }
        public DebugLabelType LabelType { get; }
        public string? Label { get; }
        public object? Value { get; }
        public Exception? Exception { get; }
        public DateTimeOffset CreatedAt { get; }
    }
}
