namespace SWeetJelly.Debug.Framework.Bootstrapper
{
    using SWeetJelly.Debug.App;
    using SWeetJelly.Debug.Contract;
    using SWeetJelly.Debug.Framework.Unity;
    using SWeetJelly.Bootstrapper.Framework.Unity;
    using SWeetJelly.Bootstrapper.Contract;
    using UnityEngine;
    [CreateAssetMenu(
        fileName = "Debug Package Bootstrapper",
        menuName = "SWeetJelly/Bootstrapper/Packages/Debug")]
    public sealed class DebugPackageBootstrapperSo : PackageBootstrapperSo
    {
        private DebugService service;
        private IServiceRegistry registry;

        public override string PackageName => "Debug";

        public override void Bootstrap(PackageBootstrapContext context)
        {
            if (context == null)
                throw new System.ArgumentNullException(nameof(context));

            if (service != null || context.Services.TryResolve<IDebugService>(out _) || SWJDebug.Service != null)
                throw new System.InvalidOperationException("A debug service or facade is already active.");

            var created = new DebugService(new DebugFormat());
            created.AddDebugger(new UnityConsoleDebugger());
            try
            {
                context.Services.Register<IDebugService>(created);
                SWJDebug.SetService(created);
            }
            catch
            {
                if (ReferenceEquals(SWJDebug.Service, created)) SWJDebug.SetService(null);
                if (context.Services.TryResolve<IDebugService>(out var registered) && ReferenceEquals(registered, created))
                    context.Services.Unregister<IDebugService>();
                created.ClearDebuggers();
                throw;
            }
            service = created;
            registry = context.Services;
        }

        public override void Shutdown(PackageBootstrapContext context)
        {
            if (service == null) return;
            var owned = service;
            if (ReferenceEquals(SWJDebug.Service, owned)) SWJDebug.SetService(null);
            try
            {
                if (registry.TryResolve<IDebugService>(out var registered) && ReferenceEquals(registered, owned))
                    registry.Unregister<IDebugService>();
            }
            finally
            {
                owned.ClearDebuggers();
                service = null;
                registry = null;
            }
        }
    }
}
