using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-999)]
public class DebugConsole : MonoBehaviour
{
    private Text consoleText;
    private string logCache = "";
    private Canvas consoleCanvas;
    private const int fontSize = 14;

    private void Awake()
    {
        // Create a new Canvas at runtime
        GameObject canvasGO = new GameObject("DebugConsoleCanvas");
        consoleCanvas = canvasGO.AddComponent<Canvas>();
        consoleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Remove or don't add a GraphicRaycaster so it won't block button clicks
        // canvasGO.AddComponent<GraphicRaycaster>(); // NOT added

        // Optionally scale with screen size
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // Keep console alive across scene loads if desired
        DontDestroyOnLoad(canvasGO);

        // Create a Text child
        GameObject textGO = new GameObject("DebugConsoleText");
        textGO.transform.SetParent(canvasGO.transform, false);

        consoleText = textGO.AddComponent<Text>();
        consoleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        consoleText.fontSize = fontSize;
        consoleText.color = new Color(1f, 1f, 1f, 1f);
        consoleText.alignment = TextAnchor.UpperLeft;
        consoleText.horizontalOverflow = HorizontalWrapMode.Wrap;
        consoleText.verticalOverflow = VerticalWrapMode.Overflow;
        // Disable raycast target so it won't block clicks
        consoleText.raycastTarget = false;

        // Position & size
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(10f, -10f);
        rt.sizeDelta = new Vector2(Screen.width * 0.9f, Screen.height);

        // Optional background: If you want a semi-transparent overlay, put it on a separate child
        // so that background also doesn't block UI. For example:
        // var bgGO = new GameObject("DebugConsoleBackground");
        // bgGO.transform.SetParent(canvasGO.transform, false);
        // var bgImage = bgGO.AddComponent<Image>();
        // bgImage.color = new Color(0f, 0f, 0f, 0.5f);
        // bgImage.raycastTarget = false;
        // (Then position/size it similarly, behind the text.)

        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        logCache += logString + "\n";
        consoleText.text = logCache;
    }
}
