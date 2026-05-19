using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

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
        RenderTexture.active = renderTexture;

        Texture2D screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenShot.Apply();

        RenderTexture.active = null;

        Player me = GetComponentInParent<Player>();
        me.SetDrawing(screenShot);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Path.Combine(Application.persistentDataPath, $"drawing_screenshot.png");
        File.WriteAllBytes(filename, bytes);

        Debug.Log("Screenshot salva para visualização em: " + filename);

        yield return null;
    }
}