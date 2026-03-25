using UnityEngine;

public class ChangeBrush : MonoBehaviour
{
    public Draw draw;

    public void ChangeColor(GameObject newBrush)
    {
        draw.brush = newBrush;
    }
}
