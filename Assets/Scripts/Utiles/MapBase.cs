using System;
using UnityEngine;
using static Tile;

[Serializable]
public class MapBase
{
    public Color _backgroundColour;
    public Color _foregroundColour;
    public Coord _mapSize;
    public float _maxObstacleHeight;
    public float _minObstacleHeight;
    [Range(0, 1)]
    public float _obstaclePercent;
    public int _seed;

    public Coord _mapCentre
    {
        get
        {
            return new Coord(_mapSize.x / 2, _mapSize.y / 2);
        }
    }
}