using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject heavyEnemyPrefab;
    public GameObject bossPrefab;

    public float spawnRange = 9;
    public int spawnWait = 3;
    private int heavyCount = 0;
    private int waveCount = 0;
    private int bossWave = 10;
    private int spawnCount = 3;
    public int enemyCount = 0;

    void Start()
    {
        SpawnEnemyAtRandLoc();
    }

    void Update()
    { 
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0 )
        {
            waveCount++;
            if (waveCount != bossWave)
            {
                SpawnWave(spawnCount);
            }
            if (waveCount == bossWave)
            {
                Debug.Log("Wave Count: " + waveCount + "equals?? = " + (waveCount == bossWave));

                SpawnBossAtRandLoc();
            }
            if (waveCount % 3 == 0)
            {
                spawnCount++;
            }
        }
    }

    IEnumerator NewWaveTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log(time);
        SpawnWave(spawnCount);
    }

    void SpawnEnemyAtRandLoc()
    {
        Instantiate(enemyPrefab, GenerateSpawnPos(), enemyPrefab.transform.rotation);
    }

    void SpawnHeavyEnemyAtRandLoc()
    {
        Instantiate(heavyEnemyPrefab, GenerateSpawnPos(), heavyEnemyPrefab.transform.rotation); 
    }

    void SpawnBossAtRandLoc()
    {
        Instantiate(bossPrefab, GenerateSpawnPos(), bossPrefab.transform.rotation);
    }

    private Vector3 GenerateSpawnPos()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    void SpawnWave(int amount)
    {
        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        for (int i = 0; i < amount; i++)
        {
            if (playerController != null)
            {
                if (playerController.powerUp == null)
                {
                    playerController.HandlePowerUpSpawn();
                }
                if (heavyCount % 3 == 0)
                {
                    heavyCount++;
                    SpawnHeavyEnemyAtRandLoc();
                }
                else
                {
                    heavyCount++;
                    SpawnEnemyAtRandLoc();
                }
            }
            
            
        }
    }
}
