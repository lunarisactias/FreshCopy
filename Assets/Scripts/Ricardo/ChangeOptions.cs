using System;
using UnityEditor;
using UnityEngine;

public class ChangeOptions : MonoBehaviour
{
    [SerializeField] private GameObject brushObj;
    [SerializeField] private GameObject eraseObj;
    [SerializeField] private GameObject highlightObj;

    

    public void ChangeColor(GameObject newBrush)
    {
        var brushScript = brushObj.GetComponent<Draw>();
        var eraseScript = eraseObj.GetComponent<Erase>();
        var highlighterScript = highlightObj.GetComponent<Highlight>();

        
        if(brushScript.enabled)
        {
            brushScript.brush = newBrush;
        }
        else if(highlighterScript.enabled)
        {
            highlighterScript.brush = newBrush;
        }
        
    }

    public void ChooseBrush(GameObject brushObj)
    {
        brushObj.GetComponent<Draw>().enabled = true;
        eraseObj.GetComponent<Erase>().enabled = false;
        highlightObj.GetComponent<Highlight>().enabled = false;
    }

    public void ChooseEraser(GameObject eraseObj)
    {
        brushObj.GetComponent<Draw>().enabled = false;
        eraseObj.GetComponent<Erase>().enabled = true;
        highlightObj.GetComponent<Highlight>().enabled = false;
    }

    public void ChooseHighlighter(GameObject highlightObj)
    {
        brushObj.GetComponent<Draw>().enabled = false;
        eraseObj.GetComponent<Erase>().enabled = false;
        highlightObj.GetComponent<Highlight>().enabled = true;
    }
}
