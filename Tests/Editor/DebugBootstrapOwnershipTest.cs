using System;
using NUnit.Framework;
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Contract;
using SWeetJelly.Debug.Framework.Bootstrapper;
using SWeetJelly.Bootstrapper.App;
using SWeetJelly.Bootstrapper.Framework.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SWeetJelly.Debug.Tests.EditMode
{
    public sealed class DebugBootstrapOwnershipTest
    {
        [Test]
        public void ExistingFacade_IsPreservedWhenBootstrapIsRejected()
        {
            var previous = SWJDebug.Service;
            var holder = new GameObject("Debug ownership test");
            var module = ScriptableObject.CreateInstance<DebugPackageBootstrapperSo>();
            var registry = new ServiceRegistry();
            var context = new PackageBootstrapContext(holder, null, null, registry);
            var foreign = new DebugService();
            try
            {
                SWJDebug.SetService(foreign);
                Assert.Throws<InvalidOperationException>(() => module.Bootstrap(context));
                Assert.That(SWJDebug.Service, Is.SameAs(foreign));
                Assert.That(registry.TryResolve<IDebugService>(out _), Is.False);
                module.Shutdown(context);
                Assert.That(SWJDebug.Service, Is.SameAs(foreign));
            }
            finally
            {
                module.Shutdown(context);
                SWJDebug.SetService(previous);
                Object.DestroyImmediate(module);
                Object.DestroyImmediate(holder);
            }
        }
        [Test]
        public void ExistingRegistryRegistration_AndReplacementOwners_SurviveShutdown()
        {
            var previous = SWJDebug.Service; SWJDebug.SetService(null);
            var holder = new GameObject("registry ownership");
            var module = ScriptableObject.CreateInstance<DebugPackageBootstrapperSo>();
            var registry = new ServiceRegistry(); var foreign = new DebugService();
            var context = new PackageBootstrapContext(holder, null, null, registry);
            try
            {
                registry.Register<IDebugService>(foreign);
                Assert.Throws<InvalidOperationException>(() => module.Bootstrap(context));
                module.Shutdown(context);
                Assert.That(registry.Resolve<IDebugService>(), Is.SameAs(foreign));
                registry.Unregister<IDebugService>();
                module.Bootstrap(context); var owned = registry.Resolve<IDebugService>();
                Assert.Throws<InvalidOperationException>(() => module.Bootstrap(context));
                Assert.That(registry.Resolve<IDebugService>(), Is.SameAs(owned));
                registry.Unregister<IDebugService>(); registry.Register<IDebugService>(foreign);
                var otherFacade = new DebugService(); SWJDebug.SetService(otherFacade);
                module.Shutdown(null); module.Shutdown(null);
                Assert.That(registry.Resolve<IDebugService>(), Is.SameAs(foreign));
                Assert.That(SWJDebug.Service, Is.SameAs(otherFacade));
            }
            finally { module.Shutdown(context); SWJDebug.SetService(previous); Object.DestroyImmediate(module); Object.DestroyImmediate(holder); }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RegistrationFailure_RollsBack_AndTheSameAssetCanRetry(bool failAfterRegistration)
        {
            var previous = SWJDebug.Service; SWJDebug.SetService(null);
            var holder = new GameObject("registration failure");
            var module = ScriptableObject.CreateInstance<DebugPackageBootstrapperSo>();
            var registry = new FailingRegistry { FailAfterRegistration = failAfterRegistration };
            var context = new PackageBootstrapContext(holder, null, null, registry);
            try
            {
                Assert.Throws<InvalidOperationException>(() => module.Bootstrap(context));
                Assert.That(SWJDebug.Service, Is.Null); Assert.That(registry.TryResolve<IDebugService>(out _), Is.False);
                registry.Fail = false; module.Bootstrap(context);
                Assert.That(SWJDebug.Service, Is.SameAs(registry.Resolve<IDebugService>()));
                module.Shutdown(null); Assert.That(registry.TryResolve<IDebugService>(out _), Is.False);
                module.Bootstrap(context); module.Shutdown(context);
            }
            finally { module.Shutdown(context); SWJDebug.SetService(previous); Object.DestroyImmediate(module); Object.DestroyImmediate(holder); }
        }

        private sealed class FailingRegistry : SWeetJelly.Bootstrapper.Contract.IServiceRegistry
        {
            private readonly ServiceRegistry inner = new ServiceRegistry();
            public bool Fail = true; public bool FailAfterRegistration;
            public void Register<T>(T service) where T : class
            {
                if (FailAfterRegistration) inner.Register(service);
                if (Fail) throw new InvalidOperationException("registration failed");
                if (!FailAfterRegistration) inner.Register(service);
            }
            public T Resolve<T>() where T : class => inner.Resolve<T>();
            public bool TryResolve<T>(out T service) where T : class => inner.TryResolve(out service);
            public void Unregister<T>() where T : class => inner.Unregister<T>();
        }
    }
}
