using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundExpandTrigger : MonoBehaviour
{
    //public AutoExpandGround ground;
    [HideInInspector]public GameObject groundTile;
    public GameObject rock;
    public int triggerIndex;

    public bool isStepped;
    public GameObject newGround;

    //private NavMeshSurface nav;

	// Use this for initialization
	void Start ()
    {
        isStepped = false;
        groundTile = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isStepped && newGround == null && triggerIndex != 4)
        {
            //print("This index: " + triggerIndex + ", newGround: " + newGround);
            newGround = Instantiate(groundTile, 
                new Vector3(
                transform.position.x + (transform.parent.transform.localScale.x * transform.localPosition.x * 0.98f), 
                transform.position.y, +
                transform.position.z + (transform.parent.transform.localScale.z * transform.localPosition.z * 0.98f)
                ), Quaternion.identity);
            newGround.GetComponent<AutoExpandGround>().expandTriggers[8 - triggerIndex].newGround = transform.parent.gameObject;
            //nav = newGround.GetComponent<NavMeshSurface>();
            //createRocksOnNewGround(newGround);
        }
	}

    public void createRocksOnNewGround(GameObject newGround)
    {
        int rockCount = 0;
        //int rockCount = betterRandom(GameManager.sminRocks, GameManager.smaxRocks)
        //                + Mathf.Clamp(Mathf.FloorToInt(GameManager.distanceTravelled / 50f), 0, GameManager.smaxRockInc);

        HaltonSequence rockHalton = new HaltonSequence();
        float size = 100.0f;
        float minX = 0;
        float maxX = 0;
        float minZ = 0;
        float maxZ = 0;
        Vector3[] rockPosi = new Vector3[rockCount * 2];
        bool[] rockPosiOccupy = new bool[rockCount * 2];

        Vector3 position = new Vector3();
        for (int i = 0; i < rockCount * 2; i++)
        {
            rockHalton.Increment();
            position = rockHalton.m_CurrentPos;
            position.y = 0.0f;
            position *= size;
            rockPosi[i] = position;
            rockPosiOccupy[i] = false;

            if(position.x <= minX)
            {
                minX = position.x;
            }
            if (position.x >= maxX)
            {
                maxX = position.x;
            }
            if (position.z <= minZ)
            {
                minZ = position.z;
            }
            if (position.z >= maxZ)
            {
                maxZ = position.z;
            }
        }

        float xOffset = maxX - ((maxX - minX) / 2f);
        float zOffset = maxZ - ((maxZ - minZ) / 2f);

        for (int i = 0; i < rockCount; i++)
        {
            Vector3 newEuler = new Vector3(BetterRandom.betterRandom(0, 360), BetterRandom.betterRandom(0, 360), BetterRandom.betterRandom(0, 360));
            Quaternion rockRota = new Quaternion();
            rockRota.eulerAngles = newEuler;
            int rockInd = BetterRandom.betterRandom(0, rockPosi.Length - 1);

            while( rockPosiOccupy[rockInd] 
                || rockPosi[rockInd].x - xOffset < -49 
                || rockPosi[rockInd].x - xOffset > 49
                || rockPosi[rockInd].z - zOffset < -49
                || rockPosi[rockInd].z - zOffset > 49)
            {
                if(GameManager.gameOver)
                {
                    return;
                }

                rockInd = BetterRandom.betterRandom(0, rockPosi.Length - 1);
            }

            rockPosiOccupy[rockInd] = true;
            Vector3 newRockPosi = rockPosi[rockInd] + newGround.transform.position;
            newRockPosi.x -= xOffset;
            newRockPosi.z -= zOffset;

            GameObject newRock = Instantiate(rock, newRockPosi, rockRota);
            newRock.GetComponent<Rock>().parentGround = newGround;
        }
        //nav.BuildNavMesh();
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            isStepped = true;
        }

        if(col.tag == "GroundTrigger")
        {
            if(col.GetComponent<GroundExpandTrigger>().triggerIndex + triggerIndex == 8)
            {
                newGround = col.transform.parent.gameObject;
            }
        }
    }

    public void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            isStepped = true;
        }

        if (col.tag == "GroundTrigger")
        {
            if (col.GetComponent<GroundExpandTrigger>().triggerIndex + triggerIndex == 8)
            {
                newGround = col.transform.parent.gameObject;
            }
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            isStepped = false;
        }

        if (col.tag == "GroundTrigger")
        {
            if (col.GetComponent<GroundExpandTrigger>().triggerIndex + triggerIndex == 8)
            {
                newGround = null;
            }
        }
    }
}
