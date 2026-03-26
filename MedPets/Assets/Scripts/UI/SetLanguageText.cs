using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetLanguageText : MonoBehaviour
{
    [Tooltip("Section name from the CSV, e.g. \"GAME WORDS\", \"PROGRESSION\", " +
             "\"MEDICATION BOTTLE\", \"GUIDEBOOK\". Case-insensitive.")]
    public string section;

    [Tooltip("0-based line within the section. 0 = first content line after the header.")]
    public int lineInSection;

    public TextMeshProUGUI textToChange;
    public bool setToCaps = false;

    private void Update()
    {
        GameState gs = GameState.Instance;
        if (gs == null) return;

        var s = gs.script;
        if (s == null) return;

        int start = gs.GetSectionStart(section);
        if (start < 0) return;

        int row = start + lineInSection;
        if (row >= s.GetLength(0)) return;

        int lang = gs.language;
        if (lang < 0 || lang >= s.GetLength(1)) return;

        if(setToCaps)
            textToChange.text = s[row, lang].ToUpperInvariant();
        else
            textToChange.text = s[row, lang];
    }
}