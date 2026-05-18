using System.IO;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id = 0;
    private int score = 0;
    public Texture2D playerDrawing;
    public TextMeshProUGUI textoComparacao;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GetComponentInChildren<SaveImage2>().TakeScreenshot();
        }
    }

    public void SetDrawing(Texture2D drawing)
    {
        playerDrawing = Path.Combine(Application.dataPath, $"drawing_screenshot_{id}.png") != null ? drawing : null;
    }

    public int GetID() 
    { 
        return id; 
    }

    public Texture2D GetDrawing()
    {
        return playerDrawing;
    }

    public void AddScore(float similarity)
    {
        int points = Mathf.RoundToInt(similarity * 100);
        score += points;
        Debug.Log($"Player {id} scored {points} points! Total: {score}");
        textoComparacao.text = points.ToString();
    }
}
