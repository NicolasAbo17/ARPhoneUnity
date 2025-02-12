using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

public class ScreenshotHandler : MonoBehaviour
{
    public GameObject UIParent;           // Hide this before taking the screenshot
    public RawImage screenshotDisplay;    // Show the screenshot result

    private void Start()
    {
        // Hide screenshot preview at start
        if (screenshotDisplay != null)
        {
            screenshotDisplay.gameObject.SetActive(false);
        }

        // Request permission on Android if necessary
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
    }

    public void TakeScreenshot()
    {
        Debug.Log("Screenshot button clicked!");
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        // Hide UI so it's not in the screenshot
        if (UIParent != null) UIParent.SetActive(false);

        yield return new WaitForEndOfFrame();

        string filename = "AR_Screenshot.png";
        string internalPath = Path.Combine(Application.persistentDataPath, filename);

        ScreenCapture.CaptureScreenshot(filename);

        // Wait briefly for the file to finalize
        yield return new WaitForSeconds(0.5f);

        // Show UI again
        if (UIParent != null) UIParent.SetActive(true);

        // Load the screenshot from internal path
        if (File.Exists(internalPath))
        {
            byte[] imageBytes = File.ReadAllBytes(internalPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            // Display it in the RawImage
            if (screenshotDisplay != null)
            {
                screenshotDisplay.texture = texture;
                screenshotDisplay.gameObject.SetActive(true);
            }

            Debug.Log("Screenshot saved internally at: " + internalPath);

#if UNITY_ANDROID && !UNITY_EDITOR
            // Copy to external storage so it appears in Gallery
            string externalDir = "/storage/emulated/0/DCIM/MyARAppScreenshots";
            if (!Directory.Exists(externalDir))
            {
                Directory.CreateDirectory(externalDir);
            }

            string externalPath = Path.Combine(externalDir, filename);
            File.Copy(internalPath, externalPath, true);

            Debug.Log("Copied to external path: " + externalPath);

            // Trigger a media scan so it shows in Gallery
            using (AndroidJavaClass mediaScannerClass = new AndroidJavaClass("android.media.MediaScannerConnection"))
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                mediaScannerClass.CallStatic(
                    "scanFile",
                    context,
                    new string[] { externalPath },
                    null,
                    null
                );
            }
#endif
        }
        else
        {
            Debug.LogWarning("Screenshot file not found at: " + internalPath);
        }
    }
}
