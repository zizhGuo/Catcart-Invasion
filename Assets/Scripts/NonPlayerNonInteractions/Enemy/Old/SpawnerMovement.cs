using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class SpawnerMovement : MonoBehaviour {

    public float speed = 10;
    public int minDistanceFromPlayer = 20;
    public int maxDistanceFromPlayer = 30;
    public float waitTime = 5.0f;          //the time for which the shooter will stop before moving again to a new location

    public int maxEnemiesInAWave = 5;               //The designer enters the maximum number of enemies to be spawned in a wave
    [HideInInspector]public int totalCountOfEnemies;             //variables for handling the wave spawning

    public GameObject explosionParticle;
    public GameObject playerKart;
    public GameObject enemyPrefab;          //prefab of the enemy drones assigned by the designer
    //public GameObject playerHead;
    public MeshRenderer shooterMesh;

    private Vector3 tempTarget = new Vector3(0, 0, 0);       //a temporary target for the drone to move
    private GameObject tempEnemy;                           //temporary storage for the instantiated droid

    private float tempWaitTime;             //this time is calculated each frame and checked with waitTime
    private bool isThereRandom;         //to check if the drone has found a new random location relative to the player
    private int movementStatus;         //this is to determine which kind of movement should the drone incorporate
    private Material shooterMat;        //This is to change the color of the material of the model of the shooter drone
    private Vector3 backwardDirection;      //the direction calculated for the backward movement of the drone
    private Vector3 spawnLocation;          //Location where the droids will be spawned
    private Vector3 tempVelocity;           //Storing the player's current velocity


    // Use this for initialization
    void Start () {
        //playerKart = FindObjectOfType<GameManager>().playerKart;        //finding the catKart and storing its gameObject, used to calculate swarm positions
        //playerHead = FindObjectOfType<GameManager>().playerHead;        //finding the player and storing its gameObject, used to shoot projectile

        playerKart = GameObject.FindGameObjectWithTag("Player").gameObject;

        isThereRandom = false;
        shooterMat = shooterMesh.material;      //to change the color of shooter drone to make it different from the normal drone

        tempWaitTime = waitTime;

        //0 = movement after spawning, 1 = Stop for specified time, 2 = movement to a new random location within range, 3 = moving back because of player movement
        movementStatus = 0;

        totalCountOfEnemies = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(playerHead.transform);                 //Continuously look at the player
        transform.LookAt(playerKart.transform);

        switch (movementStatus)
        {
            case 0:
                transform.position = Vector3.MoveTowards(transform.position, playerKart.transform.position, speed * Time.deltaTime);
                //print((playerKart.transform.position - transform.position).magnitude);
                if (((playerKart.transform.position - transform.position).magnitude) <= 25)
                {
                    movementStatus = 1;
                }
                break;

            case 1:
                tempWaitTime -= Time.deltaTime;

                if ((playerKart.transform.position - transform.position).magnitude < 20)
                {
                    backwardDirection = playerKart.transform.forward;
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + backwardDirection, tempVelocity.magnitude * Time.deltaTime);
                }

                if (tempWaitTime <= 0.0f && totalCountOfEnemies < maxEnemiesInAWave)
                {
                    totalCountOfEnemies++;
                    spawnLocation = transform.position + transform.forward * 2;
                    tempEnemy = Instantiate(enemyPrefab, spawnLocation, transform.rotation);
                    tempWaitTime = waitTime;
                    movementStatus = 2;
                }
                break;

            case 2:
                if (isThereRandom == false)
                {
                    //int inc = 0;

                    do
                    {
                        tempTarget.x = betterRandom(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                        tempTarget.z = betterRandom(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                        tempTarget.y = 0;

                        //if(Vector3.Angle(playerKart.transform.forward, tempTarget) >= 15)
                        //{
                        //    inc++;
                        //
                        //    if(inc >= 100)
                        //    {
                        //        print(Vector3.Angle(playerKart.transform.forward, tempTarget) + ", temptarget: " +  tempTarget + ", forward" + playerKart.transform.forward);
                        //        break;
                        //    }
                        //}

                    } while (tempTarget.magnitude < minDistanceFromPlayer || tempTarget.magnitude > maxDistanceFromPlayer || Vector3.Angle(playerKart.transform.forward, tempTarget) >= 15);

                    tempTarget.x += playerKart.transform.position.x;
                    tempTarget.z += playerKart.transform.position.z;
                    tempTarget.y = ((betterRandom(2000, 5000)) / 1000f);
                    isThereRandom = true;
                }
                transform.position = Vector3.MoveTowards(transform.position, tempTarget, speed * Time.deltaTime);
                if (transform.position == tempTarget)
                {
                    movementStatus = 1;
                    isThereRandom = false;
                }

                break;

            default:
                break;
        }
        shooterMat.SetColor("_Color", Color.red);
    }

    void OnDestroy()
    {
        GameObject newExplosion = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation); //Create explosion particle effect.
        GameManager.score += 1;
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

