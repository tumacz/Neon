using System;
using UnityEngine;

public class Tile: MonoBehaviour
{
    public Coord coord;
    public bool isOccupied = false;
    public float _catchTileSize;

    public void SetCoord(int x, int y)
    {
        coord = new Coord(x, y);
    }

    [System.Serializable]
    public struct Coord : IEquatable<Coord>
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coord))
                return false;

            Coord other = (Coord)obj;
            return this == other;
        }

        public bool Equals(Coord other)
        {
            return this == other;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

        public override int GetHashCode()
        {
            return (x << 16) ^ y;
        }
    }
}
