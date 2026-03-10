using UnityEngine;
using UnityEngine.UI;

public class Berry : MonoBehaviour
{
    public BerryData data;
    [SerializeField] new string name;

    [SerializeField] int[] descriptionTextLines;
    // Make sure this is the Index in the Berry Holder script in BoardManager
    [SerializeField] int id;
    [SerializeField] string[] tags;
    // Ids of incompatible berries
    [SerializeField] int[] incompatibility;
    [SerializeField] string[] requiredTags;
    [SerializeField] bool doesGravity;
    [SerializeField] int numToMatch;
    public int locX;
    public int locY;
    public float foodAmount;
    public float bloodAmount;
    public float energyAmount;
    [SerializeField] private bool added = false;

    // --- Properties ---

    public string Name => name;
    public int Id => id;
    public int NumToMatch => numToMatch;
    public bool DoesGravity => doesGravity;
    public int[] Incompatibilities => incompatibility;
    public string[] Tags => tags;
    public string[] RequiredTags => requiredTags;
    public int X => locX;
    public int Y => locY;

    public bool Added
    {
        get => added;
        set => added = value;
    }

    public string Description =>
        Tutorial.script[descriptionTextLines[0] - 3, Tutorial.language];

    public string Description2 =>
        Tutorial.script[descriptionTextLines[1] - 3, Tutorial.language];

    public string DataName
    {
        get
        {
            Debug.Log("Language: " + PlayerPrefs.GetInt("Language"));
            return data.name[PlayerPrefs.GetInt("Language")];
        }
    }

    public string DataDescription =>
        data.description[PlayerPrefs.GetInt("Language")];

    public Sprite DataSprite => data.sprite;

    // --- Methods ---

    public void SetPosition(int x, int y)
    {
        locX = x;
        locY = y;
    }
}
