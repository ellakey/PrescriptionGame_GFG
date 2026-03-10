using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextChanger : MonoBehaviour
{
    public TextMeshProUGUI textPanel;
    public TextMeshProUGUI textPanel2;
    public string text;
    public string text2;
    public bool hasMultiple;


    void Start()
    {
        textPanel.text = text;
        if (hasMultiple)
        {
            textPanel2.text = text2;
        }
    }

    public void ReEvaluate()
    {
        textPanel.text = text;
        if (hasMultiple)
        {
            textPanel2.text = text2;
        }
    }
}
