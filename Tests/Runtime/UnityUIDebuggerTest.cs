using System.Collections;
using NUnit.Framework;
using SWeetJelly.Debug.Contract;
using SWeetJelly.Debug.Framework.Unity;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace SWeetJelly.Debug.Tests.PlayMode
{
    public sealed class UnityUIDebuggerTest
    {
        [UnityTest]
        public IEnumerator SetOutput_RendersRetainedEntriesImmediately()
        {
            var holder = new GameObject("UI debugger");
            var output = new GameObject("Borrowed output", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            try
            {
                var debugger = holder.AddComponent<UnityUIDebugger>();
                var text = output.GetComponent<Text>();
                debugger.Publish(new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, "test", null, null), "before binding");
                debugger.SetOutput(text);
                Assert.That(text.text, Is.EqualTo("before binding"));
                yield return null;
            }
            finally { Object.Destroy(holder); Object.Destroy(output); }
        }
        [UnityTest]
        public IEnumerator EntryBound_ClampsAndTrimsImmediately_AndClearEmptiesDisplay()
        {
            var holder = new GameObject("UI history");
            var output = new GameObject("Borrowed output", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            try
            {
                var debugger = holder.AddComponent<UnityUIDebugger>(); var text = output.GetComponent<Text>();
                debugger.SetOutput(text); debugger.SetMaxLines(2);
                var entry = new DebugEntry(DebugLevel.Info, DebugLabelType.Grouping, "test", null, null);
                debugger.Publish(entry, "one"); debugger.Publish(entry, "two"); debugger.Publish(entry, "three");
                Assert.That(text.text, Is.EqualTo("two\nthree"));
                debugger.SetMaxLines(0); Assert.That(text.text, Is.EqualTo("three"));
                debugger.Publish(entry, "one\ntwo"); Assert.That(text.text, Is.EqualTo("one\ntwo"));
                debugger.Clear(); Assert.That(text.text, Is.Empty);
                yield return null;
            }
            finally { Object.Destroy(holder); Object.Destroy(output); }
        }

        [UnityTest]
        public IEnumerator DestroyingTarget_DoesNotDestroyBorrowedText()
        {
            var holder = new GameObject("UI owner");
            var output = new GameObject("Borrowed output", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            var debugger = holder.AddComponent<UnityUIDebugger>(); debugger.SetOutput(output.GetComponent<Text>());
            Object.Destroy(holder); yield return null;
            Assert.That(output != null, Is.True); Object.Destroy(output);
        }
    }
}
