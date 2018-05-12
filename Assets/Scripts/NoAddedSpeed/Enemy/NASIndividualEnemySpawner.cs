using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn individual enemies
/// Catcher = Catcher Drone
/// Taser = Taser Shooter Drone
/// Driller = Driller Drone
/// </summary>
public class NASIndividualEnemySpawner : MonoBehaviour
{
    public float distanceToTrigger;
    public GameObject taser;
    public GameObject catcher;
    public GameObject driller;
    //public Vector3 drillerInitialRotation;
    public string nameOfEnemyToSpawn; // The name of the enemy this spawner should spawn
    public bool showTestingMeshRenderer; // Do we show the location of the spawn point for test purpose?

    public GameObject playerKart;
    public GameObject enemyToSpawn;
    public bool spawned;

    private GameObject enemyInstance;           //Instance of the spawned enemy

    // Use this for initialization
    void Start()
    {
        playerKart = FindObjectOfType<GameManager>().playerKart;
        spawned = false;

        if (nameOfEnemyToSpawn == "Catcher")
        {
            enemyToSpawn = catcher;
        }
        else if (nameOfEnemyToSpawn == "Taser")
        {
            enemyToSpawn = taser;
            distanceToTrigger += 100;
        }
        else if (nameOfEnemyToSpawn == "Driller")
        {
            enemyToSpawn = driller;
            distanceToTrigger += 200;
        }

        // Hide spawn points location mesh
        if (!showTestingMeshRenderer)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(playerKart.transform.position, transform.position) <= distanceToTrigger && !spawned)
        {
            spawned = true;
            if (nameOfEnemyToSpawn == "Driller")
            {
                Quaternion spawnRotation = Quaternion.identity;
                spawnRotation.eulerAngles = transform.forward;
                enemyInstance = Instantiate(enemyToSpawn, transform.position, spawnRotation);
                TestDestroyAfterTraversal.listManager.destroyList.Add(enemyInstance);        //Adding the spawned enemy to the list of enemies spawned
            }
            else
            {
                enemyInstance = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
                TestDestroyAfterTraversal.listManager.destroyList.Add(enemyInstance);        //Adding the spawned enemy to the list of enemies spawned
            }
        }
    }
}
