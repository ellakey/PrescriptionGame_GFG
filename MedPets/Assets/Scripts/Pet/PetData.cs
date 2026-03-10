using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetData
{
    public int food;
    public int blood;
    public int energy;

    public bool playedOnce;

    public int[] items;


    public PetData(){
        /*food = NeedsController.food;
        blood = NeedsController.blood;
        energy = NeedsController.energy;
        playedOnce = NeedsController.playedOnce;
        for(int i = 0; i < items.Length; i++)
        {
            items[i] = Inventory.items[i];
        }*/
    }
}
