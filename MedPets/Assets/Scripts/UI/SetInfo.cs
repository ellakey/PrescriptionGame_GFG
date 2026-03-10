using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetInfo : MonoBehaviour
{
    [SerializeField] Text t1;
    [SerializeField] Text t2;
    [SerializeField] Text amount;
    // Start is called before the first frame update
    void Start()
    {
        t1.text = PatientInfo.time1;
        t2.text = PatientInfo.time2;
        amount.text = PatientInfo.medAmount;
    }
}
