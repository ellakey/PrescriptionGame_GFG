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

    private float originalFontSize;
    private bool initialized;

    private void Start()
    {
        if (textToChange != null)
        {
            originalFontSize = textToChange.fontSize;
            textToChange.enableAutoSizing = true;
            textToChange.fontSizeMin = 8f;
            textToChange.fontSizeMax = originalFontSize;
            initialized = true;
        }
    }

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

        textToChange.text = s[row, lang];
    }
}