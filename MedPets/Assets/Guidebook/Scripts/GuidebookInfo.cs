using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuidebookInfo : MonoBehaviour
{
    // public static int guidebookID;
    // public TextMeshProUGUI itemName;
    // public TextMeshProUGUI desc;
    // public TextMeshProUGUI desc2;
    // public Image image;
    // public BerryHolder holder;

    // private void Start()
    // {
    //     Berry item = holder.getBerry(guidebookID).GetComponent<Berry>();
    //     itemName.text = item.getName();
    //     desc.text = item.getDescription();
    //     desc2.text = item.getDescription2();
    //     image.sprite = holder.getBerry(guidebookID).GetComponent<Image>().sprite;
    // }

    public static int guidebookID;
    public GameObject infoPanel;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI desc;
    public Image image;

    public void SetGuidebookItem(int id)
    {
        guidebookID = id;
        Berry item = Inventory.instance.holder.getBerry(guidebookID).GetComponent<Berry>();
        itemName.text = item.getDataName();
        desc.text = item.getDataDescription();
        image.sprite = item.getDataSprite();
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
