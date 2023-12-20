using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wawe[] wawes;
    public Enemy enemy;

    private Wawe currentWawe;
    private int currentWaweNumber;
    private int enemiesToSpawn;
    private int enemiesRamainingAlive;
    private float nextSpawnTime;

    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if (enemiesToSpawn > 0 && Time.time> nextSpawnTime)
        {
            enemiesToSpawn--;
            nextSpawnTime = Time.time + currentWawe.timeBetwenSpawn;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero + Vector3.left*3+ Vector3.back*1, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    private void NextWave()
    {
        currentWaweNumber++;
        if(currentWaweNumber -1 < wawes.Length)
        {
            currentWawe = wawes[currentWaweNumber - 1];
            enemiesToSpawn = currentWawe.enemyCount;
            enemiesRamainingAlive = enemiesToSpawn;
        }
    }

    private void OnEnemyDeath()
    {
        enemiesRamainingAlive --;
        if (enemiesRamainingAlive == 0)
        {
            NextWave();
        }
    }

    [System.Serializable]
    public class Wawe
    {
        public int enemyCount;
        public float timeBetwenSpawn;
    }
}
