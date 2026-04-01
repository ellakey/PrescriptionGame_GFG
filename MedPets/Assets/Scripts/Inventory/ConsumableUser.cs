using System;
using UnityEngine;

public class ConsumableUser : MonoBehaviour
{
    public GameObject item;
    public Animator petAnim;

    public static event Action<Berry> OnItemUsed;

    private void Start()
    {
        petAnim = GameObject.FindGameObjectWithTag("Pet").GetComponent<Animator>();
    }

    public void UseItem()
    {
        Debug.Log("Using item: " + item.name);
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

        OnItemUsed?.Invoke(berry);
    }
}