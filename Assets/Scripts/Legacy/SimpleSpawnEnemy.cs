using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class SimpleSpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float initialCD;
    public float minCD;
    public float spawnSpeedIncrease;
    public float spawnRadiusMax;
    public float spawnRadiusMin;

    private float spawnCD;
    private float lastSpawnTime;
    private int currentWaveEnemyCount;

	// Use this for initialization
	void Start ()
    {
        lastSpawnTime = 0;
        spawnCD = initialCD;
        spawnEnemy();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Time.time - GameManager.gameStartTime - lastSpawnTime + ((betterRandom(0, Mathf.RoundToInt(spawnCD / 4000f * 3000f))) / 1000f) >= spawnCD)
        {
            currentWaveEnemyCount = 1;
            StartCoroutine(spawnEnemy());
        }
	}

    public IEnumerator spawnEnemy()
    {
        if(FindObjectOfType<KartBasket>().catCount <= 0)
        {
            yield break;
        }

        Vector3 newLocation = new Vector3(betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed)), 0, 
                                          betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed)));

        int whileStopper = 0;
        while(newLocation.magnitude < spawnRadiusMin + 2f * GameManager.kartMovementInfo.currentSpeed || newLocation.magnitude > spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed || Vector3.Angle(transform.forward, newLocation) >= 20)
        {
            if(whileStopper > 10000)
            {
                break;
            }

            newLocation.x = betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed));
            newLocation.z = betterRandom(-Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed), Mathf.RoundToInt(spawnRadiusMax + 2f * GameManager.kartMovementInfo.currentSpeed));
            whileStopper++;
        }

        newLocation.x += transform.position.x;
        newLocation.z += transform.position.z;
        Instantiate(enemyPrefab, new Vector3(newLocation.x, ((betterRandom(2000, 5000)) / 1000f), newLocation.z), Quaternion.identity);
        lastSpawnTime = Time.time - GameManager.gameStartTime;

        if (betterRandom(0, 100) > 60 && currentWaveEnemyCount < 1)
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
