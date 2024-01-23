using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wawe[] wawes;
    public Enemy enemy;

    private HealthComponent _playerHealthComponent;
    private Transform _playerPosition;
    private MapGenerator _mapGenerator;
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

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        _playerHealthComponent = FindObjectOfType<PlayerController>();
        _playerPosition = _playerHealthComponent.transform;

        _mapGenerator = FindObjectOfType<MapGenerator>();
        _nextCampCheckTime = _timeBetweenCampaingChecks + Time.time;
        campPositionOld = _playerHealthComponent.transform.position;
        _playerHealthComponent.OnDeath += OnPlayerDeath;

        NextWave();
    }

    private void Update()
    {
        if (!isDiseabled)
        {
            if (Time.time > _nextCampCheckTime)
            {
                _nextCampCheckTime = Time.time + _timeBetweenCampaingChecks;
                isCamping = (Vector3.Distance(_playerPosition.position, campPositionOld) < _campThresholdDistance);
                campPositionOld = _playerPosition.position;
            }
            if ((_enemiesToSpawn > 0 || currentWawe.infinite) && Time.time > _nextSpawnTime)
            {
                _enemiesToSpawn--;
                _nextSpawnTime = Time.time + currentWawe.timeBetwenSpawn;

                StartCoroutine(SpawEnemy());
            }
        }
    }

    IEnumerator SpawEnemy()
    {
        float spawnDelay = 1f;
        float tileFlashSpeed = 4;

        Transform randomTile = _mapGenerator.GetRandomOpenTile();
        if (isCamping)
        {
            randomTile = _mapGenerator.TileFromPosition(_playerPosition.position);
        }
        Material tileMat = randomTile.GetComponent<Renderer>().material;
        Color inialColor = Color.white;//tileMat.color;  
        Color spawnColor = Color.cyan;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(inialColor,spawnColor,Mathf.PingPong(spawnTimer*tileFlashSpeed, 1f));
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
        if(_currentWaweNumber -1 < wawes.Length)
        {
            currentWawe = wawes[_currentWaweNumber - 1];
            _enemiesToSpawn = currentWawe.enemyCount;
            _enemiesRamainingAlive = _enemiesToSpawn;
            if(OnNewWave != null)
            {
                OnNewWave(_currentWaweNumber);
            }
        }
    }

    public void ResetPlayerPosition()
    {
        _playerPosition.gameObject.transform.position = _mapGenerator.TileFromPosition(Vector3.zero).position + Vector3.up * 1;
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
            NextWave();
            ResetPlayerPosition();
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
