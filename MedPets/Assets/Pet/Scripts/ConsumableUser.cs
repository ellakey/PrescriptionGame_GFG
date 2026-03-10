using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableUser : MonoBehaviour
{
    public GameObject item;
    //public AudioSource audio;
    public Animator petAnim;

    private void Start()
    {
        petAnim = GameObject.FindGameObjectWithTag("Pet").GetComponent<Animator>();
    }

    public void useItem()
    {
        if(!item.GetComponent<Berry>().data.hasBeenUsed)
        {
            // has never been used before
            // popup here
            item.GetComponent<Berry>().data.Use();
        }

        petAnim.SetTrigger("Eating");
        //audio.Play();
        NeedsController.ChangeFood((int)item.GetComponent<Berry>().foodAmount);
        NeedsController.ChangeBlood((int)item.GetComponent<Berry>().bloodAmount);
        NeedsController.ChangeEnergy((int)item.GetComponent<Berry>().energyAmount);
        Inventory.items[item.GetComponent<Berry>().getId()]--;
        if (Inventory.items[item.GetComponent<Berry>().getId()] != 0)
        {
            gameObject.GetComponent<ItemController>().text.text = "x" + Inventory.items[item.GetComponent<Berry>().getId()];
        }
        else
        {
            Destroy(gameObject);
        }
        SaveSystem.SavePet();
    }

}
