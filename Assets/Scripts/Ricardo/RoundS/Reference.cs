using UnityEngine;
using UnityEngine.UI;

public class Reference : MonoBehaviour
{
    public RawImage imageD;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowImageReference(Texture2D chosenImage)
    {
        imageD.texture = chosenImage;
    }

    public void HideImageReference()
    {
        gameObject.SetActive(false);
    }

}
