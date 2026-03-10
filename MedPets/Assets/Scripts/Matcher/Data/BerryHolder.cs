using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryHolder : MonoBehaviour
{
    //Static Container that can be added to in the inspector when on an object
    [SerializeField] GameObject[] container;
    public static int itemCount;
    private void Awake()
    {
        itemCount = container.Length;
    }

    //Method to get Berry by ID/position in the container
    public GameObject getBerry(int id)
    {
        return container[id];
    }

    public int getSize()
    {
        return container.Length;
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