using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScreenshotHandler : MonoBehaviour
{
    public Button screenshotButton; // Assign the UI Button in Inspector
    public RawImage screenshotDisplay; // Assign the RawImage in Inspector

    private void Start()
    {
        if (screenshotButton != null)
        {
            screenshotButton.onClick.AddListener(TakeScreenshot);
        }

        // Hide the screenshot preview at start
        if (screenshotDisplay != null)
        {
            screenshotDisplay.gameObject.SetActive(false);
        }
    }

    public void TakeScreenshot()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();

        // Define file path
        string filename = "AR_Screenshot.png";
        string path = Path.Combine(Application.persistentDataPath, filename);
        ScreenCapture.CaptureScreenshot(filename);
        
        Debug.Log("Screenshot saved at: " + path);

        // Wait for file to be saved
        yield return new WaitForSeconds(0.5f);

        // Load the saved screenshot as a Texture
        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            // Assign texture to RawImage and show it
            screenshotDisplay.texture = texture;
            screenshotDisplay.gameObject.SetActive(true); // Show the preview
        }
    }
}
