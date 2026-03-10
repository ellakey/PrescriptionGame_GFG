using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFirstTime : MonoBehaviour
{
    public GameObject tutorialBox;

    void Start()
    {
        GameState gs = GameState.Instance;
        if (gs.playedOnce == false)
        {
            gs.food = 75;
            gs.blood = 200;
            gs.energy = 0;
            tutorialBox.SetActive(true);
        }
        gs.playedOnce = true;
        SaveSystem.SavePet();
    }
}