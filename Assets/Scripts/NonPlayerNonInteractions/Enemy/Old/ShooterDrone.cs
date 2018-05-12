using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class ShooterDrone : MonoBehaviour
{
    public float speed;
    public int minDistanceFromPlayer;
    public int maxDistanceFromPlayer;
    public float waitTime;          //the time for which the shooter will stop before moving again to a new location
    public float projectileSpeed = 10f;         //Speed of the spawned projectile specified by the designer

    public GameObject explosionParticle;
    public GameObject playerKart;
    public GameObject playerHead;
    public MeshRenderer shooterMesh;
    public GameObject projectilePrefab;

    private Vector3 tempTarget = new Vector3(0, 0, 0);       //a temporary target for the drone to move
    private GameObject tempProjectile;

    private float tempWaitTime;
    private bool isThereRandom;         //to check if the drone has found a new random location relative to the player
    public int movementStatus;         //this is to determine which kind of movement should the drone incorporate
    private Material shooterMat;        //This is to change the color of the material of the model of the shooter drone
    private Vector3 spawnLocation;      //This is the location where the projectile will be spawned in front of the shooter drone
    private Vector3 backwardDirection;      //the direction calculated for the backward movement of the drone

    //predicting the location that the player will be
    private Vector3 whereToShoot;           //the predicted location where the drone will aim with the projectile
    private Vector3 relativePosition;        //position relative to the playerKart
    private float theta;                    //angle between the relative position and the velocity of the player
    private Vector3 tempVelocity;           //temporary variable to store the player's current velocity
    private float a, b, c, delta, t;        //temporary variables to handle the quadratic equations


    // Use this for initialization
    void Start()
    {
        playerKart = FindObjectOfType<GameManager>().playerKart;        //finding the catKart and storing its gameObject, used to calculate swarm positions
        playerHead = FindObjectOfType<GameManager>().playerHead;        //finding the player and storing its gameObject, used to shoot projectile


        isThereRandom = false;
        shooterMat = shooterMesh.material;      //to change the color of shooter drone to make it different from the normal drone

        tempWaitTime = waitTime;

        //0 = movement after spawning, 1 = Stop for specified time, 2 = movement to a new random location within range, 3 = moving back because of player movement
        movementStatus = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerHead.transform);                 //Continuously look at the player

        switch (movementStatus)
        {
            case 0:
                transform.position = Vector3.MoveTowards(transform.position, playerKart.transform.position, speed * Time.deltaTime);
                //print((playerKart.transform.position - transform.position).magnitude);
                if (((playerKart.transform.position - transform.position).magnitude) <= 10)
                {
                    movementStatus = 1;
                }
                break;

            case 1:
                tempWaitTime -= Time.deltaTime;

                //if ((playerKart.transform.position - transform.position).magnitude < 10)
                //{
                //    backwardDirection = playerKart.transform.forward;
                //    transform.position = Vector3.MoveTowards(transform.position, transform.position + backwardDirection, tempVelocity.magnitude * Time.deltaTime);
                //}

                if (tempWaitTime <= 0.0f)
                {
                    //--------predicting where the player will be when the projectile hits-----------------------
                    relativePosition = transform.position - playerHead.transform.position;
                    tempVelocity = playerKart.GetComponent<Rigidbody>().velocity;
                    theta = Vector3.Angle(relativePosition, tempVelocity);      //calculating the angle

                    a = (tempVelocity.magnitude * tempVelocity.magnitude) - (projectileSpeed * projectileSpeed);
                    b = -2 * Mathf.Cos(theta * Mathf.Deg2Rad) * relativePosition.magnitude * tempVelocity.magnitude;
                    c = relativePosition.magnitude * relativePosition.magnitude;
                    delta = Mathf.Sqrt((b * b) - (4 * a * c));
                    t = -(b + delta) / (2 * a);

                    whereToShoot = playerHead.transform.position + (tempVelocity * t);
                    transform.LookAt(whereToShoot);
                    //-------------------------------------------------------------------------------------------

                    spawnLocation = transform.position + transform.forward * 2;
                    tempProjectile = Instantiate(projectilePrefab, spawnLocation, transform.rotation);
                    tempProjectile.GetComponent<ProjectileMovement>().moveSpeed = projectileSpeed;
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
        ShooterSpawner spawner = FindObjectOfType<ShooterSpawner>();
        spawner.totalCountOfEnemies--;
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
