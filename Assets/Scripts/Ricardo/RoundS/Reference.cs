using UnityEngine;
using UnityEngine.UI;

public class Reference : MonoBehaviour
{
    public RawImage imageD;
    //private RoundSystem roundSystem;


    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowImageReference(Texture2D chosenImage)
    {
        imageD.texture = chosenImage;
        imageD.SetNativeSize();
    }

}
