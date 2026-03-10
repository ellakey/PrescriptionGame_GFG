using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitch : MonoBehaviour
{
    public GameObject deselect;
    public GuidebookItem item;
    public Image image;

    private void Update()
    {
        if(image.color.a != 1f)
        {
            deselect.SetActive(false);
        }
        else
        {
            deselect.SetActive(true);
        }
    }

    public void Select()
    {
        PatientInfo.addMedication(item.guidebookID);
        Debug.Log("Selected");
    }
}
