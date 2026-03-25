using UnityEngine;

public class Player : MonoBehaviour
{
    public int id = 0;
    private int score = 0;
    public Texture2D playerDrawing;

    public void SetDrawing(Texture2D drawing)
    {
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
        score += points;
        Debug.Log($"Player {id} scored {points} points! Total: {score}");
    }
}
