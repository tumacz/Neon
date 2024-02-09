using UnityEngine;

public class Tile : MonoBehaviour
{
    public Coord coord;
    public bool isOccupied = false;
    public float _catchTileSize;


    public void SetCoord(int x, int y)
    {
        coord = new Coord(x, y);
    }

    [System.Serializable]
    public class Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
}
