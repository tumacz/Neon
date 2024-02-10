using System.Collections;
using UnityEngine;
using Zenject;

public class Spawner : MonoBehaviour
{
    //[Inject]
    private MapProvider _mapProvider;

    public Wawe[] wawes;
    public Enemy enemy;

    //[SerializeField] private PlayerController _playerPrefab;
    private HealthComponent _playerHealthComponent;
    private Transform _playerPosition => _playerHealthComponent.transform;
    private Wawe currentWawe;

    private int _currentWaweNumber;
    private int _enemiesToSpawn;
    private int _enemiesRamainingAlive;

    private float _nextSpawnTime;
    private float _timeBetweenCampaingChecks = 2f;
    private float _nextCampCheckTime;
    private float _campThresholdDistance = 1.5f;

    private Vector3 campPositionOld;

    private bool isCamping;
    private bool isDiseabled;

    //public event System.Action<int> OnNewWave;

    private void Start()
    {
        _mapProvider = FindObjectOfType<MapProvider>();
        _nextCampCheckTime = _timeBetweenCampaingChecks + Time.time;
        _playerHealthComponent = FindObjectOfType<HealthComponent>();
        //_playerPosition = _playerHealthComponent.transform;
        campPositionOld = _playerHealthComponent.transform.position;
        //SpawnPlayer();
        _playerHealthComponent.OnDeath += OnPlayerDeath;
        NextWave();
    }

    private void Update()
    {
        if (!isDiseabled)
        {
            //check if camping
            if (Time.time > _nextCampCheckTime)
            {
                if (_playerPosition != null)
                {
                    _nextCampCheckTime = Time.time + _timeBetweenCampaingChecks;
                    isCamping = (Vector3.Distance(_playerPosition.position, campPositionOld) < _campThresholdDistance);
                    campPositionOld = _playerPosition.position;
                }
                isCamping = false;
            }
            if ((_enemiesToSpawn > 0 || currentWawe.infinite) && Time.time > _nextSpawnTime)
            {
                _enemiesToSpawn--;
                _nextSpawnTime = Time.time + currentWawe.timeBetwenSpawn;
                if(_playerPosition.position !=null)
                StartCoroutine(SpawEnemy());
            }
        }
    }

    private IEnumerator SpawEnemy()
    {
        Transform randomTile = null;
        float spawnDelay = 1f;
        float tileFlashSpeed = 4;

        if (isCamping)
        {
            if (_playerPosition != null)
            {
                randomTile = _mapProvider.TileFromPosition(_playerPosition.position);
            }
        }
        else
        {
            randomTile = _mapProvider.GetRandomOpenTile();
            Debug.Log(randomTile.position);
        }
        //spawn warning
        Material tileMat = randomTile.GetComponent<Renderer>().material;
        Color inialColor = Color.white;//tileMat.color;  
        Color spawnColor = Color.cyan;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(inialColor, spawnColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1f));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWawe.moveSpeed, currentWawe.hitsToKillPlayer, currentWawe.enemyHealth, currentWawe.enemyColor);
    }

    private void NextWave()
    {
        _currentWaweNumber++;
        if (_currentWaweNumber - 1 < wawes.Length)
        {
            currentWawe = wawes[_currentWaweNumber - 1];
            _enemiesToSpawn = currentWawe.enemyCount;
            _enemiesRamainingAlive = _enemiesToSpawn;
            //if(OnNewWave != null)
            //{
            //    OnNewWave(_currentWaweNumber);
            //}
        }
        else
            Debug.Log("you won nothing!");
    }

    public void ResetPlayerPosition()
    {
        Debug.Log("reset");
        _playerPosition.position = _mapProvider.GetRandomOpenTile().position;
    }

    private void OnPlayerDeath()
    {
        isDiseabled = true;
    }

    private void OnEnemyDeath()
    {
        
        _enemiesRamainingAlive --;
        if (_enemiesRamainingAlive == 0)
        {
            _mapProvider.NextMap();
            //_mapProvider = FindObjectOfType<MapProvider>();
            NextWave();
            _playerHealthComponent.GetComponent<PlayerController>().Respawn(); //turn off PlayerController
        }
    }

    [System.Serializable]
    public class Wawe
    {
        public int enemyCount;
        public float timeBetwenSpawn;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public int enemyHealth;
        public Color enemyColor;
        public bool infinite;
    }
}


//private void SpawnPlayer()
//{
//    if (_playerPrefab != null)
//    {
//        HealthComponent _playerInstance = Instantiate(_playerPrefab, _mapGenerator.GetRandomOpenTile());
//        _playerHealthComponent = _playerInstance.GetComponent<HealthComponent>();
//        _playerPosition = _playerHealthComponent.transform;
//        campPositionOld = _playerHealthComponent.transform.position;
//    }
//    else
//        Debug.Log("Missing player prefab");
//}