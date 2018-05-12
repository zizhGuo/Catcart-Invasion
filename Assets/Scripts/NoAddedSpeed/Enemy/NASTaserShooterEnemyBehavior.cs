using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NASTaserShooterEnemyBehavior : MonoBehaviour
{
    public float moveSpeed; // How fast the drone is moving
    public int minDistanceFromPlayer;
    public int maxDistanceFromPlayer;
    public float waitTime;          //the time for which the shooter will stop before moving again to a new location
    public float taserShotMoveSpeed = 10f;         //Speed of the spawned projectile specified by the designer
    public float distanceToEngage; // How close the enemy has to be from the player to start engaging

    public GameObject explosionParticle;
    public GameObject playerKart;
    public GameObject playerHead;
    public MeshRenderer shooterMesh;
    public GameObject projectilePrefab;

    private Vector3 tempTarget = new Vector3(0, 0, 0);       //a temporary target for the drone to move
    private GameObject tempProjectile;

    public float tempWaitTime;
    private bool isThereRandom;         //to check if the drone has found a new random location relative to the player
    public int movementStatus;         //this is to determine which kind of movement should the drone incorporate
    private Material shooterMat;        //This is to change the color of the material of the model of the shooter drone
    private Vector3 spawnLocation;      //This is the location where the projectile will be spawned in front of the shooter drone
    private Vector3 backwardDirection;      //the direction calculated for the backward movement of the drone
    public NASTaserShooterEnemySpawner spawner;
    public MoveWithPlayerKart moveWithPlayerKart; // The script made it move with player's kart

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
        //shooterMat = shooterMesh.material;      //to change the color of shooter drone to make it different from the normal drone
        shooterMat = GetComponentInChildren<Transform>().GetComponentInChildren<MeshRenderer>().material;
        //shooterMat.SetColor("_Color", Color.red);

        tempWaitTime = waitTime;

        //0 = movement after spawning, 1 = Stop for specified time, 2 = movement to a new random location within range, 3 = moving back because of player movement
        movementStatus = 0;
        moveWithPlayerKart = GetComponent<MoveWithPlayerKart>();
        moveWithPlayerKart.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (movementStatus)
        {
            case 0:
                transform.LookAt(playerKart.transform);

                if (!moveWithPlayerKart.enabled) // If the enemy is far away from player then make it fly faster
                {
                    GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed * 10;
                }
                else
                {
                    GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed;
                }
                //transform.position = Vector3.MoveTowards(transform.position, playerKart.transform.position, moveSpeed * Time.deltaTime);
                //print((playerKart.transform.position - transform.position).magnitude);
                if (((playerKart.transform.position - transform.position).magnitude) <= distanceToEngage)
                {
                    moveWithPlayerKart.enabled = true;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    movementStatus = 1;
                }
                break;

            case 1:
                tempWaitTime -= Time.deltaTime;
                GetComponent<Rigidbody>().velocity = Vector3.zero;

                //if ((playerKart.transform.position - transform.position).magnitude < 10)
                //{
                //    backwardDirection = playerKart.transform.forward;
                //    transform.position = Vector3.MoveTowards(transform.position, transform.position + backwardDirection, tempVelocity.magnitude * Time.deltaTime);
                //}

                if (tempWaitTime <= 0.0f)
                {
                    /*--------predicting where the player will be when the projectile hits-----------------------
                    relativePosition = transform.position - playerHead.transform.position;
                    tempVelocity = playerKart.GetComponent<Rigidbody>().velocity;
                    theta = Vector3.Angle(relativePosition, tempVelocity);      //calculating the angle

                    a = (tempVelocity.magnitude * tempVelocity.magnitude) - (taserShotMoveSpeed * taserShotMoveSpeed);
                    b = -2 * Mathf.Cos(theta * Mathf.Deg2Rad) * relativePosition.magnitude * tempVelocity.magnitude;
                    c = relativePosition.magnitude * relativePosition.magnitude;
                    delta = Mathf.Sqrt((b * b) - (4 * a * c));
                    t = -(b + delta) / (2 * a);

                    whereToShoot = playerHead.transform.position + (tempVelocity * t);
                    transform.LookAt(whereToShoot);
                    //-------------------------------------------------------------------------------------------*/

                    transform.LookAt(playerHead.transform);
                    spawnLocation = transform.position + transform.forward * 2;
                    tempProjectile = Instantiate(projectilePrefab, spawnLocation, transform.rotation);
                    tempProjectile.GetComponent<NASTaserShotBehavior>().moveSpeed = taserShotMoveSpeed;
                    tempProjectile.GetComponent<NASTaserShotBehavior>().delayedPlayerVelocity = GameManager.kartLastVelocity;

                    tempWaitTime = waitTime;
                    movementStatus = 2;
                }
                break;

            case 2:
                if (isThereRandom == false)
                {
                    int whileStopper = 0;

                    do
                    {
                        if (whileStopper > 10000)
                        {
                            break;
                        }

                        tempTarget.x = BetterRandom.betterRandom(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                        tempTarget.z = BetterRandom.betterRandom(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                        tempTarget.y = 0;

                        whileStopper++;
                    } while (tempTarget.magnitude < minDistanceFromPlayer || tempTarget.magnitude > maxDistanceFromPlayer || Vector3.Angle(playerKart.transform.forward, tempTarget) >= 15);

                    tempTarget.x += playerKart.transform.position.x;
                    tempTarget.z += playerKart.transform.position.z;
                    tempTarget.y = ((BetterRandom.betterRandom(2000, 5000)) / 1000f);

                    tempTarget = playerKart.transform.InverseTransformPoint(tempTarget);
                    isThereRandom = true;
                }

                //transform.position = Vector3.MoveTowards(transform.position, tempTarget, moveSpeed * Time.deltaTime);
                transform.LookAt(playerKart.transform.TransformPoint(tempTarget));
                GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed;

                if (Vector3.Distance(transform.position, playerKart.transform.TransformPoint(tempTarget)) <= 0.5f)
                {
                    movementStatus = 1;
                    isThereRandom = false;
                }

                break;

            default:
                break;
        }

        transform.LookAt(playerHead.transform);                 //Continuously look at the player
    }

    void OnDestroy()
    {
        NASTaserShooterEnemySpawner spawner = FindObjectOfType<NASTaserShooterEnemySpawner>();

        if (spawner != null)
        {
            spawner.totalCountOfEnemies--;
        }
        //GameObject newExplosion = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation); //Create explosion particle effect.

        //GameManager.score += 1;
    }
}
