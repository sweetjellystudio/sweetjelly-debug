namespace SWeetJelly.Debug.App
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using SWeetJelly.Debug.Contract;

#nullable enable

    public sealed class DebugFormat : IDebugFormatter
    {
        public const string NullGlyph = "<null>";
        public const string Ellipsis = "...";

        private readonly int sequencePreview;

        public DebugFormat(int sequencePreview = 5)
        {
            this.sequencePreview = sequencePreview < 0 ? 0 : sequencePreview;
        }

        public string Format(DebugEntry entry)
        {
            if (entry.Level == DebugLevel.Exception)
                return FormatException(entry);

            var label = FormatLabel(entry.Label);
            var value = SafeString(entry.Value);

            if (string.IsNullOrWhiteSpace(label) && entry.Value == null)
                return NullGlyph;

            if (string.IsNullOrWhiteSpace(label))
                return value;

            if (entry.Value == null)
                return $"[{label}]";

            return $"[{label}] : {value}";
        }

        private string FormatException(DebugEntry entry)
        {
            var exception = entry.Exception;

            if (exception == null)
            {
                var label = string.IsNullOrWhiteSpace(entry.Label) ? "Exception" : entry.Label;
                return $"[{label}] : {NullGlyph} (no exception provided)";
            }

            var caption = string.IsNullOrWhiteSpace(entry.Label)
                ? exception.GetType().Name
                : entry.Label;

            var message = string.IsNullOrWhiteSpace(exception.Message)
                ? "(no message)"
                : exception.Message;

            return $"[{caption}] : {message}";
        }

        private static string? FormatLabel(string? label)
        {
            return string.IsNullOrWhiteSpace(label) ? null : label;
        }

        private string SafeString(object? value)
        {
            switch (value)
            {
                case null:
                    return NullGlyph;
                case string text:
                    return text;
                case IDictionary dictionary:
                    return FormatDictionary(dictionary);
                case IEnumerable enumerable:
                    return FormatEnumerable(enumerable);
                case IFormattable formattable:
                    return formattable.ToString(null, CultureInfo.InvariantCulture);
                default:
                    return value.ToString() ?? NullGlyph;
            }
        }

        private string FormatEnumerable(IEnumerable sequence)
        {
            var sb = new StringBuilder();
            sb.Append('[');

            var index = 0;
            foreach (var item in sequence)
            {
                if (index > 0)
                    sb.Append(", ");

                if (index >= sequencePreview)
                {
                    sb.Append(Ellipsis);
                    break;
                }

                sb.Append(item?.ToString() ?? NullGlyph);
                index++;
            }

            sb.Append(']');
            return sb.ToString();
        }

        private string FormatDictionary(IDictionary dictionary)
        {
            var sb = new StringBuilder();
            sb.Append('{');

            var index = 0;
            foreach (DictionaryEntry entry in dictionary)
            {
                if (index > 0)
                    sb.Append(", ");

                if (index >= sequencePreview)
                {
                    sb.Append(Ellipsis);
                    break;
                }

                sb.Append(entry.Key?.ToString() ?? NullGlyph);
                sb.Append(':');
                sb.Append(entry.Value?.ToString() ?? NullGlyph);
                index++;
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}
