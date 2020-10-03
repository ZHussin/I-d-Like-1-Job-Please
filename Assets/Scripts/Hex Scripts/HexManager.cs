using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexManager : MonoBehaviour
{
    private const float Cos30Deg = .8660254037f;
    public static Color GetColorFromCoords(HexCoord hc)
    {
        //should precalculate these and have them as a magic number / final / const
        Vector3 xUnitVector;
        Vector3 yUnitVector;
        Vector3 zUnitVector;
        xUnitVector = new Vector3(1, 0, 0);
        yUnitVector = new Vector3(0.5f, 0, cos30Deg);
        zUnitVector = new Vector3(-0.5f, 0, cos30Deg);

        Vector3 redMax;
        Vector3 greenMax;
        Vector3 blueMax;
        float maxMagnitude;

        redMax = HexSpaceToWorldSpace(layers, 0, 0);
        greenMax = HexSpaceToWorldSpace(0, layers, 0);
        blueMax = HexSpaceToWorldSpace(0, 0, layers);
        // the magnitude of each 
        maxMagnitude = redMax.magnitude;
        // Red on the x axis
        // Green on the y axis
        // Blue on the z axis
        // ?? Going off of world space because then the math is easier ??
        Vector3 position = HexSpaceToWorldSpace(hc.x, hc.y, 0);
        float redMagnitude = Vector3.Project(position, redMax).magnitude;
        float greenMagnitude = Vector3.Project(position, greenMax).magnitude;
        float blueMagnitude = Vector3.Project(position, blueMax).magnitude;
        if (Vector3.Angle(position, redMax) > 90)
        {
            redMagnitude = -redMagnitude;
        }
        if (Vector3.Angle(position, greenMax) > 90)
        {
            greenMagnitude = -greenMagnitude;
        }
        if (Vector3.Angle(position, blueMax) > 90)
        {
            blueMagnitude = -blueMagnitude;
        }
        float red = Mathf.InverseLerp(-maxMagnitude, maxMagnitude, redMagnitude);
        float green = Mathf.InverseLerp(-maxMagnitude, maxMagnitude, greenMagnitude);
        float blue = Mathf.InverseLerp(-maxMagnitude, maxMagnitude, blueMagnitude);

        return new Color(red, green, blue, 1);
    }
    public int layers = 5;
    public GameObject hexagon;
    public Vector3 origin;
    public static float cos30Deg = Mathf.Cos(30 * Mathf.Deg2Rad);
    public static float sin30Deg = Mathf.Sin(30 * Mathf.Deg2Rad);
    Vector3 xUnitVector;
    Vector3 yUnitVector;
    Vector3 zUnitVector;
    Vector3 redMax;
    Vector3 greenMax;
    Vector3 blueMax;
    float maxMagnitude;
    HexCoordEqualityComparer hcec;
    public Dictionary<HexCoord, GameObject> hexes;
    // float lastHexPlacedTime = 0;
    // int numHexesPlaced = 0;
    public static float offset;

    private void Awake()
    {
        // unsafe code
        if (offset < 0)
        {
            offset = 1f - cos30Deg;
        }
        hcec = new HexCoordEqualityComparer();
        hexes = new Dictionary<HexCoord, GameObject>(hcec);
        xUnitVector = new Vector3(1, 0, 0);
        yUnitVector = new Vector3(0.5f, 0, cos30Deg);
        zUnitVector = new Vector3(-0.5f, 0, cos30Deg);
        redMax = HexSpaceToWorldSpace(layers, 0, 0);
        greenMax = HexSpaceToWorldSpace(0, layers, 0);
        blueMax = HexSpaceToWorldSpace(0, 0, layers);
        maxMagnitude = redMax.magnitude;
        GenerateGrid(layers);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount == 120)
        {
            Debug.Log("frameCount == 120");
            GameObject[] neighbors = GetNeighbors(3, 3);
            Debug.Log(string.Format("neighbors.Length = {0}", neighbors.Length));
            foreach (GameObject neighbor in neighbors)
            {
                neighbor.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
            }
        }
    }

    // radius seems a little vague. It doesn't include the center tile
    void GenerateGrid(int radius)
    {
        AddHexAtCoords(0, 0);
        List<HexCoord> hexCoords = GetCoordsAtAndAround(new HexCoord(0, 0), radius);
        foreach(HexCoord hc in hexCoords)
        {
            AddHexAtCoords(hc);
        }
    }

    List<HexCoord> GetCoordsAtAndAround(HexCoord hc, int radius)
    {
        List<HexCoord> coords = new List<HexCoord>();
        // Once for each successive circle
        // Starts and goes to radius + 1 because we are separately doing 'i=0' above
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
                            AddHexAtCoords(new HexCoord(i + hc.x, -k + hc.y));
                            break;
                        case 2:
                            AddHexAtCoords(new HexCoord(i - k + hc.x, -i + hc.y));
                            break;
                        case 3:
                            AddHexAtCoords(new HexCoord(-k + hc.x, -i + k + hc.y));
                            break;
                        case 4:
                            AddHexAtCoords(new HexCoord(-i + hc.x, k + hc.y));
                            break;
                        case 5:
                            AddHexAtCoords(new HexCoord(-i + k + hc.x, i + hc.y));
                            break;
                    }
                }
            }
        }
        return coords;
    }

    void AddHexAtCoords(int x, int y)
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

    void AddHexAtCoords(HexCoord hc)
    {
        AddHexAtCoords(hc.x, hc.y);
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
    Color GetColorFromCoord(int x, int y) {
        // Red on the x axis
        // Green on the y axis
        // Blue on the z axis
        // ?? Going off of world space because then the math is easier ??
        Vector3 position = HexSpaceToWorldSpace(x, y, 0);
        float redMagnitude = Vector3.Project(position, redMax).magnitude;
        float greenMagnitude = Vector3.Project(position, greenMax).magnitude;
        float blueMagnitude = Vector3.Project(position, blueMax).magnitude;
        if (Vector3.Angle(position, redMax) > 90)
        {
            redMagnitude = -redMagnitude;
        }
        if (Vector3.Angle(position, greenMax) > 90)
        {
            greenMagnitude = -greenMagnitude;
        }
        if (Vector3.Angle(position, blueMax) > 90)
        {
            blueMagnitude = -blueMagnitude;
        }
        float red = Mathf.InverseLerp(-maxMagnitude, maxMagnitude, redMagnitude);
        float green = Mathf.InverseLerp(-maxMagnitude, maxMagnitude, greenMagnitude);
        float blue = Mathf.InverseLerp(-maxMagnitude, maxMagnitude, blueMagnitude);

        return new Color(red, green, blue, 1);
    }

    public static Vector3 HexSpaceToWorldSpace(int x, int y, int z)
    {
        // switch these to muggle magic numbers
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