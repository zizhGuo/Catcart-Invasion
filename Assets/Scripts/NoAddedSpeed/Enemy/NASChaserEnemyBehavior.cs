using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NASChaserEnemyBehavior : MonoBehaviour
{
    /// <summary>
    /// Enemy's distance changing with player's acceleration (If the player is slowing down, the enemy will move closer, is player speed up, enemy will move further
    /// Maximum delta distance per sec should be capped (If the player suddenly stopped, the enemy shouldn't suddenly close up), sanme for minimum delta distance
    /// Enemy's furthest distance should also be capped (The player can only be ahead of the enemy a fixed distance)
    /// When the player's speed is 0, the enemy should approach at its maximum speed
    /// Apply acceleration towards the enemy when the player is within its range (The closer the player is to the enemy, the greater the cart accelerate towards the enemy
    /// </summary>

    public float effectRange; // How close the player has to be for it to start attract the cart towards it
    public float attractForceRatio; // The ratio between the attractive force applied on the cart and the distance the cart is within the chaser enemy's range
    public float chaserMaxRelativeSpeed; // The fastest speed the chaser enemy can approach or leave the cart
    public float chaserMaxDisplacement; // The maximum distance the chaser enemy can fall behind the player
    public string axisToCalculateDistance; // The chaser enemy will only look at the difference of one axis of the cart's posision
    public Vector3 chaserInitialRotation; // Which direction the chaser will move
    public float hitDeactivationTime; // How long it will be "deactivated" after been hit by the player's laser
    public float maxCatchUpSpeed; // The chaser can only close up/stay at it's current relative position if the player's velocity is below this value, or the chaser will fall behind no matter how fast the player is moving

    public GameObject playerKart; // Player side cart
    public float playerPositionOnOneAxis; // The player's position on one axis
    public float chaserPositionOnOneAxis; // The chaser's position on one axis
    public float chaserRelativeSpeed; // The chaser's current relative speed to the player
    //public Vector3 chaserRelativeVelocity; // The chaser's current relative velocity to the player
    public bool beenhit; // Has it been hit by the player's laser
    public float lastTimeBeenHit; // The last time it is hit
    public Vector3 initialPosition; // The position when it start chasing

    // Use this for initialization
    void Start()
    {
        transform.eulerAngles = chaserInitialRotation;
        initialPosition = transform.position;

        playerKart = FindObjectOfType<GameManager>().playerKart;
        beenhit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastTimeBeenHit >= hitDeactivationTime && beenhit)
        {
            GetComponent<MoveWithPlayerKart>().enabled = true;
            beenhit = false;
        }

        if (beenhit)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<MoveWithPlayerKart>().enabled = false;
            return;
        }

        if (axisToCalculateDistance == "x") // If it is chasing alone the x axis
        {
            playerPositionOnOneAxis = playerKart.transform.position.x;
            chaserPositionOnOneAxis = transform.position.x;
            //chaserRelativeSpeed = Mathf.Clamp(GameManager.kartDeltaAcceleration.x, -chaserMaxRelativeSpeed, chaserMaxRelativeSpeed);
            transform.position = new Vector3(transform.position.x, initialPosition.y, initialPosition.z);
        }
        else if (axisToCalculateDistance == "z") // If it is chasing alone the z axis
        {
            playerPositionOnOneAxis = playerKart.transform.position.z;
            chaserPositionOnOneAxis = transform.position.z;
            //chaserRelativeSpeed = Mathf.Clamp(GameManager.kartDeltaAcceleration.z, -chaserMaxRelativeSpeed, chaserMaxRelativeSpeed);
            transform.position = new Vector3(initialPosition.x, initialPosition.y, transform.position.z);
        }

        if (Vector3.Dot(transform.forward, GameManager.kartCurrentVelocity) >= 0 && Vector3.Dot(transform.forward, (playerKart.transform.position - transform.position)) >= 0) // If the player is moving away and is ahead of it
        {
            GetComponent<MoveWithPlayerKart>().enabled = true;
        }
        else
        {
            GetComponent<MoveWithPlayerKart>().enabled = false;
        }

        if (Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis) > chaserMaxDisplacement) // If it is behind player at a distance greater than it's maximum displacement range
        {
            transform.position += chaserMaxRelativeSpeed * transform.forward * Time.deltaTime;
        }
        else if (GameManager.currentSpeed <= maxCatchUpSpeed &&
            Vector3.Dot(transform.forward, (playerKart.transform.position - transform.position)) > 0) // If the player's cart's speed is lower than how fast it can catch up and it is behind player
        {
            transform.position += chaserMaxRelativeSpeed * ((maxCatchUpSpeed - GameManager.currentSpeed) / maxCatchUpSpeed) * transform.forward * Time.deltaTime;
        }
        else if (GameManager.currentSpeed > maxCatchUpSpeed &&
                 Vector3.Dot(transform.forward, (playerKart.transform.position - transform.position)) > 0) // If the player's cart's speed is higher than how fast it can catch up and it is behind player
        {
            transform.position -= chaserMaxRelativeSpeed * ((GameManager.currentSpeed - maxCatchUpSpeed) / (GameManager.kartMovementInfo.maximumSpeed - maxCatchUpSpeed)) * transform.forward * Time.deltaTime;
        }
        else if (Vector3.Dot(transform.forward, (playerKart.transform.position - transform.position)) <= 0) // If it is not behind player
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        //if (Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis) >= 0 &&
        //    Vector3.Dot(transform.forward, (playerKart.transform.position - transform.position)) > 0) // If the distance between the chaser and the player is larger than 0 and it is behind player
        //{
        //    if (GameManager.kartCurrentVelocity.magnitude >= 0.1f) // If the player cart is not completely stopped
        //    {
        //        if (Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis) <= chaserMaxDisplacement && 
        //            GetComponent<Rigidbody>().velocity.magnitude <= chaserMaxRelativeSpeed) // If the distance between the chaser and the player is smaller than the maximum distance and the player is moving away and is not moving faster than the maximum relative speed
        //        {
        //            if (GameManager.kartMovementInfo.currentSpeed < maxCatchUpSpeed) // If the player's speed is low
        //            {
        //                GetComponent<Rigidbody>().AddForce(transform.forward * chaserRelativeSpeed, ForceMode.Acceleration);
        //            }
        //            else
        //            {
        //                print("cart highspeed");
        //                GetComponent<Rigidbody>().AddForce(-transform.forward * chaserMaxRelativeSpeed * ((GameManager.kartMovementInfo.currentSpeed - maxCatchUpSpeed) / 
        //                                                                                                  (GameManager.kartMovementInfo.maximumSpeed - maxCatchUpSpeed)), ForceMode.Acceleration);
        //            }
        //        }
        //        else
        //        {
        //            GetComponent<Rigidbody>().velocity = Vector3.zero;
        //            GetComponent<Rigidbody>().AddForce(transform.forward * chaserMaxRelativeSpeed, ForceMode.VelocityChange);
        //        }
        //    }
        //    else
        //    {
        //        print("cart stopped");
        //        GetComponent<Rigidbody>().velocity = transform.forward * chaserMaxRelativeSpeed;
        //    }
        //}
        //else
        //{
        //    GetComponent<Rigidbody>().velocity = Vector3.zero;
        //}

        //print(Mathf.Clamp((effectRange - Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis)), 0, effectRange));
        if (GameManager.currentSpeed > 1) // If the player is moving
        {
            playerKart.GetComponent<Rigidbody>().AddForce(-transform.forward * attractForceRatio *
                                                          ((effectRange - (Mathf.Clamp(Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis), 0, effectRange))) / effectRange) *
                                                          GameManager.kartMovementInfo.acceleration,
                                                         ForceMode.Acceleration); // Drag player's cart towards it
        }
        else
        {
            playerKart.GetComponent<Rigidbody>().AddForce(-transform.forward * attractForceRatio *
                                                             ((effectRange - (Mathf.Clamp(Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis), 0, effectRange))) / effectRange),
                                                            ForceMode.Acceleration); // Drag player's cart towards it
        }
        //GameManager.kartMovementInfo.acceleration = -Mathf.Clamp((effectRange - Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis)), 0, effectRange);

        if (Mathf.Abs(playerPositionOnOneAxis - chaserPositionOnOneAxis) <= effectRange) // If player's cart is within range
        {
            GameManager.kartMovementInfo.beingDraggedByChaser = true;
        }
        else
        {
            GameManager.kartMovementInfo.beingDraggedByChaser = false;
        }

        //if(Vector3.Dot(transform.forward, GetComponent<Rigidbody>().velocity) <= 0) // Prevent chaser from going backward
        //{
        //    print("back!");
        //    GetComponent<Rigidbody>().velocity = Vector3.zero;
        //}
    }
}
