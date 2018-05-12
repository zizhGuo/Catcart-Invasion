using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class SimpleEnemyBehavior : MonoBehaviour
{
    public float moveSpeed;
    public GameObject explosionParticle;
    public GameObject actualModel; // The actual enemy model which is its child

    public GameObject playerKart;
    public GameObject playerHead;
    public GameObject catBasket;
    public Quaternion standQuat;
    public Vector3 standRota;
    public bool isSlapped; // Is the enemy been slapped by the player's melee
    public Vector3 slapForce; // The enemie's velocity when it's been slapped
    public GameObject targetCat; // The cat the enemy is looking to catch
    public bool hasCat; // If the enemy caught cat
    public Vector3 fleeDirection; // Which direction the enemy is fleeing after it caught a cat
    public Vector3 startCatchPosition; // The position where the enemy start to go catch the cat (When the enemy just reached the basket)
    public float startCatchTime; // The time where the enemy start to go catch the cat (When the enemy just reached the basket)
    public Vector3 spawnPosition; // The position where the enemy spawned
    public float spawnTime; // The time the enemy spawned

    // Use this for initialization
    void Start()
    {
        //moveSpeed *= GameManager.sSpeedMultiplier;

        playerKart = FindObjectOfType<GameManager>().playerKart;
        playerHead = FindObjectOfType<GameManager>().playerHead;
        catBasket = FindObjectOfType<GameManager>().catBasket;
        standRota = Vector3.zero;
        moveSpeed *= ((betterRandom(1000, 2500)) / 1000f);
        isSlapped = false;
        hasCat = false;
        fleeDirection = transform.position;
        spawnPosition = playerKart.transform.InverseTransformPoint(transform.position);
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
            //transform.position = playerKart.transform.TransformPoint(spawnPosition * 
            //                                                         (1 - (Time.time - spawnTime) / 
            //                                                              (Vector3.Distance(playerKart.transform.TransformPoint(spawnPosition), playerKart.transform.position) / moveSpeed) // Calculate how much time does it take for the drone to reach the player
            //                                                         )
            //                                                        ); // Move the drone towards the player

            transform.LookAt(playerHead.transform);
        }

        if (!isSlapped && targetCat != null && !hasCat) // If the enemy already near the basket, selected a cat to catch, but didn't catch it yet
        {
            if(!catBasket.GetComponent<KartBasket>().cats.Contains(targetCat)) // If the target cat is already gone for some reason
            {
                targetCat = catBasket.GetComponent<KartBasket>().cats[catBasket.GetComponent<KartBasket>().cats.Count - 1];
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                startCatchPosition = targetCat.transform.InverseTransformPoint(transform.position);
                startCatchTime = Time.time;
            }

            transform.LookAt(targetCat.transform.position + (Vector3.up * 0.5f));
            //GetComponent<Rigidbody>().velocity = (transform.forward.normalized) * (moveSpeed * 0.3f + playerKart.GetComponent<Rigidbody>().velocity.magnitude);
            transform.position = targetCat.transform.TransformPoint(startCatchPosition * (1 - (Time.time - startCatchTime) / 2f));

            if (Vector3.Distance(transform.position, targetCat.transform.position) <= 0.2f) // If the enemy is close enough to the cat then it will catch the cat
            {
                hasCat = true;
                catBasket.GetComponent<KartBasket>().cats.Remove(targetCat);
                targetCat.GetComponent<BoxCollider>().enabled = false;
                targetCat.GetComponent<CatStayInKart>().isInBasket = false;
                transform.LookAt(fleeDirection);
            }
        }

        if (hasCat)
        {
            transform.LookAt(fleeDirection);
            targetCat.transform.position = transform.position - (Vector3.up / 2f);
            GetComponent<Rigidbody>().velocity = (transform.forward.normalized) * ((moveSpeed + GameManager.kartMovementInfo.currentSpeed) * 1.25f + playerKart.GetComponent<Rigidbody>().velocity.magnitude * 0.2f);
        }
    }

    void FixedUpdate()
    {
        if (!isSlapped && targetCat == null && !hasCat) // If the enemy is not near the basket and didn't catch a cat
        {
            transform.LookAt(catBasket.transform//.position
                                                .TransformPoint(Vector3.up * 1f)
                             + (playerKart.GetComponent<Rigidbody>().velocity * //Time.fixedUnscaledDeltaTime *
                                (Mathf.Clamp01(Vector3.Distance(catBasket.transform.position, transform.position) * 0.75f /
                                               ((moveSpeed + GameManager.kartMovementInfo.currentSpeed) // enemy's speed
                                                 + playerKart.GetComponent<Rigidbody>().velocity.magnitude // kart's speed
                                               )
                                              )
                                )
                               )
                            );

            GetComponent<Rigidbody>().velocity = (transform.forward.normalized) * (moveSpeed + GameManager.kartMovementInfo.currentSpeed);

            //print(catBasket.transform.TransformPoint(Vector3.up * 1.55f));
            if (Vector3.Distance(transform.position, catBasket.transform.position + (Vector3.up * 1.55f)) <= 0.5f) // If the enemy is near the cat basket
            {
                targetCat = catBasket.GetComponent<KartBasket>().cats[catBasket.GetComponent<KartBasket>().cats.Count - 1];
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                startCatchPosition = targetCat.transform.InverseTransformPoint(transform.position);
                startCatchTime = Time.time;
            }
        }
    }

    void OnDestroy()
    {
        if(hasCat)
        {
            targetCat.GetComponent<Rigidbody>().velocity = Vector3.zero;
            targetCat.GetComponent<BoxCollider>().enabled = true;
        }

        //GameObject newExplosion = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation); //Create explosion particle effect

        //GameManager.score += 1;
    }

    //void OnCollisionEnter(Collision col)
    //{
    //    if (col.collider.tag == "Player" || col.collider.tag == "HUDwhole" || col.collider.tag == "HUDglass")
    //    {
    //        //GameManager.gameOverProc();
    //        GameManager.score += 1;
    //        Destroy(gameObject);
    //    }
    //}

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
