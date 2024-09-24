using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject heavyEnemyPrefab;
    public float spawnRange = 9;
    public int spawnWait = 3;
    private int heavyCount = 0;
    private int waveCount = 0;
    private int spawnCount = 3;
    public int enemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyAtRandLoc();
        //StartCoroutine(NewWaveTimer(spawnWait));
    }

    // Update is called once per frame
    void Update()
    { 
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0 )
        {
            waveCount++;
            SpawnWave(spawnCount);
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
        
        //StartCoroutine(NewWaveTimer(spawnWait));
    }

    void SpawnEnemyAtRandLoc()
    {
        Instantiate(enemyPrefab, GenerateSpawnPos(), enemyPrefab.transform.rotation);
    }

    void SpawnHeavyEnemyAtRandLoc()
    {
        Instantiate(heavyEnemyPrefab, GenerateSpawnPos(), heavyEnemyPrefab.transform.rotation);
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
            if (playerController.powerUp == null)
            {
                int numRand = Random.Range(0, 2);
                switch (numRand)
                {
                    case 0:
                        playerController.HandlePowerUpSpawn();
                        break;
                    case 1:
                        playerController.HandleBulletPowerUpSpawn();
                        break;
                }
                    
                
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
