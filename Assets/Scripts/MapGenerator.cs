using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Net.NetworkInformation;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    [SerializeField] private Transform _tilePrefab;
    [SerializeField] private Transform _obstaclePrefab;
    [SerializeField] private Transform _navMeshFloor;
    [SerializeField] private Transform _mapEdgePrefab;

    [SerializeField] [Range(0, 1)] private float _outlinePercent;
    [SerializeField] private float _tileSize;

    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;
    private Queue<Coord> _shuffledOpenTileCoords;
    Transform[,] _tileMap; 

    public Map _currentMap;

    void Start()
    {
        GenerateMap();
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    private void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
        _currentMap = maps[mapIndex];
        _tileMap = new Transform[_currentMap._mapSize.x, _currentMap._mapSize.y];
        GetComponent<BoxCollider>().size = new Vector3(_currentMap._mapSize.x * _tileSize,0.5f, _currentMap._mapSize.y * _tileSize);
        GenerateCoords();
        string holderName = GenerateMapHolder();
        Transform mapHolder = GenerateTiles(holderName);
        GenerateObstacles(mapHolder);
        GenerateBoundries(mapHolder);

        _navMeshFloor.localScale = new Vector3(_currentMap._mapSize.x, _currentMap._mapSize.y) * _tileSize;
    }

    private string GenerateMapHolder()
    {
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        return holderName;
    }

    private Transform GenerateTiles(string holderName)
    {
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        for (int x = 0; x < _currentMap._mapSize.x; x++)
        {
            for (int y = 0; y < _currentMap._mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(_tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - _outlinePercent) * _tileSize;
                newTile.parent = mapHolder;
                _tileMap[x, y] = newTile;
            }
        }
        return mapHolder;
    }

    private void GenerateCoords()
    {
        _allTileCoords = new List<Coord>();
        for (int x = 0; x < _currentMap._mapSize.x; x++)
        {
            for (int y = 0; y < _currentMap._mapSize.y; y++)
            {
                _allTileCoords.Add(new Coord(x, y));
            }
        }
        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _currentMap._seed));
    }

    private void GenerateObstacles(Transform mapHolder)
    {
        System.Random prng = new System.Random(_currentMap._seed);
        // matrix for tiles
        bool[,] obstacleMap = new bool[(int)_currentMap._mapSize.x, (int)_currentMap._mapSize.y];

        int obstacleCount = (int)(_currentMap._mapSize.x * _currentMap._mapSize.y * _currentMap._obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(_allTileCoords); //list of tiles without obstacles

        //generate obstacles
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != _currentMap._mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(_currentMap._minObstacleHeight, _currentMap._maxObstacleHeight, (float) prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(_obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - _outlinePercent) * _tileSize, obstacleHeight, (1- _outlinePercent)*_tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPrecent = randomCoord.y / (float)_currentMap._mapSize.y;
                obstacleMaterial.color = Color.Lerp(_currentMap._foregroundColour, _currentMap._backgroundColour, colorPrecent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        _shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), _currentMap._seed));
    }

    private void GenerateBoundries(Transform mapHolder)
    {
        Transform leftMapEdge = Instantiate(_mapEdgePrefab, (Vector3.left * (_currentMap._mapSize.x) / 2f + Vector3.up * .5f) * _tileSize, Quaternion.identity) as Transform;
        leftMapEdge.parent = mapHolder;
        leftMapEdge.localScale = new Vector3(Mathf.Epsilon, 1, _currentMap._mapSize.y) * _tileSize;
        Transform rightMapEdge = Instantiate(_mapEdgePrefab, (Vector3.right * (_currentMap._mapSize.x) / 2f + Vector3.up * .5f) * _tileSize, Quaternion.identity) as Transform;
        rightMapEdge.parent = mapHolder;
        rightMapEdge.localScale = new Vector3(Mathf.Epsilon, 1, _currentMap._mapSize.y) * _tileSize;
        Transform upMapEdge = Instantiate(_mapEdgePrefab, (Vector3.forward * (_currentMap._mapSize.y) / 2f + Vector3.up * .5f) * _tileSize, Quaternion.identity) as Transform;
        upMapEdge.parent = mapHolder;
        upMapEdge.localScale = new Vector3(_currentMap._mapSize.x, 1, Mathf.Epsilon) * _tileSize;
        Transform bottomMapEdge = Instantiate(_mapEdgePrefab, (Vector3.back * (_currentMap._mapSize.y) / 2f + Vector3.up * .5f) * _tileSize, Quaternion.identity) as Transform;
        bottomMapEdge.parent = mapHolder;
        bottomMapEdge.localScale = new Vector3(_currentMap._mapSize.x, 1, Mathf.Epsilon) * _tileSize;
        Transform groundMapEdge = Instantiate(_mapEdgePrefab, (Vector3.zero - Vector3.up * .1f) * _tileSize, Quaternion.identity) as Transform;
        groundMapEdge.localScale = new Vector3(_currentMap._mapSize.x +10, 0.1f, _currentMap._mapSize.y + 10) * _tileSize;
        groundMapEdge.parent = mapHolder;
        groundMapEdge.gameObject.layer = LayerMask.NameToLayer("Ground");
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) //bfs
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(_currentMap._mapCentre); //start point for check accessibility
        mapFlags[_currentMap._mapCentre.x, _currentMap._mapCentre.y] = true;
        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++) 
            {
                for (int y = -1; y <= 1; y++)
                {
                    //check 8 surrrounding tiles
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    //check if it not diagonal, only vertical and horizontal movement allowed
                    if (x == 0 || y == 0)
                    {
                        // chcek if bfs is in map size
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            // chceck if it was visited and if there is no obstacle
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                //add acessible tile
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        // chceck if every tile is acessible from starting point
        int targetAccessibleTileCount = (int)(_currentMap._mapSize.x * _currentMap._mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-_currentMap._mapSize.x / 2f + 0.5f + x, 0, -_currentMap._mapSize.y / 2f + 0.5f + y) * _tileSize;
    }

    public Transform TileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / _tileSize + (_currentMap._mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / _tileSize + (_currentMap._mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, _tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, _tileMap.GetLength(1) - 1);
        return _tileMap[x, y];
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = _shuffledOpenTileCoords.Dequeue();
        _shuffledOpenTileCoords.Enqueue(randomCoord);
        return _tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Coord))
                return false;

            Coord other = (Coord)obj;
            return this == other;
        }

        public override int GetHashCode()
        {
            return (x << 16) ^ y; // Dowolna funkcja mieszaj¹ca, dostosuj j¹ do swoich potrzeb
        }

    }

    [System.Serializable]
    public class Map
    {
        public Coord _mapSize;
        [Range(0, 1)]
        public float _obstaclePercent;
        public int _seed;
        public float _minObstacleHeight;
        public float _maxObstacleHeight;
        public Color _foregroundColour;
        public Color _backgroundColour;

        public Coord _mapCentre
        {
            get
            {
                return new Coord(_mapSize.x / 2, _mapSize.y / 2);
            }
        }

    }
}

