using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoad : MonoBehaviour
{
    void Start()
    {
        SaveSystem.LoadPet();
    }
}
