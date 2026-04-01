using UnityEngine;
using UnityEngine.UI;

public class Erase : MonoBehaviour
{
    public Camera cam;
    public GameObject brush;

    public LineRenderer currentLineRenderer;

    public Slider scaleSlider;

    private int orderInLayer = 0;
    private Transform parentTransform;

    Vector2 lastPos;

    private void Start()
    {
        parentTransform = transform;
    }
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
        orderInLayer++;
        GameObject brushInstance = Instantiate(brush, parentTransform);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        currentLineRenderer.sortingOrder = orderInLayer;

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
