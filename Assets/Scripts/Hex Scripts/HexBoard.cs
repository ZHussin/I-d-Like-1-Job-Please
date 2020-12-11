using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MY IDEA
// To have this class handle everything related to tile adjacency
// Ex: Where can I move? HexBoard knows what's around your coords.
public class HexBoard : MonoBehaviour
{
    public const float Cos30Deg = .8660254037f;
    public float offset = 1 - Cos30Deg;
    public GameObject hexTilePrefab;
    public Dictionary<HexCoord, HexTile> gameBoard;
    // considering an adjacency list here, would mean much faster lookup than using GetNeighbors()
    // unsure what the memory impact would be of using List<> here
    public Dictionary<HexTile, List<HexTile>> tileAdjacencies;
    public Vector3 boardOrigin;
    public int boardRadius;
    Vector3 xUnitVector = new Vector3(1, 0, 0);
    Vector3 yUnitVector = new Vector3(0.5f, 0, Cos30Deg);

    public void Awake()
    {
        gameBoard = new Dictionary<HexCoord, HexTile>(new HexCoordEqualityComparer());
        boardOrigin = gameObject.transform.position;
        xUnitVector = new Vector3(1, 0, 0);
        yUnitVector = new Vector3(0.5f, 0, Cos30Deg);
        GenerateBoard();
    }
    
    public void Update()
    {
        
    }

    public void GenerateBoard()
    {
        List<HexCoord> hexCoords = GetCoordArrayWithinRadius(new HexCoord(0, 0), boardRadius);
        foreach (HexCoord hc in hexCoords)
        {
            InstantiateHexAtCoords(hc);
        }
    }
    
    public List<HexCoord> GetCoordArrayWithinRadius(HexCoord hc, int radius)
    {
        List<HexCoord> coords = new List<HexCoord>
        {
            hc
        };
        // Once for each successive circle
        // Starts and goes to radius + 1 because we are separately doing 'i=0' above with hc in the constructor
        for (int i = 1; i < radius + 1; i++)
        {
            coords.AddRange(GetCoordsInRing(hc, i));
        }
        return coords;
    }

    public List<HexCoord> GetCoordsInRing(HexCoord hc, int radius)
    {
        List<HexCoord> coords = new List<HexCoord>(radius * 6);
        // Now one row for each side of the hexagon
        for (int j = 0; j < 6; j++)
        {
            // One hex long per unit distance from the center
            for (int k = 0; k < radius; k++)
            {
                switch (j)
                {
                    // Amount in each one is = radius
                    // This will be the from the top-right point going clockwise
                    case 0:
                        coords.Add(new HexCoord(k + hc.x, radius - k + hc.y));
                        break;
                    case 1:
                        coords.Add(new HexCoord(radius + hc.x, -k + hc.y));
                        break;
                    case 2:
                        coords.Add(new HexCoord(radius - k + hc.x, -radius + hc.y));
                        break;
                    case 3:
                        coords.Add(new HexCoord(-k + hc.x, -radius + k + hc.y));
                        break;
                    case 4:
                        coords.Add(new HexCoord(-radius + hc.x, k + hc.y));
                        break;
                    case 5:
                        coords.Add(new HexCoord(-radius + k + hc.x, radius + hc.y));
                        break;
                }
            }
        }
        return coords;
    }

    public void InstantiateHexAtCoords(HexCoord hc)
    {
        GameObject newHex = Instantiate(hexTilePrefab, boardOrigin + LocalCoordToWorldSpace(hc), Quaternion.identity);
        newHex.transform.parent = this.gameObject.transform;
        Color newColor = ComputeColorFromCoord(hc);
        newHex.GetComponent<MeshRenderer>().material.color = newColor;
        HexTile tile = newHex.GetComponent<HexTile>();
        tile.coordinates = hc;
        tile.hexColor = newColor;
        tile.gameObject.name = string.Format("HexTile: ({0},{1})", hc.x, hc.y);
        tile.x = hc.x;
        tile.y = hc.y;
        tile.red = newColor.r;
        tile.green = newColor.g;
        tile.blue = newColor.b;
        gameBoard.Add(hc, tile);
    }

    public List<HexTile> GetNeighbors(HexCoord hexCoord)
    {
        List<HexCoord> neighbors = GetCoordsInRing(hexCoord, 1);
        HexTile newNeighbor;
        List<HexTile> newNeighbors = new List<HexTile>();
        foreach (HexCoord hc in neighbors)
        {
            if (gameBoard.TryGetValue(hc, out newNeighbor))
            {
                newNeighbors.Add(newNeighbor);
            }
        }
        return newNeighbors;
    }

    public List<HexTile> GetAdjacent(HexTile ht)
    {
        List<HexTile> adjacentTiles;
        tileAdjacencies.TryGetValue(ht, out adjacentTiles);
        return adjacentTiles;
    }

    public Color ComputeColorFromCoord(HexCoord hc)
    {
        float z = (hc.y - hc.x) / 2f;
        float xProportion, yProportion, zProportion;
        xProportion = (float)(hc.x + .5 * hc.y) / boardRadius;
        yProportion = (float)(hc.y + .5 * hc.x) / boardRadius;
        zProportion = z / boardRadius;
        float red, green, blue;
        red = (xProportion + 1) / 2;
        green = (yProportion + 1) / 2;
        blue = (zProportion + 1) / 2;
        // Red on the x axis
        // Green on the y axis
        // Blue on the z axis
        // Alpha = 1;
        return new Color(red, green, blue, 1);
    }

    public Vector3 LocalCoordToWorldSpace(HexCoord hc)
    {
        return boardOrigin + (Cos30Deg + offset) * (hc.x * xUnitVector + hc.y * yUnitVector);
    }

    private void OnDrawGizmos()
    {
        // x direction
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(boardOrigin + Vector3.up, boardOrigin + Vector3.up + xUnitVector);
        // y direction
        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(boardOrigin + Vector3.up, boardOrigin + Vector3.up + yUnitVector);
    }

}