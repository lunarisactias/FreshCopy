using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SaveImage : MonoBehaviour
{
    public RectTransform targetRectTransform;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Caller();
        }
    }

    public void Caller()
    {
        String imagePath = Application.dataPath + "/image.png";
        StartCoroutine(captureScreenshot(imagePath));
        Debug.Log("imagePath=" + imagePath);
    }

    IEnumerator captureScreenshot(String imagePath)
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);

        screenImage.ReadPixels(new Rect(0, 0, targetRectTransform.rect.width, targetRectTransform.rect.height - 100), 0, 0);
        screenImage.Apply();

        Debug.Log(" screenImage.width" + screenImage.width + " texelSize" + screenImage.texelSize);
       
        byte[] imageBytes = screenImage.EncodeToPNG();

        Debug.Log("imagesBytes=" + imageBytes.Length);

        //Save image to file
        System.IO.File.WriteAllBytes(imagePath, imageBytes);
    }
}
