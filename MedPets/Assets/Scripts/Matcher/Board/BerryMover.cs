using UnityEngine;

public class BerryMover : MonoBehaviour
{
    [SerializeField] GameGrid grid;
    [SerializeField] float xScale;
    [SerializeField] float yScale;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    [SerializeField] float berryScale;

    public float Scale => berryScale;

    // Converts X from matrix to real space
    public float ConvertX(float x)
    {
        return (x * xScale + xOffset - (xScale * (grid.Width - 1) / 2));
    }

    // Converts X from real space to matrix
    public float ReverseConvertX(float x)
    {
        return (x / xScale - xOffset + (xScale * (grid.Width - 1) / 2));
    }

    // Converts Y from matrix to real space
    public float ConvertY(float y)
    {
        return -(y * yScale + yOffset - (yScale * (grid.Height - 1) / 2));
    }

    // Converts Y from real space to matrix
    public float ReverseConvertY(float y)
    {
        return -(y / yScale - yOffset + (yScale * (grid.Height - 1) / 2));
    }
}
