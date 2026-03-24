using UnityEngine;
using UnityEngine.UI;

public class Draw : MonoBehaviour
{
    public Camera cam;
    public GameObject brush;

    LineRenderer currentLineRenderer;

    public Slider scaleSlider;

    Vector2 lastPos;

    private void Update()
    {
        DrawMethod();
    }
    void DrawMethod()
    {
        if(Input.GetMouseButtonDown(0))
        {
            CreateBrush();
        }
        if(Input.GetMouseButton(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            if(mousePos != lastPos)
            {
                AddAPoint(mousePos);
                lastPos = mousePos;
            }
            AddAPoint(mousePos);
        }
        else
        {
            currentLineRenderer = null;
        }
    }

    void CreateBrush()
    {
        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        ChangeSize();

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        currentLineRenderer.SetPosition(0, mousePos);
        currentLineRenderer.SetPosition(1, mousePos);
    }

    void AddAPoint(Vector2 pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }

    void ChangeSize()
    {
        float value = scaleSlider.value;
        currentLineRenderer.startWidth = value;
        currentLineRenderer.endWidth = value;
    }
}
