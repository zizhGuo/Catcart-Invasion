//This script needs to be attached to a gameObject which exists only to run this script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class ShooterSpawner : MonoBehaviour {

    public GameObject enemyPrefab;              //assign the appropriate drone prefab here
    public float initialCD;                     //Initial cooldown before the spawning of the first drone
    public float minCD;                         //The spawn cooldown is going to lessen after every wave, this is the minimum value fo the cool down
    public float spawnSpeedIncrease;            //This is the value with which the spawn cooldown is gonna decrease after every wave
    public float spawnRadiusMax;                  //The maximum distance away from the player where the drone should be spawned
    public float spawnRadiusMin;                  //The minimum distance away from the player where the drone should be spawned
    public int maxEnemiesInAWave;               //The designer enters the maximum number of enemies to be spawned in a wave
    public int totalEnemyCount;
    public int totalCountOfEnemies;             //variables for handling the wave spawning

    private float spawnCD;                      //This is the cool down before the spawning of consecutive waves
    private float lastSpawnTime;                //This will store the value of the time stamp when the last wave was spawned
    private int currentWaveEnemyCount;          //This stores the number of current enemies in this wave
    

    void Start()
    {
        //spawnRadiusMin *= GameManager.sSpeedMultiplier;
        //spawnRadiusMax *= GameManager.sSpeedMultiplier;

        lastSpawnTime = 0;
        spawnCD = initialCD;                    //assigning the cooldown value given by the designer to the spawning cooldown
        totalCountOfEnemies = 0;
    }

    void Update()
    {
        if(Time.time - GameManager.gameStartTime - lastSpawnTime + ((betterRandom(0, Mathf.RoundToInt(spawnCD / 4000f * 3000f))) / 1000f) >= spawnCD)
        {
            currentWaveEnemyCount = 1;
            StartCoroutine(spawnEnemy());
        }
    }

    public IEnumerator spawnEnemy()
    {
        if (totalCountOfEnemies >= totalEnemyCount)
        {
            yield break;
        }

        Vector3 newLocation = new Vector3(betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed)), 0, 
                                          betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed)));

        int whileStopper = 0;

        while (newLocation.magnitude < spawnRadiusMin + 2f * GameManager.kartMovementInfo.currentSpeed || newLocation.magnitude > spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed || Vector3.Angle(transform.forward, newLocation) >= 30)
        {
            if (whileStopper > 10000)
            {
                break;
            }

            newLocation.x = betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed));
            newLocation.z = betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed));
            whileStopper++;
        }

        newLocation.x += transform.position.x;          //adding the relative location of the drone to the player's location to make it a global value
        newLocation.z += transform.position.z;

        Instantiate(enemyPrefab, new Vector3(newLocation.x, ((betterRandom(2000, 5000)) / 1000f), newLocation.z), Quaternion.identity);
        lastSpawnTime = Time.time - GameManager.gameStartTime;
        totalCountOfEnemies++;

        if (betterRandom(0, 100) > 60 && currentWaveEnemyCount < maxEnemiesInAWave)
        {
            currentWaveEnemyCount++;
            yield return new WaitForSeconds((betterRandom(500, 1000)) / 1000f);
            StartCoroutine(spawnEnemy());
            yield break;
        }

        if (spawnCD > minCD)
        {
            spawnCD -= spawnSpeedIncrease;
        }
    }

    #region Better random number generator 

    private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

    public static int betterRandom(int minimumValue, int maximumValue)
    {
        byte[] randomNumber = new byte[1];

        _generator.GetBytes(randomNumber);

        double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

        // We are using Math.Max, and substracting 0.00000000001,  
        // to ensure "multiplier" will always be between 0.0 and .99999999999 
        // Otherwise, it's possible for it to be "1", which causes problems in our rounding. 
        double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

        // We need to add one to the range, to allow for the rounding done with Math.Floor 
        int range = maximumValue - minimumValue + 1;

        double randomValueInRange = Math.Floor(multiplier * range);

        return (int)(minimumValue + randomValueInRange);
    }
    #endregion
}