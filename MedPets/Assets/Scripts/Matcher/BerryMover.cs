using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryMover : MonoBehaviour
{

    [SerializeField] GameGrid grid;
    [SerializeField] float xScale;
    [SerializeField] float yScale;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    [SerializeField] float berryScale;


    public float getScale()
    {
        return berryScale;
    }

    //Converts X from matrix to real space
    public float conX(float x)
    {
        return (x*xScale + xOffset - (xScale*(grid.getWidth() - 1) / 2));
    }

    //Converts X from real space to matrix
    public float revConX(float x)
    {
        return (x / xScale - xOffset + (xScale*(grid.getWidth() - 1) / 2));
    }
    //Same for Y
    public float conY(float y)
    {
        return -(y*yScale + yOffset - (yScale*(grid.getHeight() - 1) / 2));
    }

    public float revConY(float y)
    {
        return -(y / yScale - yOffset + (yScale*(grid.getHeight() - 1) / 2));
    }

}
