using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NASTestEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float initialCD;
    public float minCD;
    public float spawnSpeedIncrease;
    public float spawnRadiusMax;
    public float spawnRadiusMin;
    public float spawnHeightMax;
    public float spawnHeightMin;
    public int maxEnemyCanBe;
    public float maxSpawnAngle;

    private float spawnCD;
    private float lastSpawnTime;
    private int currentWaveEnemyCount;
    public int totalCountOfEnemies;
    public GameObject currentTestEnemy;

    // Use this for initialization
    void Start()
    {
        lastSpawnTime = 0;
        spawnCD = initialCD;
        spawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - GameManager.gameStartTime - lastSpawnTime + ((BetterRandom.betterRandom(0, Mathf.RoundToInt(spawnCD / 4000f * 3000f))) / 1000f) >= spawnCD && currentTestEnemy == null)
        {
            currentWaveEnemyCount = 1;
            StartCoroutine(spawnEnemy());
        }
    }

    public IEnumerator spawnEnemy()
    {
        if (FindObjectOfType<PlayerKartBasket>().catCount <= 0)
        {
            yield break;
        }

        Vector3 newLocation = new Vector3(BetterRandom.betterRandom(-Mathf.RoundToInt(spawnRadiusMax), Mathf.RoundToInt(spawnRadiusMax)), 0,
                                          BetterRandom.betterRandom(-Mathf.RoundToInt(spawnRadiusMax), Mathf.RoundToInt(spawnRadiusMax)));

        int whileStopper = 0;
        while (newLocation.magnitude < spawnRadiusMin || newLocation.magnitude > spawnRadiusMax || Vector3.Angle(transform.forward, newLocation) >= maxSpawnAngle)
        {
            if (whileStopper > 10000)
            {
                break;
            }

            newLocation.x = BetterRandom.betterRandom(-Mathf.RoundToInt(spawnRadiusMax), Mathf.RoundToInt(spawnRadiusMax));
            newLocation.z = BetterRandom.betterRandom(-Mathf.RoundToInt(spawnRadiusMax), Mathf.RoundToInt(spawnRadiusMax));
            whileStopper++;
        }

        newLocation.x += transform.position.x;
        newLocation.z += transform.position.z;
        lastSpawnTime = Time.time - GameManager.gameStartTime;

        if (totalCountOfEnemies >= maxEnemyCanBe)
        {
            //yield break;
        }

        currentTestEnemy = Instantiate(enemyPrefab, new Vector3(newLocation.x, ((BetterRandom.betterRandom(Mathf.RoundToInt(spawnHeightMin * 1000), Mathf.RoundToInt(spawnHeightMax * 1000))) / 1000f), newLocation.z), Quaternion.identity);
        //totalCountOfEnemies++;

        if (BetterRandom.betterRandom(0, 100) > 60 && currentWaveEnemyCount < 0)
        {
            currentWaveEnemyCount++;
            yield return new WaitForSeconds((BetterRandom.betterRandom(500, 1000)) / 1000f);
            StartCoroutine(spawnEnemy());
            yield break;
        }

        if (spawnCD > minCD)
        {
            spawnCD -= spawnSpeedIncrease;
        }
    }
}
