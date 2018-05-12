using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectIfPlayerIsInKart : MonoBehaviour
{
    public static PlayAreaFollowKart playerPlayArea; //The player's actual play area

    public static SteamVR_Camera playerHead; //The player's head collider
    public DetectDoorClosing leftDoorDetector; //The detector to detect if left door is closed
    public DetectDoorClosing rightDoorDetector; //The detector to detect if right door is closed
    public bool playerIsInKart; //If the player is inside the kart

    // Use this for initialization
    void Start()
    {
        playerPlayArea = FindObjectOfType<PlayAreaFollowKart>();
        playerHead = FindObjectOfType<SteamVR_Camera>();
        playerIsInKart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPlayArea == null)
        {
            playerPlayArea = FindObjectOfType<PlayAreaFollowKart>();
            playerHead = FindObjectOfType<SteamVR_Camera>();
        }
    }

    public void getInKart() //Place the player in the right position in the kart
    {
        //print(transform.parent.name);
        playerPlayArea.playerOffset = new Vector3(playerHead.transform.localPosition.x, 0, playerHead.transform.localPosition.z); //Record player's current position relative to the play area (where is the player within the play area now)
        playerPlayArea.doFollow = true;
        //GameManager.gameStartProc();
    }

    public void OnTriggerStay(Collider other)
    {
        if (!playerPlayArea.doFollow && other.tag == "MainCamera") //If the trigger is player's head
        {
            getInKart();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MainCamera") //If the trigger is player's head
        {
            getInKart();
        }

        //if (!leftDoorDetector.isDoorClosed) //If the left door is opened
        //{
        //    if (other.tag == "MainCamera") //If the trigger is player's head
        //    {
        //        if (transform.InverseTransformPoint(other.transform.position).x < 0) //Means the player is on the left side of the kart
        //        {
        //            playerIsInKart = true;
        //        }
        //    }
        //}

        //if (!rightDoorDetector.isDoorClosed) //If the right door is opened
        //{
        //    if (other.tag == "MainCamera") //If the trigger is player's head
        //    {
        //        if (transform.InverseTransformPoint(other.transform.position).x > 0) //Means the player is on the right side of the kart
        //        {
        //            playerIsInKart = true;
        //        }
        //    }
        //}

        //if (other.tag == "MainCamera")
        //{
        //    //print("headEnter");
        //    playerPlayArea.playerOffset = new Vector3(playerHead.transform.localPosition.x, 0, playerHead.transform.localPosition.z);
        //    playerPlayArea.doFollow = true;
        //}
    }

    //public void OnTriggerStay(Collider other)
    //{
    //    if (other.tag == "MainCamera")
    //    {
    //        playerPlayArea.doFollow = true;
    //    }
    //}

    public void OnTriggerExit(Collider other)
    {
        // Do not let player get off the cart
        if (GameManager.gameManager.alwaysSyncPlayAreaWithCart)
        {
            //return;
        }

        if (other.tag == "MainCamera")
        {
            playerIsInKart = false;
            playerPlayArea.doFollow = false;
            playerPlayArea.areaToKartDistSet = false;
        }
        //if (!leftDoorDetector.isDoorClosed) //If the left door is opened
        //{
        //    if (other.tag == "MainCamera") //If the trigger is player's head
        //    {
        //        if (transform.InverseTransformPoint(other.transform.position).x < 0) //Means the player is on the left side of the kart
        //        {
        //            playerIsInKart = false;
        //        }
        //    }
        //}

        //if (!rightDoorDetector.isDoorClosed) //If the right door is opened
        //{
        //    if (other.tag == "MainCamera") //If the trigger is player's head
        //    {
        //        if (transform.InverseTransformPoint(other.transform.position).x > 0) //Means the player is on the right side of the kart
        //        {
        //            playerIsInKart = false;
        //        }
        //    }
        //}
    }
}
