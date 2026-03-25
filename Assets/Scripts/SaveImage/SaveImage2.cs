using UnityEngine;
using System.IO;
using System.Collections;

public class SaveImage2 : MonoBehaviour
{
    public Camera captureCamera; 
    public RenderTexture renderTexture;


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TakeScreenshot();
        }
    }
    public void TakeScreenshot()
    {
        StartCoroutine(CaptureCanvasRoutine());
    }

    private IEnumerator CaptureCanvasRoutine()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = renderTexture;

        Texture2D screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenShot.Apply();

        RenderTexture.active = null;

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Path.Combine(Application.dataPath, "canvas_screenshot.png");
        File.WriteAllBytes(filename, bytes);

        Debug.Log("Screenshot saved to: " + filename);

        Destroy(screenShot);
    }
}
