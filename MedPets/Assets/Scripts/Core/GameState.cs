using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central singleton that owns all persistent player/game data.
/// Auto-creates itself before any scene loads via RuntimeInitializeOnLoadMethod,
/// so you can press Play from any scene and have saved data available.
/// </summary>
public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [Header("Pet Needs")]
    public float food;
    public float blood;
    public float energy;
    public bool playedOnce;

    [Header("Inventory")]
    public int[] items;

    [Header("Patient Info")]
    public int medId;
    public string medAmount;
    public int[] matcherIds;
    public List<int> medications = new List<int>();
    public string petName = "Fido";
    public string dosage;
    public string time;

    [Header("Localization")]
    public int language;
    public string[,] script;

    [Header("Progression")]
    public int progressionCounter;

    [Header("Tutorial")]
    public int tutorialStep;
    public int[] tutorialPartStarts;

    [Header("CSV Sections")]

    public string[] sectionNames;
    public int[] sectionStarts;

    public int GetSectionStart(string name)
    {
        if (sectionNames == null || string.IsNullOrEmpty(name)) return -1;
        string search = name.Trim().ToUpperInvariant();
        for (int i = 0; i < sectionNames.Length; i++)
        {
            if (sectionNames[i].Trim().ToUpperInvariant() == search)
                return sectionStarts[i];
        }
        return -1;
    }

    public int GetSectionLength(string name)
    {
        if (sectionNames == null || script == null || string.IsNullOrEmpty(name)) return 0;
        string search = name.Trim().ToUpperInvariant();
        for (int i = 0; i < sectionNames.Length; i++)
        {
            if (sectionNames[i].Trim().ToUpperInvariant() != search) continue;

            int start = sectionStarts[i];
            int end = (i + 1 < sectionStarts.Length) ? sectionStarts[i + 1] : script.GetLength(0);
            return end - start;
        }
        return 0;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoBootstrap()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("GameState (Auto)");
        go.AddComponent<GameState>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SaveSystem.LoadPet();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveSystem.SavePet();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.SavePet();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SavePet();
    }

    // --- Patient Info helpers ---

    public void SetMedId(int id)
    {
        medId = id;
        matcherIds = new int[] { id, 7, 10, 4, 5 };
    }

    public int RandomMed()
    {
        if (medications.Count != 0)
        {
            return medications[Random.Range(0, medications.Count)];
        }
        return 0;
    }

    public void AddMedication(int id)
    {
        bool changed = false;
        for (int i = 0; i < medications.Count; i++)
        {
            if (medications[i] == id)
            {
                medications.Remove(id);
                changed = true;
                break;
            }
        }
        if (!changed)
        {
            medications.Add(id);
        }
        SaveSystem.SavePet();
    }

    // --- Needs helpers ---

    public void ChangeFood(int amount)
    {
        food = Mathf.Clamp(food + amount, 0, 100);
        SaveSystem.SavePet();
    }

    public void ChangeBlood(int amount)
    {
        blood = Mathf.Clamp(blood + amount, 0, 220);
        SaveSystem.SavePet();
    }

    public void ChangeEnergy(int amount)
    {
        energy = Mathf.Clamp(energy + amount, 0, 100);
        SaveSystem.SavePet();
    }

    // --- Inventory helpers ---

    public void InitItems(int size)
    {
        if (items == null || items.Length == 0)
        {
            items = new int[size];
        }
    }
}