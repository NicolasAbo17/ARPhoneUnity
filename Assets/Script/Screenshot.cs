using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScreenshotHandler : MonoBehaviour
{
    public GameObject UIParent;
    public RawImage screenshotDisplay; // Assign the RawImage in Inspector

    private void Start()
    {
        // Hide the screenshot preview at start
        if (screenshotDisplay != null)
        {
            screenshotDisplay.gameObject.SetActive(false);
        }
    }

    public void TakeScreenshot()
    {
        Debug.Log("Screenshot button clicked!");
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {     
        // Hide UI Elements
        UIParent.SetActive(false);

        yield return new WaitForEndOfFrame();

        // Define file path
        string filename = "AR_Screenshot.png";
        string path = Path.Combine(Application.persistentDataPath, filename);
        ScreenCapture.CaptureScreenshot(filename);

        Debug.Log("Screenshot saved at: " + path);

        // Wait for file to be saved
        yield return new WaitForSeconds(0.5f);

        // Restore the UI
        UIParent.SetActive(true);

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