using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetLanguageText : MonoBehaviour
{
    public int textLine;
    public TextMeshProUGUI textToChange;

    private void Update()
    {
        var s = Tutorial.script;
        if (s == null) return;

        int index = textLine - Tutorial.HeaderRows;
        if (index < 0 || index >= s.GetLength(0)) return;

        int lang = Tutorial.language;
        if (lang < 0 || lang >= s.GetLength(1)) return;

        textToChange.text = s[index, lang];
    }
}