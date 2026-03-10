using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BottleName : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        text.text = PatientInfo.petName;
    }
}
