using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuidebookInfo : MonoBehaviour
{
    public static int guidebookID;
    public GameObject infoPanel;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI desc;
    public Image image;

    public void SetGuidebookItem(int id)
    {
        guidebookID = id;
        Berry item = Inventory.instance.holder.GetBerry(guidebookID).GetComponent<Berry>();
        itemName.text = item.DataName;
        desc.text = item.DataDescription;
        image.sprite = item.DataSprite;
    }

    public void ShowGuidebookInfo()
    {
        infoPanel.SetActive(true);
    }

    public void CloseGuidebookInfo()
    {
        infoPanel.SetActive(false);
    }
}
