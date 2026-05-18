using UnityEngine;

public class testeTamanhoCam : MonoBehaviour
{
    public Camera cam;
    void Start()
    {

        gameObject.GetComponent<Camera>();

        Debug.Log(cam.pixelWidth);
        Debug.Log(cam.pixelHeight);
    }

    void Update()
    {
        
    }
}
