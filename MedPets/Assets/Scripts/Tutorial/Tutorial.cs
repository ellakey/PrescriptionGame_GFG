using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

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

    public static Tutorial Instance { get; private set; }

    private int currentText;
    private int currentPanel;
    private int lastPanel;
    private float currentLoc;
    public int spacing = 800;

    private List<GameObject> panels;
    private bool nameChangeable = false;
    private string currentName;

    // Matches section headers like "1 . TUTORIAL", "2 . GAME WORDS", "4 . GUIDEBOOK"
    private static readonly Regex SectionHeaderPattern = new Regex(@"^\d+\s*\.\s*(.+)$");

    private void Start()
    {
        Instance = this;
        currentName = "Fido";
        panels = new List<GameObject>();
        ReadCSV();
        lastPanel = 0;
        currentLoc = 0;

        // Intro lines start at row 0 (before any PART header)
        currentText = 0;

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
        string[] lines = tutorialScript.text.Split('\n');

        List<string[]> contentRows = new List<string[]>();
        List<int> partStarts = new List<int>();
        List<string> secNames = new List<string>();
        List<int> secStarts = new List<int>();

        foreach (string rawLine in lines)
        {
            if (string.IsNullOrWhiteSpace(rawLine)) continue;

            // Strip all quote characters (CSV quoting artifacts, also fixes
            // broken quoting from commas in Spanish text)
            string line = rawLine.Replace("\"", "");

            // Strip trailing commas (empty spreadsheet columns)
            line = line.TrimEnd(',').Trim();

            if (string.IsNullOrEmpty(line)) continue;

            // --- Detect PART headers: "PART 1: PET STATS---" ---
            if (line.StartsWith("PART ", StringComparison.OrdinalIgnoreCase))
            {
                partStarts.Add(contentRows.Count);
                continue;
            }

            // Skip the language header row (e.g. "English (Revised)\tSpanish (Revised)")
            if (line.IndexOf("English", StringComparison.OrdinalIgnoreCase) >= 0 &&
                line.IndexOf("Spanish", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                continue;
            }

            // Split by tab to get language columns
            string[] cols = line.Split('\t');
            string firstCol = cols[0].Trim();

            // --- Detect section headers: "N . NAME" with dashes in second column ---
            // (e.g. "2 . GAME WORDS\t-------")
            Match sectionMatch = SectionHeaderPattern.Match(firstCol);
            if (sectionMatch.Success)
            {
                // Check that second column is all dashes or the row has only one real column
                bool isDashColumn = cols.Length >= 2 && IsDecorative(cols[1].Trim());
                bool isSingleColumn = cols.Length < languageCount;

                if (isDashColumn || isSingleColumn)
                {
                    string name = sectionMatch.Groups[1].Value.Trim();
                    // Strip any trailing dashes from the name itself
                    name = name.TrimEnd('-').Trim();
                    secNames.Add(name);
                    secStarts.Add(contentRows.Count);
                    continue;
                }
            }

            // --- Content row: must have at least languageCount non-empty columns ---
            if (cols.Length < languageCount) continue;

            bool isContent = true;
            for (int j = 0; j < languageCount; j++)
            {
                string cell = cols[j].Trim();
                if (string.IsNullOrEmpty(cell) || IsDecorative(cell))
                {
                    isContent = false;
                    break;
                }
            }
            if (!isContent) continue;

            string[] cleaned = new string[languageCount];
            for (int j = 0; j < languageCount; j++)
            {
                cleaned[j] = cols[j].Trim();
            }
            contentRows.Add(cleaned);
        }

        // Build script array
        script = new string[contentRows.Count, languageCount];
        for (int i = 0; i < contentRows.Count; i++)
        {
            for (int j = 0; j < languageCount; j++)
            {
                script[i, j] = contentRows[i][j];
            }
        }

        // Store on GameState
        GameState gs = GameState.Instance;
        gs.tutorialPartStarts = partStarts.ToArray();
        gs.sectionNames = secNames.ToArray();
        gs.sectionStarts = secStarts.ToArray();

        // Debug log for verification
        for (int s = 0; s < secNames.Count; s++)
        {
            int start = secStarts[s];
            string preview = (start < contentRows.Count) ? script[start, 0] : "(empty)";
            Debug.Log($"[CSV] Section \"{secNames[s]}\" starts at row {start}: \"{preview}\"");
        }
        for (int p = 0; p < partStarts.Count; p++)
        {
            int start = partStarts[p];
            string preview = (start < contentRows.Count) ? script[start, 0] : "(empty)";
            Debug.Log($"[CSV] PART {p + 1} starts at row {start}: \"{preview}\"");
        }
    }

    private static bool IsDecorative(string cell)
    {
        if (cell.Length == 0) return false;
        foreach (char c in cell)
        {
            if (c != '-' && c != ' ') return false;
        }
        return true;
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
            // Intro tutorial done — advance step so PART 1 triggers on Pet scene
            GameState gs = GameState.Instance;
            if (gs.tutorialStep < 1)
            {
                gs.tutorialStep = 1;
                SaveSystem.SavePet();
            }
            mainMenu.ToPet();
        }
    }

    public void NotifyNameChanged()
    {
        nameChangeable = true;
    }
}