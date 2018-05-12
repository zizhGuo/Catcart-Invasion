using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerSpawner : MonoBehaviour {

    public GameObject prefabToBeSpawned;
    public float spawnDepth = 6;                //depth below zero where the driller should be spawned
    public bool jumpScare = false;                      //is it a jump scare type driller or not
    public float distanceBetweenDustParticles = 1;      //to define the frequency of the dust particle effects when the driller moves underground
    public bool followFishingRod = false;               //If true, the driller will follow the fishing rod under the ground, this overrides following the axes
    public bool followX = true;                         //If true, then the driller copies x coordinate of the playerkart underground, if false, then it copies the z coordinate

    private bool isActive;                      //will the spawner spawn when it comes in contact with the player's fishing rod
    private GameObject driller;                 //variable to store the newly spawned driller
    
	// Use this for initialization
	void Start () {

        isActive = true;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (isActive == true)
        {
            if (other.tag == "FishingRod")
            {
                driller = Instantiate(prefabToBeSpawned, other.transform.position + new Vector3(0, -spawnDepth, 0), other.transform.rotation * Quaternion.Euler(0, 180, 0));
                driller.GetComponent<DrillerJumpBehavior>().followPointBeforeSpawning = other.gameObject;
                driller.GetComponent<DrillerJumpBehavior>().spawnDepth = spawnDepth;
                driller.GetComponent<DrillerJumpBehavior>().distanceBetweenDustParticles = distanceBetweenDustParticles;
                driller.GetComponent<DrillerJumpBehavior>().followX = followX;
                driller.GetComponent<DrillerJumpBehavior>().followFishingRod = followFishingRod;

                TestDestroyAfterTraversal.listManager.destroyList.Add(driller);               //Adding the spawned enemy to the list of enemies spawned

                isActive = false;

                if(jumpScare == true)
                {
                    driller.GetComponent<DrillerJumpBehavior>().shouldComeOut = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(isActive == false && jumpScare == false)
        {
            if(other.tag == "FishingRod")
            {
                driller.GetComponent<DrillerJumpBehavior>().shouldComeOut = true;
            }
        }
    }
}
