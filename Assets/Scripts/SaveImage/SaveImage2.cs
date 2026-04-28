using UnityEngine;
using System.IO;
using System.Collections;
using UnityEditor;

public class SaveImage2 : MonoBehaviour
{
    public Camera captureCamera; 
    public RenderTexture renderTexture;

    private int playerID;


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
        playerID = GetComponentInParent<Player>().GetID();
        string filename = Path.Combine(Application.dataPath, $"Resources/drawing_screenshot_{playerID}.png");
        File.WriteAllBytes(filename, bytes);

        Texture2D playerDrawing = Resources.Load<Texture2D>($"drawing_screenshot_{playerID}");

        GetComponentInParent<Player>().SetDrawing(playerDrawing);

        //AssetDatabase.Refresh();

        Debug.Log("Screenshot saved to: " + filename);

        Destroy(screenShot);
    }
}
