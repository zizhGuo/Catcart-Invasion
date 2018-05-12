using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NASTaserShooterEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float initialCD;
    public float minCD;
    public float spawnSpeedIncrease;
    public float spawnRadiusMax;
    public float spawnRadiusMin;
    public int maxEnemyCanBe; // The maximum number of taser enemy can be in any time

    private float spawnCD;
    private float lastSpawnTime;
    private int currentWaveEnemyCount;
    public int totalCountOfEnemies; // The number of taser enemy at this time

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
        if (Time.time - GameManager.gameStartTime - lastSpawnTime + ((BetterRandom.betterRandom(0, Mathf.RoundToInt(spawnCD / 4000f * 3000f))) / 1000f) >= spawnCD)
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
        while (newLocation.magnitude < spawnRadiusMin || newLocation.magnitude > spawnRadiusMax || Vector3.Angle(transform.forward, newLocation) >= 20)
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
            yield break;
        }

        GameObject newTaserShooter = Instantiate(enemyPrefab, new Vector3(newLocation.x, ((BetterRandom.betterRandom(2000, 5000)) / 1000f), newLocation.z), Quaternion.identity);
        newTaserShooter.GetComponent<NASTaserShooterEnemyBehavior>().spawner = this;
        totalCountOfEnemies++;

        if (BetterRandom.betterRandom(0, 100) > 60 && currentWaveEnemyCount < 1)
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
