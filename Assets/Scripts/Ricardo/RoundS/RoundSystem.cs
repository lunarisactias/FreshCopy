using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundSystem : MonoBehaviour
{
    [Header("CHOSEN IMAGES")]
    [SerializeField] private Texture2D[] imgs;


    [Header("TIMER")]
    [SerializeField] private float roundTimer;
    [SerializeField] private TextMeshProUGUI tmp;
    public bool activateTimer;


    [Header("SCRIPTS")]
    [SerializeField] private SentisComparer imgComparer;
    [SerializeField] private Reference refer;

    private void Awake()
    {
        imgComparer = GameObject.Find("SentisTest").GetComponent<SentisComparer>();
        refer = GameObject.Find("Reference").GetComponent<Reference>();
        roundTimer = 5.9f;
        activateTimer = true;
    }

    void Update()
    {
        RoundCountdown();

        if (activateTimer)
        {
            roundTimer -= Time.deltaTime;
            if (roundTimer <= 0)
            {
                ChooseImage();
                roundTimer = 10;
                activateTimer = false;
            }
        }

    }

    void ChooseImage()
    {
        int chooseRandomImage = Random.Range(0, imgs.Length);
        Texture2D chosenImage = imgs[chooseRandomImage];
        imgComparer.originalDrawing = chosenImage;

        refer.gameObject.SetActive(true);

        refer.ShowImageReference(chosenImage);
    }

    void RoundCountdown()
    {
        int secInt = (int)roundTimer;
        tmp.text = secInt.ToString();


    }
}
