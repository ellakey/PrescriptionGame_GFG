using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryHolder : MonoBehaviour
{
    // Static Container that can be added to in the inspector when on an object
    [SerializeField] GameObject[] container;

    public int Size => container.Length;

    // Get Berry prefab by ID/position in the container
    public GameObject GetBerry(int id)
    {
        return container[id];
    }
}

/*
 * LIST OF TAGS CURRENTLY IN GAME
 * 
 * Carb
 * Protein
 * Fat
 * Medicine
 * Fiber
 * Natural Sugar
 * Refined Sugar
 * 
 */