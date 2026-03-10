using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishedButton : MonoBehaviour
{
    [SerializeField] Text medInput;

    public void finish()
    {
        PatientInfo.setMedAmount(medInput.text);
        SceneManager.LoadScene("Title");
    }
}
