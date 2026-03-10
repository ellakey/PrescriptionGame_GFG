using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePet()
    {
        PlayerPrefs.SetFloat("Blood", NeedsController.blood);
        PlayerPrefs.SetFloat("Food", NeedsController.food);
        PlayerPrefs.SetFloat("Energy", NeedsController.energy);
        for(int i = 0; i < Inventory.items.Length; i++)
        {
            PlayerPrefs.SetInt("Item" + i, Inventory.items[i]);
        }
        for (int i = 0; i < PatientInfo.medications.Count; i++)
        {
            PlayerPrefs.SetInt("Med" + i, PatientInfo.medications[i]);
        }
        PlayerPrefs.SetInt("MedAmount", PatientInfo.medications.Count);
        PlayerPrefs.SetInt("Progression", Progression.progressionCounter);
        SetBool("PetVisited", NeedsController.playedOnce);
        PlayerPrefs.SetString("PetName", PatientInfo.petName);
        PlayerPrefs.SetInt("Language", Tutorial.language);
        SaveScript();
        PlayerPrefs.Save();
    }

    public static void LoadPet()
    {
        NeedsController.blood = PlayerPrefs.GetFloat("Blood", NeedsController.blood);
        NeedsController.food = PlayerPrefs.GetFloat("Food", NeedsController.food);
        NeedsController.energy = PlayerPrefs.GetFloat("Energy", NeedsController.energy);
        for (int i = 0; i < Inventory.items.Length; i++)
        {
            Inventory.items[i] = PlayerPrefs.GetInt("Item" + i, 0);
        }
        int medAmount = PlayerPrefs.GetInt("MedAmount", 0);
        Progression.progressionCounter = PlayerPrefs.GetInt("Progression", Progression.progressionCounter);
        NeedsController.playedOnce = GetBool("PetVisited", NeedsController.playedOnce);
        PatientInfo.petName = PlayerPrefs.GetString("PetName", PatientInfo.petName);
        Tutorial.language = PlayerPrefs.GetInt("Language", Tutorial.language);
        for (int i = 0; i < medAmount; i++)
        {
            Debug.Log("Before Med" + i + " " + PlayerPrefs.GetInt("Med" + i, 0));
            PatientInfo.addMedication(PlayerPrefs.GetInt("Med" + i, 0));
            Debug.Log("After Med");
        }
        LoadScript();
    }

    private static void SetBool(string name, bool value)
    {
        if (value)
        {
            PlayerPrefs.SetInt(name, 1);
        }
        else
        {
            PlayerPrefs.SetInt(name, 0);
        }
    }

    private static bool GetBool(string name, bool dflt)
    {
        if (PlayerPrefs.GetInt(name, 0) == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void SaveScript()
    {
        PlayerPrefs.SetInt("ScriptLength", Tutorial.script.GetLength(0));
        PlayerPrefs.SetInt("ScriptWidth", Tutorial.script.GetLength(1));
        for (int i = 0; i < Tutorial.script.GetLength(0); i++)
        {
            for(int j = 0; j < Tutorial.script.GetLength(1); j++)
            {
                PlayerPrefs.SetString("Script" + i + "-" + j, Tutorial.script[i, j]);
            }
        }
    }

    private static void LoadScript()
    {
        if(PlayerPrefs.GetInt("ScriptLength", 0) != 0)
        {
            Tutorial.script = new string[PlayerPrefs.GetInt("ScriptLength", 0), PlayerPrefs.GetInt("ScriptWidth", 0)];
            for (int i = 0; i < PlayerPrefs.GetInt("ScriptLength", 0); i++)
            {
                for (int j = 0; j < PlayerPrefs.GetInt("ScriptWidth", 0); j++)
                {
                    Tutorial.script[i, j] = PlayerPrefs.GetString("Script" + i + "-" + j, "N/A");
                }
            }
        }
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteAll();
        ResetAllItems();
    }

    public static void ResetAllItems()
    {
        // get scriptables from resources folder and reset them
        BerryData[] berries = Resources.LoadAll<BerryData>("BerryData");
        foreach (BerryData berry in berries)
        {
            berry.hasBeenUsed = false;
        }
    }

}
