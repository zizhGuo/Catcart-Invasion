using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use to repeatly generate new enemies to test
/// </summary>
public class TestEnemy : MonoBehaviour
{
    public float spawnCD; // How long between each spawn

    public float lastSpawnTime; // When is the last spawn

    // Use this for initialization
    void Start()
    {
        lastSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastSpawnTime >= spawnCD)
        {
            GetComponentInChildren<NASIndividualEnemySpawner>().spawned = false;
            lastSpawnTime = Time.time;
        }
    }
}
