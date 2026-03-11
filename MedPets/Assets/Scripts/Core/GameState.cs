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

    /// <summary>
    /// Runs before any scene loads, even before Awake().
    /// If no GameState exists yet (e.g. hitting Play from a non-Title scene),
    /// creates one so all other scripts can safely access GameState.Instance.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoBootstrap()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("GameState (Auto)");
        go.AddComponent<GameState>();
        // Awake() handles the rest: sets Instance, DontDestroyOnLoad, loads save data
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

        // Auto-load saved data on startup
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
        // Auto-save when transitioning between scenes
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

    // --- Patient Info helpers (moved from static PatientInfo) ---

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

    // --- Needs helpers (moved from NeedsController statics) ---

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