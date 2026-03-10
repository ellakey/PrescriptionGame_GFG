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
        textToChange.text = Tutorial.script[textLine - 3, Tutorial.language];
    }
}
