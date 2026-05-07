using System.IO;
using TMPro;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    public int id = 0;
    public Texture2D playerDrawing;
    public TextMeshProUGUI textoComparacao;

    [Networked] public int Score { get; set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GetComponentInChildren<SaveImage2>().TakeScreenshot();
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetDrawing(Texture2D drawing)
    {
        if (playerDrawing != null)
        {
            Destroy(playerDrawing);
        }

        playerDrawing = drawing;
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
        Score += points;
        Debug.Log($"Player {id} scored {points} points! Total: {Score}");
        textoComparacao.text = points.ToString();
    }
}
