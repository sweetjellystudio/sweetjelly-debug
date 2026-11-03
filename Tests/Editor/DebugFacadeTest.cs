using System;
using NUnit.Framework;
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Contract;

namespace SWeetJelly.Debug.Tests.EditMode
{
    public sealed class DebugFacadeTest
    {
        [Test]
        public void SetOutput_PreservesInjectedCustomServiceAndItsConfiguration()
        {
            var original = SWJDebug.Service;
            var custom = new CustomService { Enabled = false, MinimumLevel = DebugLevel.Warning };
            try
            {
                SWJDebug.SetService(custom);
                SWJDebug.Set(new Output());
                Assert.That(SWJDebug.Service, Is.SameAs(custom));
                Assert.That(custom.Enabled, Is.False);
                Assert.That(custom.MinimumLevel, Is.EqualTo(DebugLevel.Warning));
            }
            finally { SWJDebug.SetService(original); }
        }

        private sealed class Output : IDebugger
        {
            public void Publish(DebugEntry entry, string message) { }
        }

        private sealed class CustomService : IDebugService
        {
            private readonly DebugService inner = new DebugService();
            public bool Enabled { get => inner.Enabled; set => inner.Enabled = value; }
            public DebugLevel MinimumLevel { get => inner.MinimumLevel; set => inner.MinimumLevel = value; }
            public IDebugFormatter Formatter => inner.Formatter;
            public void SetFormatter(IDebugFormatter formatter) => inner.SetFormatter(formatter);
            public void AddDebugger(IDebugger debugger) => inner.AddDebugger(debugger);
            public void RemoveDebugger(IDebugger debugger) => inner.RemoveDebugger(debugger);
            public void ClearDebuggers() => inner.ClearDebuggers();
            public void Log(DebugLevel level, DebugLabelType type, string label, object value = null) => inner.Log(level, type, label, value);
            public void LogException(DebugLabelType type, string label, Exception exception) => inner.LogException(type, label, exception);
        }
    }
}
