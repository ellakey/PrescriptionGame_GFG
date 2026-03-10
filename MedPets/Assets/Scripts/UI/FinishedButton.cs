using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishedButton : MonoBehaviour
{
    [SerializeField] Text medInput;

    public void Finish()
    {
        PatientInfo.SetMedAmount(medInput.text);
        SceneManager.LoadScene("Title");
    }
}
