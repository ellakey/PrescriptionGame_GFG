using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tutorial : MonoBehaviour
{
    public int languageCount;
    public GameObject[] prefabs;
    public GameObject parent;
    public RectTransform rect;
    public TextAsset tutorialScript;
    public MainMenu mainMenu;
    public float speed;

    public int[] progression;

    // Convenience accessors so other scripts can still say Tutorial.language / Tutorial.script
    // without knowing about GameState directly. Keeps changes minimal elsewhere.
    public static int language
    {
        get => GameState.Instance.language;
        set => GameState.Instance.language = value;
    }
    public static string[,] script
    {
        get => GameState.Instance.script;
        set => GameState.Instance.script = value;
    }

    private static int currentText;
    private int currentPanel;
    private int lastPanel;
    private float currentLoc;
    public int spacing = 800;

    private List<GameObject> panels;
    public static bool nameChangeable = false;
    private string currentName;

    private void Start()
    {
        currentName = "Fido";
        panels = new List<GameObject>();
        ReadCSV();
        lastPanel = 0;
        currentLoc = 0;
        for (int i = 0; i < progression.Length; i++)
        {
            panels.Add(Instantiate(prefabs[progression[i]], parent.transform));
            panels[i].GetComponent<TextChanger>().text = script[currentText, language];
            currentText++;
            if (panels[i].GetComponent<TextChanger>().hasMultiple)
            {
                panels[i].GetComponent<TextChanger>().text2 = script[currentText, language];
                currentText++;
            }
        }
    }

    private void Update()
    {
        if (currentLoc != rect.localPosition.x)
        {
            int direction = (rect.localPosition.x < currentLoc) ? 1 : -1;
            rect.localPosition = new Vector2(rect.localPosition.x + (speed * direction), rect.localPosition.y);
            if (currentLoc > rect.localPosition.x - speed + 1 && currentLoc < rect.localPosition.x + speed + 1)
            {
                rect.localPosition = new Vector2(currentLoc, rect.localPosition.y);
            }
        }
        if (nameChangeable)
        {
            GameState gs = GameState.Instance;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].GetComponent<TextChanger>().text = panels[i].GetComponent<TextChanger>().text.Replace(currentName, gs.petName);
                panels[i].GetComponent<TextChanger>().ReEvaluate();
            }
            currentName = gs.petName;
            nameChangeable = false;
        }
    }

    private void ReadCSV()
    {
        string[] currentData;
        currentData = tutorialScript.text.Split(new string[] { "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        script = new string[(currentData.Length / languageCount) - (languageCount * 2) + 2, languageCount];
        for (int i = 0; i <= (currentData.Length / languageCount) - (languageCount * 2) + 1; i++)
        {
            for (int j = 0; j < languageCount; j++)
            {
                script[i, j] = currentData[languageCount * (i + languageCount) + j];
            }
        }
    }

    private void Move()
    {
        if (currentPanel != 0)
        {
            int direction = (currentPanel > lastPanel) ? 1 : -1;
            if (progression[currentPanel] > 2 || progression[lastPanel] > 2)
            {
                currentLoc = currentLoc + (spacing * direction);
            }
            else
            {
                currentLoc = currentLoc + (spacing * direction);
                rect.localPosition = new Vector2(currentLoc, rect.localPosition.y);
            }
        }
        else
        {
            int direction = (currentPanel > lastPanel) ? 1 : -1;
            rect.localPosition = new Vector2(rect.localPosition.x + (spacing * direction), rect.localPosition.y);
            currentLoc = 0;
        }
    }

    public void MoveLeft()
    {
        if (currentPanel != 0)
        {
            lastPanel = currentPanel;
            currentPanel -= 1;
            Move();
        }
    }

    public void MoveRight()
    {
        if (currentPanel != transform.childCount - 1)
        {
            lastPanel = currentPanel;
            currentPanel += 1;
            Move();
        }
        else
        {
            mainMenu.ToPet();
        }
    }
}