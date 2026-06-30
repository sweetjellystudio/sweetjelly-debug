namespace SWeetJelly.Debug.Framework.Unity
{
    using System.Collections.Generic;
    using SWeetJelly.Debug.Contract;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class UnityUIDebugger : MonoBehaviour, IDebugger
    {
        [SerializeField] private Text output;
        [SerializeField] private int maxLines = 20;

        private readonly Queue<string> lines = new Queue<string>();

        public void Publish(DebugEntry entry, string formattedMessage)
        {
            if (maxLines <= 0)
                maxLines = 1;

            lines.Enqueue(formattedMessage);

            while (lines.Count > maxLines)
                lines.Dequeue();

            if (output != null)
                output.text = string.Join("\n", lines);
        }

        public void SetOutput(Text text)
        {
            output = text;
        }

        public void Clear()
        {
            lines.Clear();

            if (output != null)
                output.text = string.Empty;
        }
    }
}
