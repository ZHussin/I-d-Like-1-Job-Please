using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MY IDEA
// To have this class handle everything related to tile adjacency
// Ex: Where can I move? HexBoard knows what's around your coords.
public class HexBoard : MonoBehaviour
{
    public Dictionary<HexCoord, HexTile> gameBoard;
    int radius;
    void Awake()
    {
        gameBoard = new Dictionary<HexCoord, HexTile>(new HexCoordEqualityComparer());
    }
    
    void Update()
    {
        
    }

    List<HexCoord> GetCoordArrayWithinRadius(HexCoord hc, int radius)
    {
        List<HexCoord> coords = new List<HexCoord>
        {
            hc
        };
        // Once for each successive circle
        // Starts and goes to radius + 1 because we are separately doing 'i=0' above with hc
        for (int i = 1; i < radius + 1; i++)
        {
            // One hex long per unit distance from the center
            for (int k = 0; k < i; k++)
            {
                // Now one row for each side of the hexagon
                for (int j = 0; j < 6; j++)
                {
                    switch (j)
                    {
                        // Amount in each one is = radius
                        // This will be the from the top-right point going clockwise
                        case 0:
                            coords.Add(new HexCoord(k + hc.x, i - k + hc.y));
                            break;
                        case 1:
                            coords.Add(new HexCoord(i + hc.x, -k + hc.y));
                            break;
                        case 2:
                            coords.Add(new HexCoord(i - k + hc.x, -i + hc.y));
                            break;
                        case 3:
                            coords.Add(new HexCoord(-k + hc.x, -i + k + hc.y));
                            break;
                        case 4:
                            coords.Add(new HexCoord(-i + hc.x, k + hc.y));
                            break;
                        case 5:
                            coords.Add(new HexCoord(-i + k + hc.x, i + hc.y));
                            break;
                    }
                }
            }
        }
        return coords;
    }

    void InstantiateHexAtCoords(int x, int y)
    {
        GameObject newHex;
        Color hexColor;
        hexColor = GetColorFromCoord(x, y);
        newHex = Instantiate(hexagon, origin + HexSpaceToWorldSpace(x, y, 0), Quaternion.identity);
        newHex.GetComponent<MeshRenderer>().material.color = hexColor;
        HexCoord newCoord = new HexCoord(x, y);
        HexTile tile = newHex.GetComponent<HexTile>();
        tile.coords = newCoord;
        tile.x = x;
        tile.y = y;
        hexes.Add(newCoord, newHex);
        newHex.transform.parent = this.gameObject.transform;
    }

    void InstantiateHexAtCoords(HexCoord hc)
    {
        InstantiateHexAtCoords(hc.x, hc.y);
    }

    GameObject[] GetNeighbors(int x, int y)
    {
        return GetNeighbors(new HexCoord(x, y));
    }

    // I hate how I wrote this one but not entirely sure how to make it cleaner
    GameObject[] GetNeighbors(HexCoord hexCoord)
    {
        List<HexCoord> neighbors = GetCoordsAtAndAround(hexCoord, 1);
        neighbors.Remove(hexCoord);
        GameObject newNeighbor;
        List<GameObject> newNeighbors = new List<GameObject>();
        foreach (HexCoord hc in neighbors)
        {
            if (hexes.TryGetValue(hc, out newNeighbor))
            {
                newNeighbors.Add(newNeighbor);
            }
        }
        return newNeighbors.ToArray();
    }

    // Implicitly uses the radius
    // Probably should tie things together a different way
    Color GetColorFromCoord(HexCoord hc)
    {
        float z = (hc.y - hc.x) / 2f;
        float xProportion, yProportion, zProportion;
        xProportion = (float)hc.x / radius;
        yProportion = (float)hc.y / radius;
        zProportion = z / radius;
        
        // Red on the x axis
        // Green on the y axis
        // Blue on the z axis
        // Alpha = 1;
        return new Color(xProportion, yProportion, zProportion, 1);
    }

    Vector3 GetHexWorldPosition(HexCoord hc)
    {
        return (cos30Deg + offset) * (x * xUnitVector + y * yUnitVector + z * zUnitVector);
    }

    private void OnDrawGizmos()
    {
        // x direction
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin + Vector3.up, origin + Vector3.up + xUnitVector);
        // y direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin + Vector3.up, origin + Vector3.up + yUnitVector);
        // z direction
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin + Vector3.up, origin + Vector3.up + zUnitVector);
    }

}