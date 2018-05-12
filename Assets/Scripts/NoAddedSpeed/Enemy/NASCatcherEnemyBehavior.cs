using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class NASCatcherEnemyBehavior : MonoBehaviour
{
    public float moveSpeed;
    public float engageDistance;                // How close it should be with the player to start moving back with the player
    public float gazedBehaviorCooldown;         // How long before the catcher can do another gazed behavior before the last time

    public GameObject playerHead;
    public GameObject catBasket;
    public Quaternion standQuat;
    public Vector3 standRota;
    public bool isSlapped;                      // Is the enemy been slapped by the player's melee
    public Vector3 slapForce;                   // The enemy's velocity when it's been slapped
    public GameObject targetCat;                // The cat the enemy is looking to catch
    public bool hasCat;                         // If the enemy caught cat
    public Vector3 fleeDirection;               // Which direction the enemy is fleeing after it caught a cat
    public Vector3 startCatchPosition;          // The position where the enemy start to go catch the cat (When the enemy just reached the basket)
    public float startCatchTime;                // The time where the enemy start to go catch the cat (When the enemy just reached the basket)
    //public Vector3 spawnPosition; // The position where the enemy spawned
    public float spawnTime;                         // The time the enemy spawned
    public GameObject playerKart;                   // The player side kart
    public float timeTillTeleport;                  // How long the drone will fly away before it teleports after it caught the cat
    public float timeCaughtCat;                     // The time the enemy just caught the cat
    public MoveWithPlayerKart moveWithPlayerKart;   // The script made it move with player's kart
    public BossBehavior boss;                       // The boss
    public float lastGazedBehaviorTime;             // The time of the last time the catcher is doing the behavior when it is gazed by the player
    public AudioSource suckCatSound;                // Audio source that plays the suck sound when caught a cat

    // Use this for initialization
    void Start()
    {
        playerHead = GameManager.gameManager.playerHead;
        catBasket = GameManager.gameManager.catBasket;
        playerKart = GameManager.gameManager.playerKart;
        standRota = Vector3.zero;
        moveSpeed *= ((BetterRandom.betterRandom(1000, 2500)) / 1000f);
        isSlapped = false;
        hasCat = false;
        fleeDirection = transform.position - playerKart.transform.position;
        moveWithPlayerKart = GetComponent<MoveWithPlayerKart>();
        moveWithPlayerKart.enabled = false;
        //boss = FindObjectOfType<BossBehavior>();
        suckCatSound = GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (isSlapped)
        {
            GetComponent<Rigidbody>().velocity = slapForce;
        }

        if (!isSlapped && targetCat == null && !hasCat) // If the enemy is not near the basket and didn't catch a cat
        {
            transform.LookAt(playerHead.transform);

            if (Vector3.Distance(transform.position, playerKart.transform.position) <= engageDistance &&
                moveWithPlayerKart.enabled == false)
            {
                moveWithPlayerKart.enabled = true;
                lastGazedBehaviorTime = Time.time - gazedBehaviorCooldown * 0.5f;
            }
        }

        if (!isSlapped && targetCat != null && !hasCat) // If the enemy already near the basket, selected a cat to catch, but didn't catch it yet
        {
            //Vector3 currrentPosition = transform.position;

            if (!catBasket.GetComponent<PlayerKartBasket>().cats.Contains(targetCat)) // If the target cat is already gone for some reason
            {
                targetCat = catBasket.GetComponent<PlayerKartBasket>().cats[catBasket.GetComponent<PlayerKartBasket>().cats.Count - 1];
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                //startCatchPosition = targetCat.transform.InverseTransformPoint(transform.position);
                startCatchPosition = transform.position - targetCat.transform.position;
                startCatchTime = Time.time;
            }

            transform.LookAt(targetCat.transform.position + (Vector3.up * 0.5f));
            transform.position = targetCat.transform.position + (startCatchPosition * (1 - (Time.time - startCatchTime) / 2f));

            if (Vector3.Distance(transform.position, targetCat.transform.position) <= 0.2f) // If the enemy is close enough to the cat then it will catch the cat
            {
                hasCat = true;
                timeCaughtCat = Time.time;
                catBasket.GetComponent<PlayerKartBasket>().cats.Remove(targetCat);
                catBasket.GetComponent<PlayerKartBasket>().nonPlayerBasket.cats.Remove(targetCat.GetComponent<PlayerSideMirror>().nonPlayerSideCopy);
                targetCat.GetComponent<Collider>().enabled = false;
                targetCat.GetComponent<PlayerCatStayInBasket>().isInBasket = false;
                targetCat.GetComponent<PlayerSideMirror>().inKart = false; // If the enemy caught the cat then the cat is no longer in kart
                targetCat.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Collider>().enabled = false;
                targetCat.GetComponent<InteractWithCat>().isCaught = true;
                targetCat.GetComponent<InteractWithCat>().meowSound.PlayOneShot(targetCat.GetComponent<InteractWithCat>().meowClips[4]);
                fleeDirection += Vector3.up * BetterRandom.betterRandom(4, 8); // Make the enemy run away a bit higher
                fleeDirection = playerKart.transform.position + fleeDirection;
                transform.LookAt(fleeDirection);
                suckCatSound.PlayOneShot(suckCatSound.clip); // Play suck cat sound
            }
        }

        if (hasCat)
        {
            //fleeDirection = playerKart.transform.position + (playerKart.transform.forward * 50f) + (Vector3.up * BetterRandom.betterRandom(5, 15));
            //transform.LookAt(fleeDirection);
            //print(targetCat.GetComponent<PlayerSideMirror>().doMirror + " " + targetCat.transform.position);

            //if (!boss.started) // If the catcher is not spawned from boss fight
            //{
            transform.LookAt(playerKart.transform.position + fleeDirection); // So that the enemy will always flee ahead of the player's kart
            //}
            //else
            //{
            //    transform.LookAt(boss.transform.position);
            //}

            GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed * 1.25f;
            targetCat.transform.position = transform.position - (Vector3.up * 0.32f);

            if (Time.time - timeCaughtCat >= timeTillTeleport)// && !boss.started) // If the catcher fleed
            {
                hasCat = false;
                //GetComponent<SmallEnemyDie>().enabled = false;
                Destroy(targetCat.GetComponent<PlayerSideMirror>().nonPlayerSideCopy);
                targetCat.SetActive(false);
                Destroy(gameObject);
            }
            //else if (boss.started)
            //{
            //    if (Vector3.Distance(transform.position, boss.transform.position) <= 5) // If the catcher reached the boss
            //    {
            //        hasCat = false;
            //        //GetComponent<SmallEnemyDie>().enabled = false;
            //        Destroy(targetCat.GetComponent<PlayerSideMirror>().nonPlayerSideCopy);
            //        targetCat.SetActive(false);
            //        Destroy(gameObject);
            //    }
            //}
        }
    }

    void FixedUpdate()
    {
        if (!isSlapped && targetCat == null && !hasCat) // If the enemy is not near the basket and didn't catch a cat
        {
            transform.LookAt(catBasket.transform.TransformPoint(Vector3.up * 1f));

            if (!moveWithPlayerKart.enabled) // If the enemy is far away from player then make it fly faster
            {
                GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed * 10;
            }
            else
            {
                GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed;

                // If the player is looking at the catcher enemy and it can do another gaze behavior
                if (GetComponentInChildren<CheckIfGazed>().isBeingGazed && Time.time - lastGazedBehaviorTime >= gazedBehaviorCooldown)
                {
                    //BehaviorWhenGazed();
                    lastGazedBehaviorTime = Time.time;
                }
            }

            //print(Vector3.Distance(transform.position, catBasket.transform.position + (Vector3.up * 1.55f)));

            if (Vector3.Distance(transform.position, catBasket.transform.position + (Vector3.up * 1.55f)) <= 0.57f) // If the enemy is near the cat basket
            {
                targetCat = catBasket.GetComponent<PlayerKartBasket>().cats[catBasket.GetComponent<PlayerKartBasket>().cats.Count - 1];
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                startCatchPosition = transform.position - targetCat.transform.position;
                startCatchTime = Time.time;
            }
        }
    }

    /// <summary>
    /// What the catcher enemy will do if it is gazed by the player
    /// </summary>
    public void BehaviorWhenGazed()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerKart.transform.position); // Calculate current distance to player

        // Make the drone look at a slightly different angle from the player
        Vector3 newEulerAngles = transform.eulerAngles;
        newEulerAngles.x += BetterRandom.betterRandom(-15, 15);
        newEulerAngles.y += BetterRandom.betterRandom(-15, 15);
        newEulerAngles.z += BetterRandom.betterRandom(-15, 15);
        transform.eulerAngles = newEulerAngles;

        transform.position += transform.forward * distanceToPlayer * 0.5f; // Make the drone teleport towards the player
    }

    void OnDestroy()
    {
        if (hasCat && targetCat != null)
        {
            //targetCat.GetComponent<Rigidbody>().velocity = GameManager.kartCurrentVelocity;
            //targetCat.GetComponent<Rigidbody>().useGravity = true;
            targetCat.GetComponent<Collider>().enabled = true;
            targetCat.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Collider>().enabled = true;
            targetCat.GetComponent<MoveWithPlayerKartAndStop>().Start();
            targetCat.GetComponent<InteractWithCat>().isCaught = false;
        }
    }
}
