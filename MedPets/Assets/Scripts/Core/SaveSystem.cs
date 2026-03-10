using UnityEngine;

public static class SaveSystem
{
    public static void SavePet()
    {
        GameState gs = GameState.Instance;
        if (gs == null) return;

        PlayerPrefs.SetFloat("Blood", gs.blood);
        PlayerPrefs.SetFloat("Food", gs.food);
        PlayerPrefs.SetFloat("Energy", gs.energy);

        if (gs.items != null)
        {
            for (int i = 0; i < gs.items.Length; i++)
            {
                PlayerPrefs.SetInt("Item" + i, gs.items[i]);
            }
        }

        for (int i = 0; i < gs.medications.Count; i++)
        {
            PlayerPrefs.SetInt("Med" + i, gs.medications[i]);
        }
        PlayerPrefs.SetInt("MedAmount", gs.medications.Count);
        PlayerPrefs.SetInt("Progression", gs.progressionCounter);
        SetBool("PetVisited", gs.playedOnce);
        PlayerPrefs.SetString("PetName", gs.petName);
        PlayerPrefs.SetInt("Language", gs.language);
        SaveScript(gs);
        PlayerPrefs.Save();
    }

    public static void LoadPet()
    {
        GameState gs = GameState.Instance;
        if (gs == null) return;

        gs.blood = PlayerPrefs.GetFloat("Blood", gs.blood);
        gs.food = PlayerPrefs.GetFloat("Food", gs.food);
        gs.energy = PlayerPrefs.GetFloat("Energy", gs.energy);

        if (gs.items != null)
        {
            for (int i = 0; i < gs.items.Length; i++)
            {
                gs.items[i] = PlayerPrefs.GetInt("Item" + i, 0);
            }
        }

        int medAmount = PlayerPrefs.GetInt("MedAmount", 0);
        gs.progressionCounter = PlayerPrefs.GetInt("Progression", gs.progressionCounter);
        gs.playedOnce = GetBool("PetVisited", gs.playedOnce);
        gs.petName = PlayerPrefs.GetString("PetName", gs.petName);
        gs.language = PlayerPrefs.GetInt("Language", gs.language);

        // Clear before loading — AddMedication is a toggle, so calling it
        // on meds already in the list would remove them instead of adding them.
        gs.medications.Clear();
        for (int i = 0; i < medAmount; i++)
        {
            gs.AddMedication(PlayerPrefs.GetInt("Med" + i, 0));
        }

        LoadScript(gs);
    }

    private static void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }

    private static bool GetBool(string name, bool dflt)
    {
        return PlayerPrefs.GetInt(name, 0) == 1;
    }

    private static void SaveScript(GameState gs)
    {
        if (gs.script == null) return;
        PlayerPrefs.SetInt("ScriptLength", gs.script.GetLength(0));
        PlayerPrefs.SetInt("ScriptWidth", gs.script.GetLength(1));
        for (int i = 0; i < gs.script.GetLength(0); i++)
        {
            for (int j = 0; j < gs.script.GetLength(1); j++)
            {
                PlayerPrefs.SetString("Script" + i + "-" + j, gs.script[i, j]);
            }
        }
    }

    private static void LoadScript(GameState gs)
    {
        int length = PlayerPrefs.GetInt("ScriptLength", 0);
        if (length != 0)
        {
            int width = PlayerPrefs.GetInt("ScriptWidth", 0);
            gs.script = new string[length, width];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    gs.script[i, j] = PlayerPrefs.GetString("Script" + i + "-" + j, "N/A");
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
        BerryData[] berries = Resources.LoadAll<BerryData>("BerryData");
        foreach (BerryData berry in berries)
        {
            berry.hasBeenUsed = false;
        }
    }
}