using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public bool isInventory;
    public GameObject itemPrefab;
    public BerryHolder holder;

    [Header("New Item Popup")]
    public GameObject newItemPopup;
    public TextMeshProUGUI newItemText;
    public TextMeshProUGUI newItemDescription;
    public Image newItemImage;

    private void Start()
    {
        instance = this;
        if(GameState.Instance != null && holder != null) GameState.Instance.InitItems(holder.Size);

        if (isInventory)
        {
            LoadItems();
        }
    }

    public void LoadItems()
    {
        int[] items = GameState.Instance.items;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != 0)
            {
                GameObject current = Instantiate(itemPrefab, gameObject.transform);
                current.GetComponent<ItemController>().text.text += items[i];
                current.GetComponent<ItemController>().button.sprite = holder.GetBerry(i).GetComponent<Image>().sprite;
                current.GetComponent<ItemController>().GetComponent<ConsumableUser>().item = holder.GetBerry(i);
            }
        }
    }

    public static void ShowNewItemPopup(Berry berry)
    {
        if (instance == null) return;
        instance.ShowNewItemPopupInternal(berry.DataName, berry.DataDescription, berry.DataSprite);
    }

    private void ShowNewItemPopupInternal(string name, string description, Sprite image)
    {
        newItemText.text = name;
        newItemDescription.text = description;
        newItemImage.sprite = image;
        newItemPopup.SetActive(true);
    }

    public void CloseNewItemPopup()
    {
        newItemPopup.SetActive(false);
    }
}