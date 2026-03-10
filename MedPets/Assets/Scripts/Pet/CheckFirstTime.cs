using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFirstTime : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tutorialBox;

    void Start()
    {
        if(NeedsController.playedOnce == false)
        {
            NeedsController.food = 75;
            NeedsController.blood = 200;
            NeedsController.energy = 0;
            tutorialBox.SetActive(true);
        }
        NeedsController.playedOnce = true;
        SaveSystem.SavePet();
    }
}
