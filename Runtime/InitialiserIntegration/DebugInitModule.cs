namespace SWeetJelly.Debug.InitialiserIntegration
{
    using SWeetJelly.Debug.App;
    using SWeetJelly.Debug.Contract;
    using SWeetJelly.Debug.Framework;
    using SWeetJelly.Initialiser.Contract;
    using SWeetJelly.Initialiser.Framework;
    using UnityEngine;

    [RegisterModule("Debug")]
    [CreateAssetMenu(
        fileName = "Debug Init Module",
        menuName = "SWeetJelly/Initialiser/Modules/Debug")]
    public sealed class DebugInitModule : InitModuleSo
    {
        private DebugService service;

        public override string ModuleName => "Debug";

        public override void Install(InitModuleInstallContext context)
        {
            if (context == null)
                throw new System.ArgumentNullException(nameof(context));

            service = new DebugService(new DebugFormat());
            service.AddDebugger(new UnityConsoleDebugger());

            SWJDebug.SetService(service);
            context.Services.Register<IDebugService>(service);
        }

        public override void Uninstall(InitModuleInstallContext context)
        {
            SWJDebug.SetService(null);

            if (context != null)
                context.Services.Unregister<IDebugService>();

            service?.ClearDebuggers();
            service = null;
        }
    }
}
