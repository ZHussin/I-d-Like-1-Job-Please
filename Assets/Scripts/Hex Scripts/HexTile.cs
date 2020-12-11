using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public HexCoord coordinates;
    public Color hexColor; // For now they will just be one color, no fancy texture neither
    public int x;
    public int y;
    public float red;
    public float green;
    public float blue;

    public override string ToString()
    {
        return coordinates.ToString();
    }
}

