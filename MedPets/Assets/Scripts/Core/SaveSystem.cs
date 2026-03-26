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
        PlayerPrefs.SetInt("TutorialStep", gs.tutorialStep);
        SetBool("PetVisited", gs.playedOnce);
        PlayerPrefs.SetString("PetName", gs.petName);
        PlayerPrefs.SetInt("Language", gs.language);
        SaveScript(gs);
        SavePartStarts(gs);
        SaveSections(gs);
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
        gs.tutorialStep = PlayerPrefs.GetInt("TutorialStep", gs.tutorialStep);
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
        LoadPartStarts(gs);
        LoadSections(gs);
    }

    // --- Tutorial part starts ---

    private static void SavePartStarts(GameState gs)
    {
        int count = (gs.tutorialPartStarts != null) ? gs.tutorialPartStarts.Length : 0;
        PlayerPrefs.SetInt("TutPartCount", count);
        for (int i = 0; i < count; i++)
        {
            PlayerPrefs.SetInt("TutPart" + i, gs.tutorialPartStarts[i]);
        }
    }

    private static void LoadPartStarts(GameState gs)
    {
        int count = PlayerPrefs.GetInt("TutPartCount", 0);
        if (count > 0)
        {
            gs.tutorialPartStarts = new int[count];
            for (int i = 0; i < count; i++)
            {
                gs.tutorialPartStarts[i] = PlayerPrefs.GetInt("TutPart" + i, 0);
            }
        }
    }

    // --- CSV section lookup ---

    private static void SaveSections(GameState gs)
    {
        int count = (gs.sectionNames != null) ? gs.sectionNames.Length : 0;
        PlayerPrefs.SetInt("SecCount", count);
        for (int i = 0; i < count; i++)
        {
            PlayerPrefs.SetString("SecName" + i, gs.sectionNames[i]);
            PlayerPrefs.SetInt("SecStart" + i, gs.sectionStarts[i]);
        }
    }

    private static void LoadSections(GameState gs)
    {
        int count = PlayerPrefs.GetInt("SecCount", 0);
        if (count > 0)
        {
            gs.sectionNames = new string[count];
            gs.sectionStarts = new int[count];
            for (int i = 0; i < count; i++)
            {
                gs.sectionNames[i] = PlayerPrefs.GetString("SecName" + i, "");
                gs.sectionStarts[i] = PlayerPrefs.GetInt("SecStart" + i, 0);
            }
        }
    }

    // --- Script ---

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

    // --- Helpers ---

    private static void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }

    private static bool GetBool(string name, bool dflt)
    {
        return PlayerPrefs.GetInt(name, 0) == 1;
    }

    // --- Reset ---

    public static void Reset()
    {
        PlayerPrefs.DeleteAll();
        ResetAllItems();

        GameState gs = GameState.Instance;
        if (gs != null)
        {
            gs.food = 0;
            gs.blood = 0;
            gs.energy = 0;
            gs.playedOnce = false;
            gs.petName = "Pet";
            gs.medId = 0;
            gs.medAmount = "";
            gs.matcherIds = null;
            gs.medications.Clear();
            gs.dosage = "";
            gs.time = "";
            gs.language = 0;
            gs.script = null;
            gs.progressionCounter = 0;
            gs.tutorialStep = 0;
            gs.tutorialPartStarts = null;
            gs.sectionNames = null;
            gs.sectionStarts = null;
            if (gs.items != null)
            {
                for (int i = 0; i < gs.items.Length; i++)
                    gs.items[i] = 0;
            }
        }
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