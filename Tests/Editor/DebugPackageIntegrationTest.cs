namespace SWeetJelly.Debug.Tests.EditMode
{
    using NUnit.Framework;
    using SWeetJelly.Debug.App;
    using SWeetJelly.Debug.Contract;
    using SWeetJelly.Debug.Framework.Bootstrapper;
    using SWeetJelly.Bootstrapper.App;
    using SWeetJelly.Bootstrapper.Framework.Unity;
    using UnityEngine;

    public sealed class DebugPackageIntegrationTest
    {
        [Test]
        public void DebugPackageBootstrapperSo_RegistersDebugServiceAndSetsFacade()
        {
            var holder = new GameObject("Debug Test Holder");
            var module = ScriptableObject.CreateInstance<DebugPackageBootstrapperSo>();

            try
            {
                var registry = new ServiceRegistry();
                var context = new PackageBootstrapContext(holder, null, null, registry);

                module.Bootstrap(context);

                Assert.That(registry.TryResolve<IDebugService>(out var service), Is.True);
                Assert.That(service, Is.Not.Null);
                Assert.That(SWJDebug.Service, Is.SameAs(service));

                SWJDebug.Log("Debug", "Ready");
                SWJDebug.TraceLog("Trace Ready");

                module.Shutdown(context);

                Assert.That(registry.TryResolve<IDebugService>(out _), Is.False);
                Assert.That(SWJDebug.Service, Is.Null);
            }
            finally
            {
                SWJDebug.SetService(null);
                Object.DestroyImmediate(module);
                Object.DestroyImmediate(holder);
            }
        }
    }
}
