using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    void Start()
    {
        GameGrid.score = 0;
        for(int i = 0; i < DragBehavior.itemCounts.Length; i++)
        {
            DragBehavior.itemCounts[i] = 0;
        }
    }
}
