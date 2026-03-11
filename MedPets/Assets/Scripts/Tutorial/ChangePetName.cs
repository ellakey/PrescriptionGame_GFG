using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangePetName : MonoBehaviour
{
    public void ChangeName(string name)
    {
        PatientInfo.PetName = name;
        if(name.Equals("") || name.Equals(" "))
        {
            PatientInfo.PetName = "Pet";
        }
        if (Tutorial.Instance != null)
        {
            Tutorial.Instance.NotifyNameChanged();
        }
    }
}
