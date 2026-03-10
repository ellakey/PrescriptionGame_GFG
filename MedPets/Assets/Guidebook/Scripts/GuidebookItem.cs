using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookItem : MonoBehaviour
{
    public int guidebookID;
    private GuidebookInfo guidebookInfo;

    void Start()
    {
        guidebookInfo = FindObjectOfType<GuidebookInfo>();
        if(guidebookInfo == null)
        {
            Debug.LogError("GuidebookInfo script not found in the scene.");
        }
    }

    public void setGuidebookID()
    {
        if(guidebookInfo == null)
        {
            Debug.LogError("GuidebookInfo script not found in the scene.");
            return;
        }
        
        guidebookInfo.SetGuidebookItem(guidebookID);
        guidebookInfo.ShowGuidebookInfo();
    }
}
