using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultDrillerJumpBehavior : MonoBehaviour {
    public float speedMultiplier = 6;
    public float plantHeight = 5;
    //public GameObject shockAttack;                  //The shock particle effect the driller is going to release
    public GameObject dustParticle;                     //The dust particle prefab to be instantiated when the driller moves underground
    public float waitTimeDustEffect;                    //The delay for having the dust effect after spawning the dust egg
    public GameObject shock; // The trigger that detects the cart and release the shock
    public Transform scope; // The scope on the driller

    private Vector3 whereToTick;                        //The location where the driller should go and wait to be detonated
    private Rigidbody rb;                               //A variable for this gameobject's rigidbody component
    private Vector3 currentDistanceVector;              //The direction in which the driller moves to get into the right position
    private float distance;                             //magnitude of the current Distance Vector
    private float distanceMovedEachFrame;               //Distance moved underground between two frames
    private float tempSumDistanceEachFrame = 0;         //summation of the distance moved each frame, for comparison
    private Vector3 positionLastFrame;                  //position of the driller in the last frame
    private Transform playerKart;                      //holds the reference to the player kart

    [HideInInspector] public bool isItPlanted;                              //Is the driller in place to be detonated
    [HideInInspector] public bool shouldComeOut = true;                    //should the driller come out of the ground or not, this variable is only manipulated by the object that spawned this
    [HideInInspector] public GameObject followPointBeforeSpawning;          //contains the point to follow before jumping out
    [HideInInspector] public float spawnDepth;                              //this value is given by the object that spawned this
    [HideInInspector] public float distanceBetweenDustParticles = 1;        //distance travelled after which spawning the dust effect
    [HideInInspector] public GameObject tempDustEgg;                        //temporary variable to store the next dust egg, to be able to alter certain parameters in the egg
    [HideInInspector] public bool followFishingRod;                         //If true, the driller will follow the fishing rod under the ground, this overrides following the axes
    [HideInInspector] public bool followX;                                  //If true, then the driller copies x coordinate of the playerkart underground, if false, then it copies the z coordinate

    // Use this for initialization
    void Start()
    {
        playerKart = followPointBeforeSpawning.transform.parent.transform;

        whereToTick = transform.position + new Vector3(0, plantHeight, 0);

        positionLastFrame = transform.position;

        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);

        isItPlanted = false;

        GetComponent<EnemyInfoOnUI>().shouldShow = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //transform.rotation = followPointBeforeSpawning.transform.rotation * Quaternion.Euler(0, 180, 0);

        if (isItPlanted == false && shouldComeOut == true)
        {
            if (!GetComponent<EnemyInfoOnUI>().shouldShow)
            {
                GetComponent<EnemyInfoOnUI>().shouldShow = true; // Turn on the enemy UI elements
                shock.SetActive(true); // Active the shock trigger

                // Look at the player
                transform.LookAt(GameManager.gameManager.playerHead.transform.position);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                scope.transform.localEulerAngles = Vector3.zero;
            }

            currentDistanceVector = whereToTick - transform.position;
            distance = currentDistanceVector.magnitude;
            rb.velocity = currentDistanceVector.normalized * distance * speedMultiplier;

            if (distance < 0.05)
            {
                isItPlanted = true;
                rb.velocity = Vector3.zero;
            }

            GetComponent<AudioSource>().Play(); // Play the sound effect when the driller jumps out
        }

        if (isItPlanted == false && shouldComeOut == false)
        {
            positionLastFrame = transform.position;

            if (followFishingRod == true)
            {
                transform.position = new Vector3(followPointBeforeSpawning.transform.position.x, -spawnDepth, followPointBeforeSpawning.transform.position.z);
            }
            else
            {
                if (followX == true)
                {
                    transform.position = new Vector3(playerKart.position.x, -spawnDepth, followPointBeforeSpawning.transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(followPointBeforeSpawning.transform.position.x, -spawnDepth, playerKart.position.z);
                }
            }

            distanceMovedEachFrame = (transform.position - positionLastFrame).magnitude;
            tempSumDistanceEachFrame = tempSumDistanceEachFrame + distanceMovedEachFrame;

            if (tempSumDistanceEachFrame >= distanceBetweenDustParticles)
            {
                tempSumDistanceEachFrame = 0;
                tempDustEgg = Instantiate(dustParticle, new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);
                tempDustEgg.GetComponent<SpawnDrillerDigOutParticle>().waitTime = waitTimeDustEffect;
            }

            whereToTick = transform.position + new Vector3(0, plantHeight, 0);

            // Scope look at the player
            scope.transform.LookAt(GameManager.gameManager.playerHead.transform.position);
            scope.transform.eulerAngles = new Vector3(0, scope.transform.eulerAngles.y, 0);
        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //if(other.tag == "Player")
    //{
    //Instantiate(shockAttack, transform.position, transform.rotation);
    //        Destroy(gameObject);
    //    }
    //}
}
