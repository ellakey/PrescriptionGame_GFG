using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangePetName : MonoBehaviour
{
    public void changePetName(string name)
    {
        PatientInfo.petName = name;
        if(name.Equals("") || name.Equals(" "))
        {
            PatientInfo.petName = "Fido";
        }
        Tutorial.nameChangeable = true;
    }
}
