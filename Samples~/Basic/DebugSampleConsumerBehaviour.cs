namespace SWeetJelly.Samples.Debug
{
    using System;
    using SWeetJelly.Debug.App;
    using SWeetJelly.Debug.Contract;
    using SWeetJelly.Bootstrapper.Framework.Unity;
    using UnityEngine;

    public sealed class DebugSampleConsumerBehaviour : MonoBehaviour
    {
        private IDebugService debugService;
        private string status = "Waiting for Debug service...";
        private int logCount;

        private void OnEnable()
        {
            BootstrapperBehaviour.Bootstrapped += OnBootstrapped;
            ResolveIfReady();
        }

        private void OnDisable()
        {
            BootstrapperBehaviour.Bootstrapped -= OnBootstrapped;
        }

        private void OnBootstrapped(object sender, EventArgs args)
        {
            ResolveIfReady();
        }

        private void ResolveIfReady()
        {
            if (!BootstrapperBehaviour.IsBootstrapped || BootstrapperBehaviour.Services == null)
                return;

            if (!BootstrapperBehaviour.Services.TryResolve<IDebugService>(out debugService))
            {
                status = "IDebugService is not registered.";
                UnityEngine.Debug.LogWarning(status, this);
                return;
            }

            debugService.MinimumLevel = DebugLevel.Trace;
            status = "Debug service is ready.";
            SWJDebug.Log("Debug Sample", status);
            SWJDebug.TraceLog("Initial trace from sample.");
        }

        private void OnGUI()
        {
            const float x = 16f;
            var y = 16f;

            GUI.Label(new Rect(x, y, 520f, 24f), status);
            y += 32f;

            GUI.enabled = debugService != null;

            if (GUI.Button(new Rect(x, y, 160f, 28f), "Log Info"))
                SWJDebug.Log("Sample Info", NextValue());

            y += 34f;

            if (GUI.Button(new Rect(x, y, 160f, 28f), "Log Warning"))
                SWJDebug.LogWarning("Sample Warning", NextValue());

            y += 34f;

            if (GUI.Button(new Rect(x, y, 160f, 28f), "Log Error"))
                SWJDebug.LogError("Sample Error", NextValue());

            y += 34f;

            if (GUI.Button(new Rect(x, y, 160f, 28f), "Trace Log"))
                SWJDebug.TraceLog(NextValue());

            y += 34f;

            if (GUI.Button(new Rect(x, y, 160f, 28f), "Log Exception"))
                SWJDebug.LogException("Sample Exception", new InvalidOperationException(NextValue()));

            y += 42f;

            var enabledLabel = debugService != null && debugService.Enabled
                ? "Disable Debug"
                : "Enable Debug";

            if (GUI.Button(new Rect(x, y, 160f, 28f), enabledLabel))
            {
                debugService.Enabled = !debugService.Enabled;
                status = $"Debug enabled: {debugService.Enabled}";
            }

            GUI.enabled = true;
        }

        private string NextValue()
        {
            logCount++;
            return $"Sample log #{logCount}";
        }
    }
}
