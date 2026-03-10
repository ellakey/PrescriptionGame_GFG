using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedsController : MonoBehaviour
{
    public PetUIController foodBar;
    public PetUIController bloodBar;
    public PetUIController energyBar;

    public PetController controller;
    public GameObject playGlow;
    public GameObject energyMeterGlow;
    public GameObject foodMeterGlow;
    public GameObject bloodMeterGlow;

    private GameState gs;

    private void Start()
    {
        gs = GameState.Instance;
        gs.playedOnce = true;
        SaveSystem.SavePet();
    }

    public void Update()
    {
        foodBar.UpdateFoodBar(100, gs.food);
        bloodBar.UpdateBloodBar(gs.blood);
        energyBar.UpdateEnergyBar(100, gs.energy);

        if (gs.food <= 20)
        {
            controller.Hungry();
            if (foodMeterGlow != null) foodMeterGlow.SetActive(true);
        }
        else
        {
            if (foodMeterGlow != null) foodMeterGlow.SetActive(false);
        }

        if (gs.food < 0) gs.food = 0;

        if (gs.blood < 80)
        {
            controller.LowBloodSugar();
            if (bloodMeterGlow != null) bloodMeterGlow.SetActive(true);
        }
        else if (gs.blood > 140)
        {
            controller.HighBloodSugar();
            if (bloodMeterGlow != null) bloodMeterGlow.SetActive(true);
        }
        else
        {
            if (bloodMeterGlow != null) bloodMeterGlow.SetActive(false);
        }

        if (gs.energy <= 40)
        {
            controller.Bored();
            if (playGlow != null) playGlow.SetActive(true);
            if (energyMeterGlow != null) energyMeterGlow.SetActive(true);
        }
        else
        {
            if (playGlow != null) playGlow.SetActive(false);
            if (energyMeterGlow != null) energyMeterGlow.SetActive(false);
        }

        if (gs.food >= 80 && gs.blood > 80 && gs.blood < 140 && gs.energy >= 80)
        {
            controller.Happy();
        }

        controller.UpdateSprite(gs.food);
    }
}