using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public GameObject prefab;

    // coordinates are primary key for tile db
    public HexCoord coordinates;

    // For now they will just be one color, no fancy texture neither
    Color hexColor;

    // hexColor = GetColorFromCoord(x, y);
    // newHex = Instantiate(hexagon, origin + HexSpaceToWorldSpace(x, y, 0), Quaternion.identity);
    // newHex.GetComponent<MeshRenderer>().material.color = hexColor;
    // HexCoord newCoord = new HexCoord(x, y);
    public HexTile(HexCoord hc)
    {
        hexColor = GetColorFromCoord(hc);
        Instantiate(prefab, origin + HexSpaceToWorldSpace(coordinates), Quaternion.identity);
    }
}

