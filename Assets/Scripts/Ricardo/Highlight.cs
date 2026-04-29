using UnityEngine;
using UnityEngine.UI;

public class Highlight : MonoBehaviour
{
    public Camera cam;
    public GameObject brush;

    public LineRenderer currentLineRenderer;

    public Slider scaleSlider;

    private int orderInLayer = 1;
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
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            MouseDrawing();
        }
        else
        {
            MobileDrawing();
        }
    }

    void MobileDrawing()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                CreateBrush();
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchPos = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                if (touchPos != lastPos)
                {
                    AddAPoint(touchPos);
                    lastPos = touchPos;
                }
                AddAPoint(touchPos);
            }

        }
        else
        {
            currentLineRenderer = null;
        }
    }
    void MouseDrawing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateBrush();
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos != lastPos)
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
        currentLineRenderer.sortingOrder = orderInLayer - 1;

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
        float value = scaleSlider.value + 0.25f;
        currentLineRenderer.startWidth = value;
        currentLineRenderer.endWidth = value;
    }
}
