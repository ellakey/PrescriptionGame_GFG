using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChecks : MonoBehaviour
{
    public GameObject check;

    public void CheckButton()
    {
        if (check.activeSelf)
        {
            check.SetActive(false);
        }
        else
        {
            check.SetActive(true);
        }
    }
}
