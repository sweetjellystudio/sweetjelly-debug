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
        private GameObject ownedOutput;
        private Canvas ownedCanvas;
        private bool useUI;
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
            Detach();
        }

        private void OnBootstrapped(object sender, EventArgs args)
        {
            ResolveIfReady();
        }

        private void ResolveIfReady()
        {
            if (!BootstrapperBehaviour.IsBootstrapped || BootstrapperBehaviour.Services == null)
                return;

            if (!BootstrapperBehaviour.Services.TryResolve<IDebugService>(out var resolved))
                return;

            if (ReferenceEquals(debugService, resolved)) return;
            Detach();
            debugService = resolved;
            if (useUI) AttachUI();
        }

        private void OnGUI()
        {
            const float x = 190f;
            var y = 48f;

            GUI.Label(new Rect(x, y, 280f, 24f), $"Debugger Output: {debuggerMode}");
            y += 32f;

            GUI.enabled = debugService != null;

            if (GUI.Button(new Rect(x, y, 180f, 28f), "Remove UI Output"))
                UseConsoleDebugger();

            y += 34f;

            if (GUI.Button(new Rect(x, y, 180f, 28f), "Add UI Output"))
                UseUIDebugger();

            GUI.enabled = true;
        }

        private void UseConsoleDebugger()
        {
            useUI = false;
            RemoveUI();
            debuggerMode = "Console Debugger";
        }

        private void UseUIDebugger()
        {
            if (debugService == null) return;
            useUI = true;
            AttachUI();
            debugService.Log(DebugLevel.Info, DebugLabelType.Grouping, "Debug Sample", "Added UI output; Console is retained.");
        }

        private void AttachUI()
        {
            if (uiDebugger != null)
            {
                uiDebugger.SetMaxLines(maxLines);
                return;
            }
            output = output != null ? output : CreateOutputText();
            uiDebugger = gameObject.AddComponent<UnityUIDebugger>();
            uiDebugger.SetOutput(output);
            uiDebugger.SetMaxLines(maxLines);
            debugService.AddDebugger(uiDebugger);
            debuggerMode = "Console + UI Debugger";
        }

        private void RemoveUI()
        {
            if (uiDebugger != null)
            {
                debugService?.RemoveDebugger(uiDebugger);
                uiDebugger.Clear();
                Destroy(uiDebugger);
                uiDebugger = null;
            }
            if (ownedOutput != null)
            {
                Destroy(ownedOutput);
                ownedOutput = null;
                output = null;
            }
            if (ownedCanvas != null)
            {
                Destroy(ownedCanvas.gameObject);
                ownedCanvas = null;
            }
        }

        private void Detach()
        {
            RemoveUI();
            debugService = null;
        }

        private Text CreateOutputText()
        {
            if (maxLines <= 0)
                maxLines = 1;

            var canvas = BootstrapperBehaviour.Canvas != null
                ? BootstrapperBehaviour.Canvas
                : CreateCanvas();

            var outputObject = new GameObject("Debug UI Output");
            ownedOutput = outputObject;
            outputObject.transform.SetParent(canvas.transform, false);

            var rect = outputObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-16f, 16f);
            rect.sizeDelta = new Vector2(520f, maxLines * 22f);

            var text = outputObject.AddComponent<Text>();
            text.alignment = TextAnchor.LowerRight;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = Color.white;
            text.raycastTarget = false;

            return text;
        }

        private Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Debug UI Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            ownedCanvas = canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }
    }
}
