using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicationButtons : MonoBehaviour
{
    public void setMedication(int id)
    {
        PatientInfo.addMedication(id);
    }
}
