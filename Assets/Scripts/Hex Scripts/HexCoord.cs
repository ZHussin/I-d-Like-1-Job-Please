using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoord
{
    public int x;
    public int y;

    public HexCoord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public HexCoord[] GetNeighborCoords()
    {
        return GetNeighborCoords(x, y);
    }

    public static HexCoord[] GetNeighborCoords(int _x, int _y)
    {
        HexCoord[] neighbors = new HexCoord[6];

        // Top right first, going clockwise
        neighbors[0] = new HexCoord(_x, _y + 1);
        neighbors[1] = new HexCoord(_x + 1, _y);
        neighbors[2] = new HexCoord(_x + 1, _y - 1);
        neighbors[3] = new HexCoord(_x, _y - 1);
        neighbors[4] = new HexCoord(_x - 1, _y);
        neighbors[5] = new HexCoord(_x - 1, _y + 1);

        return neighbors;
    }

    public override string ToString()
    {
        return string.Format("x: {0}, y: {1}", x, y);
    }

}

public class HexCoordEqualityComparer : IEqualityComparer<HexCoord>
{
    public bool Equals(HexCoord a, HexCoord b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public int GetHashCode(HexCoord a)
    {
        int hash = 23;
        hash = hash * 31 + a.x.GetHashCode();
        hash = hash * 31 + a.y.GetHashCode();
        return hash;
    }
}