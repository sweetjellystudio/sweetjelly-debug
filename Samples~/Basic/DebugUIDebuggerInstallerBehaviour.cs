namespace SWeetJelly.Samples.Debug
{
    using System;
    using SWeetJelly.Debug.App;
    using SWeetJelly.Debug.Contract;
    using SWeetJelly.Debug.Framework.Unity;
    using SWeetJelly.Bootstrapper.Framework.Unity;

    using UnityEngine;
    using UnityEngine.UI;

    public sealed class DebugUIDebuggerInstallerBehaviour : MonoBehaviour
    {
        [SerializeField] private Text output;
        [SerializeField] private int maxLines = 12;

        private IDebugService debugService;
        private UnityConsoleDebugger consoleDebugger;
        private UnityUIDebugger uiDebugger;
        private string debuggerMode = "Console Debugger (default)";

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
            if (debugService != null || !BootstrapperBehaviour.IsBootstrapped || BootstrapperBehaviour.Services == null)
                return;

            if (!BootstrapperBehaviour.Services.TryResolve<IDebugService>(out debugService))
                return;

            consoleDebugger = new UnityConsoleDebugger();
        }

        private void OnGUI()
        {
            const float x = 190f;
            var y = 48f;

            GUI.Label(new Rect(x, y, 280f, 24f), $"Debugger Output: {debuggerMode}");
            y += 32f;

            GUI.enabled = debugService != null;

            if (GUI.Button(new Rect(x, y, 180f, 28f), "Use Console Debugger"))
                UseConsoleDebugger();

            y += 34f;

            if (GUI.Button(new Rect(x, y, 180f, 28f), "Use UI Debugger"))
                UseUIDebugger();

            GUI.enabled = true;
        }

        private void UseConsoleDebugger()
        {
            if (debugService == null)
                return;

            consoleDebugger = consoleDebugger ?? new UnityConsoleDebugger();

            debugService.ClearDebuggers();
            debugService.AddDebugger(consoleDebugger);

            debuggerMode = "Console Debugger";
            SWJDebug.Log("Debug Sample", "Switched to UnityConsoleDebugger.");
        }

        private void UseUIDebugger()
        {
            if (debugService == null)
                return;

            output = output != null ? output : CreateOutputText();
            uiDebugger = uiDebugger != null ? uiDebugger : gameObject.AddComponent<UnityUIDebugger>();
            uiDebugger.SetOutput(output);

            debugService.ClearDebuggers();
            debugService.AddDebugger(uiDebugger);

            debuggerMode = "UI Debugger";
            SWJDebug.Log("Debug Sample", "Switched to UnityUIDebugger.");
        }

        private Text CreateOutputText()
        {
            if (maxLines <= 0)
                maxLines = 1;

            var canvas = BootstrapperBehaviour.Canvas != null
                ? BootstrapperBehaviour.Canvas
                : CreateCanvas();

            var outputObject = new GameObject("Debug UI Output");
            outputObject.transform.SetParent(canvas.transform, false);

            var rect = outputObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-16f, 16f);
            rect.sizeDelta = new Vector2(520f, maxLines * 22f);

            var text = outputObject.AddComponent<Text>();
            text.alignment = TextAnchor.LowerRight;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = Color.white;

            return text;
        }

        private Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Debug UI Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }
    }
}
