using System;
using System.Collections.Generic;
using NUnit.Framework;
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Contract;

namespace SWeetJelly.Debug.Tests.EditMode
{
    public sealed class DebugServiceRoutingTest
    {
        [Test]
        public void RemovingSelfDuringPublish_DoesNotSkipTheNextOutput()
        {
            var service = new DebugService();
            var received = new List<string>();
            var first = new CallbackDebugger(() => received.Add("first"));
            first.Callback = () => { received.Add("first"); service.RemoveDebugger(first); };
            service.AddDebugger(first);
            service.AddDebugger(new CallbackDebugger(() => received.Add("second")));

            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "first entry");
            CollectionAssert.AreEqual(new[] { "first", "second" }, received);
            received.Clear();
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "next entry");
            CollectionAssert.AreEqual(new[] { "second" }, received);
        }

        [Test]
        public void ReentrantLog_IsRejected_AndLaterLogsStillWork()
        {
            var service = new DebugService();
            bool nested = false;
            var debugger = new CallbackDebugger(() =>
            {
                if (nested) return;
                nested = true;
                service.Log(DebugLevel.Info, DebugLabelType.Grouping, "nested");
            });
            service.AddDebugger(debugger);
            Assert.Throws<InvalidOperationException>(() => service.Log(DebugLevel.Info, DebugLabelType.Grouping, "outer"));
            service.RemoveDebugger(debugger);
            Assert.DoesNotThrow(() => service.Log(DebugLevel.Info, DebugLabelType.Grouping, "later"));
        }

        [Test]
        public void ClearDuringPublish_AffectsOnlyLaterEntries()
        {
            var service = new DebugService(); var received = new List<string>();
            service.AddDebugger(new CallbackDebugger(() => { received.Add("first"); service.ClearDebuggers(); }));
            service.AddDebugger(new CallbackDebugger(() => received.Add("second")));
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "one");
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "two");
            CollectionAssert.AreEqual(new[] { "first", "second" }, received);
        }

        [Test]
        public void AddedTarget_WaitsUntilNextEntry_AndDuplicateRegistrationIsIgnored()
        {
            var service = new DebugService(); int addedCount = 0;
            var added = new CallbackDebugger(() => addedCount++);
            service.AddDebugger(new CallbackDebugger(() => service.AddDebugger(added)));
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "one");
            Assert.That(addedCount, Is.Zero);
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "two");
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "three");
            Assert.That(addedCount, Is.EqualTo(2));
        }

        [Test]
        public void FormatterChangingTargets_DoesNotChangeTheCurrentSnapshot()
        {
            var service = new DebugService(); int received = 0;
            service.AddDebugger(new CallbackDebugger(() => received++));
            service.SetFormatter(new CallbackFormatter(() => { service.ClearDebuggers(); return "formatted"; }));
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "one");
            service.Log(DebugLevel.Info, DebugLabelType.Grouping, "two");
            Assert.That(received, Is.EqualTo(1));
        }

        [Test]
        public void TargetFailure_StopsDelivery_PropagatesOriginalException_AndReleasesGuard()
        {
            var service = new DebugService(); int second = 0; var error = new InvalidOperationException("target failed");
            var failing = new CallbackDebugger(() => throw error);
            service.AddDebugger(failing); service.AddDebugger(new CallbackDebugger(() => second++));
            Assert.That(Assert.Throws<InvalidOperationException>(() => service.Log(DebugLevel.Info, DebugLabelType.Grouping, "one")), Is.SameAs(error));
            Assert.That(second, Is.Zero);
            service.RemoveDebugger(failing); service.Log(DebugLevel.Info, DebugLabelType.Grouping, "two");
            Assert.That(second, Is.EqualTo(1));
        }

        [Test]
        public void FormatterFailure_PropagatesOriginalException_AndReleasesGuard()
        {
            var service = new DebugService(); var error = new InvalidOperationException("formatter failed");
            service.SetFormatter(new CallbackFormatter(() => throw error));
            Assert.That(Assert.Throws<InvalidOperationException>(() => service.Log(DebugLevel.Info, DebugLabelType.Grouping, "one")), Is.SameAs(error));
            service.SetFormatter(new DebugFormat());
            Assert.DoesNotThrow(() => service.Log(DebugLevel.Info, DebugLabelType.Grouping, "later"));
        }

        [TestCase(false, DebugLevel.Trace)]
        [TestCase(true, DebugLevel.Trace)]
        [TestCase(true, DebugLevel.Info)]
        public void DisabledOrBelowMinimum_DoesNotFormat(bool enabled, DebugLevel level)
        {
            var service = new DebugService { Enabled = enabled, MinimumLevel = DebugLevel.Warning };
            service.SetFormatter(new CallbackFormatter(() => throw new Exception("must not format")));
            Assert.DoesNotThrow(() => service.Log(level, DebugLabelType.Grouping, "filtered"));
        }

        [Test]
        public void IndependentServiceForwarding_IsAllowed_AndFilteredRecursionIsIgnored()
        {
            var first = new DebugService { MinimumLevel = DebugLevel.Warning }; var second = new DebugService(); int received = 0;
            second.AddDebugger(new CallbackDebugger(() => received++));
            first.AddDebugger(new CallbackDebugger(() =>
            {
                first.Log(DebugLevel.Info, DebugLabelType.Grouping, "filtered");
                second.Log(DebugLevel.Info, DebugLabelType.Grouping, "forwarded");
            }));
            first.Log(DebugLevel.Warning, DebugLabelType.Grouping, "outer");
            Assert.That(received, Is.EqualTo(1));
        }

        private sealed class CallbackFormatter : IDebugFormatter
        {
            private readonly Func<string> format;
            public CallbackFormatter(Func<string> format) { this.format = format; }
            public string Format(DebugEntry entry) => format();
        }

        private sealed class CallbackDebugger : IDebugger
        {
            public Action Callback;
            public CallbackDebugger(Action callback) { Callback = callback; }
            public void Publish(DebugEntry entry, string message) => Callback();
        }
    }
}
