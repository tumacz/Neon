using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Tile;

public class MapProvider : MonoBehaviour
{
    [SerializeField] private List<GameObject> _mapList;

    public GameObject _map;
    private Transform[,] _tileMap;
    public List<Coord> _allUnoccupiedTileCoords = new List<Coord>();
    private float _tileSize;
    private int _mapsizeX;
    private int _mapsizeY;
    private Queue<GameObject> _mapQueue;
    Tile[] tiles = null;
    List<Transform> _tileList;

    private void Start()
    {
        _mapQueue = new Queue<GameObject>();
        if (_mapList != null && _mapList.Count > 0)
        {
            foreach (GameObject map in _mapList)
            {
                _mapQueue.Enqueue(map);
            }
            if (_mapQueue.Count > 0)
            {
                GameObject selectedMap = _mapQueue.Dequeue();
                _map = Instantiate(selectedMap, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Map queue is empty!");
            }
            RecollectTiles();
        }
    }

    public void NextMap()
    {
        if (_mapQueue.Count > 0)
        {
            if (_map != null)
            {
                _map.SetActive(false);
                Destroy(_map);
                _map = null;
                tiles = null;
            }
            GameObject selectedMap = _mapQueue.Dequeue();
            _map = Instantiate(selectedMap, transform.position, Quaternion.identity);
            RecollectTiles();
        }
        else
        {
            Debug.Log("no maps in queue");
        }
    }

    private void RecollectTiles()
    {
        _tileList = _map.GetComponent<TileRunner>()._tileList;

        int maxX = int.MinValue;
        int maxY = int.MinValue;
        float maxTileSize = float.MinValue;
        _allUnoccupiedTileCoords.Clear();

        foreach (Tile tileComponent in _tileList.Select(t => t.GetComponent<Tile>()))
        {
            if (tileComponent.coord.x > maxX)
            {
                maxX = tileComponent.coord.x;
            }
            if (tileComponent.coord.y > maxY)
            {
                maxY = tileComponent.coord.y;
            }
            if (!tileComponent.isOccupied)
            {
                _allUnoccupiedTileCoords.Add(tileComponent.coord);
            }
            if (tileComponent._catchTileSize > maxTileSize)
            {
                maxTileSize = tileComponent._catchTileSize;
            }
        }

        _tileMap = new Transform[maxX + 1, maxY + 1];

        foreach (Tile tileComponent in _tileList.Select(t => t.GetComponent<Tile>()))
        {
            _tileMap[tileComponent.coord.x, tileComponent.coord.y] = tileComponent.transform;
        }

        _tileSize = maxTileSize;
        _mapsizeX = maxX;
        _mapsizeY = maxY;
    }

    public Transform TileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / _tileSize + (_mapsizeX - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / _tileSize + (_mapsizeY - 1) / 2f);
        x = Mathf.Clamp(x, 0, _tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, _tileMap.GetLength(1) - 1);
        if (_tileMap != null)
        {
            return _tileMap[x, y];
        }
        else
        {
            return null;
        }
    }

    public Transform GetRandomOpenTile()
    {
        if (_allUnoccupiedTileCoords.Count == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, _allUnoccupiedTileCoords.Count);
        Coord randomCoord = _allUnoccupiedTileCoords[randomIndex];

        return _tileMap[randomCoord.x, randomCoord.y];
    }
}