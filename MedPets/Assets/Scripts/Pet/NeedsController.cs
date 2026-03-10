using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedsController : MonoBehaviour
{
    public static float food;
    public static float blood;
    public static float energy;
    public static bool playedOnce;

    public PetUIController foodBar;
    public PetUIController bloodBar;
    public PetUIController energyBar;

    public PetController controller;
    public GameObject playGlow;
    public GameObject energyMeterGlow;
    public GameObject foodMeterGlow;
    public GameObject bloodMeterGlow;

    private void Start()
    {
        playedOnce = true;
        SaveSystem.SavePet();
    }

    public void Update()
    {
        foodBar.UpdateFoodBar(100, food);
        bloodBar.UpdateBloodBar(blood);
        energyBar.UpdateEnergyBar(100, energy);

        if (TimingManager.gameHourTimer < 0)
        {
            //PetUIController.instance.UpdateImages(food);
        }

        if (food <= 20)
        {
            controller.Hungry();
            if (foodMeterGlow != null)
            {
                foodMeterGlow.SetActive(true);
            }
        }
        else
        {
            if (foodMeterGlow != null)
            {
                foodMeterGlow.SetActive(false);
            }
        }

        if(food < 0)
        {
            food = 0;
        }

        if(blood < 80)
        {
            controller.LowBloodSugar();
            if(bloodMeterGlow != null)
            {
                bloodMeterGlow.SetActive(true);
            }
        }
        else if(blood > 140)
        {
            controller.HighBloodSugar();
            if (bloodMeterGlow != null)
            {
                bloodMeterGlow.SetActive(true);
            }
        }
        else
        {
            if (bloodMeterGlow != null)
            {
                bloodMeterGlow.SetActive(false);
            }
        }

        if(energy <= 40)
        {
            controller.Bored();
            if(playGlow != null)
            {
                playGlow.SetActive(true);
            }
            if(energyMeterGlow != null)
            {
                energyMeterGlow.SetActive(true);
            }
            
        }
        else
        {
            if (playGlow != null)
            {
                playGlow.SetActive(false);
            }
            if (energyMeterGlow != null)
            {
                energyMeterGlow.SetActive(false);
            }
        }

        if(food >= 80 && blood > 80 && blood < 140 && energy >= 80)
        {
            controller.Happy();
        }
    }

    public static void ChangeFood(int amount)
    {
        food += amount;
        
        if (food <= 0)
        {
            food = 0;
        } 
        else if (food >= 100)
        {
            food = 100;
        }
        SaveSystem.SavePet();
    }

    public static void ChangeBlood(int amount)
    {
        blood += amount;

        if (blood <= 0)
        {
            blood = 0;
        }
        else if (blood >= 220)
        {
            blood = 220;
        }
        SaveSystem.SavePet();
    }

    public static void ChangeEnergy(int amount)
    {
        energy += amount;

        if (energy <= 0)
        {
            energy = 0;
        }
        else if (energy >= 100)
        {
            energy = 100;
        }
        SaveSystem.SavePet();
    }
}
