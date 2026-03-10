using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicationButtons : MonoBehaviour
{
    public void SetMedication(int id)
    {
        PatientInfo.AddMedication(id);
    }
}
