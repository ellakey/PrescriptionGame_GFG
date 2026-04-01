using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BerryData", menuName = "ScriptableObjects/BerryData", order = 1)]
public class BerryData : ScriptableObject
{
    public string ID;
    public string[] localizedNames;
    public string[] localizedDescriptions;
    public Sprite sprite;
    public bool hasBeenUsed = false;

    #if UNITY_EDITOR
    void OnEnable()
    {
        hasBeenUsed = false;
    }
    #endif

    public void Use()
    {
        hasBeenUsed = true;
    }
}