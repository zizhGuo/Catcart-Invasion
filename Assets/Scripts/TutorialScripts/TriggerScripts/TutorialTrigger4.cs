using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger4 : TutorialTriggerModel
{
    public GameObject cartStopPositionMarker; // The indicator of where the player should stop his cart
    public bool laserTurnedOn;
    public TutorialLine tutorialLine; // 9

    // Use this for initialization
    void Start()
    {
        laserTurnedOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 3)
        {
            return;
        }

        // If the player turns on the laser pointer
        if (GameManager.kartMovementInfo != null && GameManager.kartMovementInfo.isLaserActive)
        {
            laserTurnedOn = true;
        }

        // If the cart move to the stop position
        if (laserTurnedOn && GameManager.kartMovementInfo != null
                          //&& !GameManager.kartMovementInfo.isLaserActive
                          //&& GameManager.currentSpeed <= 3
                          && Vector3.Distance(cartStopPositionMarker.transform.position,
                                              GameManager.gameManager.playerKart.transform.position) <= 2)
        {
            laserTurnedOn = false;
            cartStopPositionMarker.SetActive(false); // Hide the marker
            FindObjectsOfType<KartFollowLaserSpot>()[0].canDrive = false; // Disable laser drive
            FindObjectsOfType<KartFollowLaserSpot>()[1].canDrive = false; // Disable laser drive
            GameManager.kartMovementInfo.laserOffStopKart(true); // Stop the cart
            TutorialManager.tutorialText.text = "Click trigger to toggle Laserpointer";

            if (!tutorialLine.played)
            {
                tutorialLine.played = true;
                TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
                TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLine.waitTime, tutorialLine.line);
            }
        }

        if (TutorialManager.tutorialProgress == 3 && // If the tutorial event 3 is played
            !GameManager.kartMovementInfo.isLaserActive && // If the player turned off the laser pointer
            !cartStopPositionMarker.activeInHierarchy) // If the player reached the target location (because the mark is off)
        {
            gameObject.SetActive(false);
        }
    }
}
