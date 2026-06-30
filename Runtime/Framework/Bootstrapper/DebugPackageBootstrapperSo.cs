namespace SWeetJelly.Debug.Framework.Bootstrapper
{
    using SWeetJelly.Debug.App;
    using SWeetJelly.Debug.Contract;
    using SWeetJelly.Debug.Framework.Unity;
    using SWeetJelly.Bootstrapper.Framework.Unity;
    using UnityEngine;
    [CreateAssetMenu(
        fileName = "Debug Package Bootstrapper",
        menuName = "SWeetJelly/Bootstrapper/Packages/Debug")]
    public sealed class DebugPackageBootstrapperSo : PackageBootstrapperSo
    {
        private DebugService service;

        public override string PackageName => "Debug";

        public override void Bootstrap(PackageBootstrapContext context)
        {
            if (context == null)
                throw new System.ArgumentNullException(nameof(context));

            service = new DebugService(new DebugFormat());
            service.AddDebugger(new UnityConsoleDebugger());

            SWJDebug.SetService(service);
            context.Services.Register<IDebugService>(service);
        }

        public override void Shutdown(PackageBootstrapContext context)
        {
            SWJDebug.SetService(null);

            if (context != null)
                context.Services.Unregister<IDebugService>();

            service?.ClearDebuggers();
            service = null;
        }
    }
}
