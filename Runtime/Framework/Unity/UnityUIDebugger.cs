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

            Render();
        }

        public void SetOutput(Text text)
        {
            output = text;
            Render();
        }

        public void SetMaxLines(int value)
        {
            maxLines = value > 0 ? value : 1;
            while (lines.Count > maxLines) lines.Dequeue();
            Render();
        }

        public void Clear()
        {
            lines.Clear();

            Render();
        }

        private void Render()
        {
            if (output != null) output.text = string.Join("\n", lines);
        }

        private void OnValidate()
        {
            if (maxLines <= 0) maxLines = 1;
        }
    }
}
