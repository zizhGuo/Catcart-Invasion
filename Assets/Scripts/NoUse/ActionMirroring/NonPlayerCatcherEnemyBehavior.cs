using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCatcherEnemyBehavior : MonoBehaviour
{
    public float moveSpeed;
    
    public GameObject playerHead;
    public GameObject nonPlayerCatBasket;
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
        playerHead = FindObjectOfType<MirrorGameManager>().playerHead;
        nonPlayerCatBasket = FindObjectOfType<MirrorGameManager>().nonPlayerCatBasket;
        standRota = Vector3.zero;
        moveSpeed *= ((BetterRandom.betterRandom(1000, 2500)) / 1000f);
        isSlapped = false;
        hasCat = false;
        fleeDirection = transform.position;
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
        }

        if (!isSlapped && targetCat != null && !hasCat) // If the enemy already near the basket, selected a cat to catch, but didn't catch it yet
        {
            if (!nonPlayerCatBasket.GetComponent<KartBasket>().cats.Contains(targetCat)) // If the target cat is already gone for some reason
            {
                targetCat = nonPlayerCatBasket.GetComponent<KartBasket>().cats[nonPlayerCatBasket.GetComponent<KartBasket>().cats.Count - 1];
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                startCatchPosition = targetCat.transform.InverseTransformPoint(transform.position);
                startCatchTime = Time.time;
            }

            transform.LookAt(targetCat.transform.position + (Vector3.up * 0.5f));
            transform.position = targetCat.transform.TransformPoint(startCatchPosition * (1 - (Time.time - startCatchTime) / 2f));

            if (Vector3.Distance(transform.position, targetCat.transform.position) <= 0.2f) // If the enemy is close enough to the cat then it will catch the cat
            {
                hasCat = true;
                nonPlayerCatBasket.GetComponent<KartBasket>().cats.Remove(targetCat);
                targetCat.GetComponent<BoxCollider>().enabled = false;
                targetCat.GetComponent<CatStayInKart>().isInBasket = false;
                transform.LookAt(fleeDirection);
            }
        }

        if (hasCat)
        {
            transform.LookAt(fleeDirection);
            targetCat.transform.position = transform.position - (Vector3.up / 2f);
            GetComponent<Rigidbody>().velocity = (transform.forward.normalized) * ((moveSpeed + GameManager.kartMovementInfo.currentSpeed) * 1.25f);
        }
    }

    void FixedUpdate()
    {
        if (!isSlapped && targetCat == null && !hasCat) // If the enemy is not near the basket and didn't catch a cat
        {
            transform.LookAt(nonPlayerCatBasket.transform.position);

            GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed;
            
            if (Vector3.Distance(transform.position, nonPlayerCatBasket.transform.position + (Vector3.up * 1.55f)) <= 0.5f) // If the enemy is near the cat basket
            {
                targetCat = nonPlayerCatBasket.GetComponent<KartBasket>().cats[nonPlayerCatBasket.GetComponent<KartBasket>().cats.Count - 1];
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                startCatchPosition = targetCat.transform.InverseTransformPoint(transform.position);
                startCatchTime = Time.time;
            }
        }
    }

    void OnDestroy()
    {
        if (hasCat)
        {
            targetCat.GetComponent<Rigidbody>().velocity = Vector3.zero;
            targetCat.GetComponent<BoxCollider>().enabled = true;
        }
    }
}