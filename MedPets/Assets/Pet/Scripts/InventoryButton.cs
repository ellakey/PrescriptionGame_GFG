using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public Animator anim;

    private Inventory inventory;

    private void Start()
    {
        inventory = Inventory.instance;
    }

    public void scrollOut()
    {
        if (inventory.newItemPopup.activeSelf)
        {
            inventory.CloseNewItemPopup();
        }
        else
        {
            anim.SetBool("isIn", false);
        }
    }

    public void scrollIn()
    {
        anim.SetBool("isIn", true);
    }
}
