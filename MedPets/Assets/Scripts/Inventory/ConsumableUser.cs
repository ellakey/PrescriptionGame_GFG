using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableUser : MonoBehaviour
{
    public GameObject item;
    public Animator petAnim;

    private void Start()
    {
        petAnim = GameObject.FindGameObjectWithTag("Pet").GetComponent<Animator>();
    }

    public void UseItem()
    {
        Berry berry = item.GetComponent<Berry>();
        GameState gs = GameState.Instance;

        if (!berry.data.hasBeenUsed)
        {
            Inventory.ShowNewItemPopup(berry);
            berry.data.Use();
        }

        petAnim.SetTrigger("Eating");
        gs.ChangeFood((int)berry.foodAmount);
        gs.ChangeBlood((int)berry.bloodAmount);
        gs.ChangeEnergy((int)berry.energyAmount);

        gs.items[berry.Id]--;
        if (gs.items[berry.Id] != 0)
        {
            gameObject.GetComponent<ItemController>().text.text = "x" + gs.items[berry.Id];
        }
        else
        {
            Destroy(gameObject);
        }
        SaveSystem.SavePet();
    }
}