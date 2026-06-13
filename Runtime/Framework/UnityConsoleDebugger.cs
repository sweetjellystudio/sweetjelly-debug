namespace SWeetJelly.Debug.Framework
{
    using SWeetJelly.Debug.Contract;
    using UnityEngine;

    public sealed class UnityConsoleDebugger : IDebugger
    {
        public void Publish(DebugEntry entry, string formattedMessage)
        {
            var message = FormatForUnity(entry, formattedMessage);

            switch (entry.Level)
            {
                case DebugLevel.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case DebugLevel.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
                case DebugLevel.Exception:
                    UnityEngine.Debug.LogError(message);
                    if (entry.Exception != null)
                        UnityEngine.Debug.LogException(entry.Exception);
                    break;
                default:
                    UnityEngine.Debug.Log(message);
                    break;
            }
        }

        private static string FormatForUnity(DebugEntry entry, string formattedMessage)
        {
            if (entry.Value is Object unityObject)
                return FormatUnityObject(entry, unityObject);

            if (string.IsNullOrWhiteSpace(entry.Label))
                return formattedMessage;

            var labelColor = entry.LabelType == DebugLabelType.Origin
                ? "#6FB1FF"
                : "#A0A0A0";

            var coloredLabel = $"<color={labelColor}>{entry.Label}</color>";
            var rawLabel = $"[{entry.Label}]";

            return formattedMessage.StartsWith(rawLabel)
                ? formattedMessage.Replace(rawLabel, $"[{coloredLabel}]")
                : formattedMessage;
        }

        private static string FormatUnityObject(DebugEntry entry, Object unityObject)
        {
            var value = unityObject == null
                ? "<null>"
                : $"{(string.IsNullOrWhiteSpace(unityObject.name) ? unityObject.GetType().Name : unityObject.name)} ({unityObject.GetType().Name})";

            if (string.IsNullOrWhiteSpace(entry.Label))
                return value;

            var labelColor = entry.LabelType == DebugLabelType.Origin
                ? "#6FB1FF"
                : "#A0A0A0";

            return $"[<color={labelColor}>{entry.Label}</color>] : {value}";
        }
    }
}
