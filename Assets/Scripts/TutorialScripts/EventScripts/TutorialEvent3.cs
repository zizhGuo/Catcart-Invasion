using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent3 : TutorialEventModel
{
    //public GameObject objectDetector; // The detector that detects if objects is in the CatCart
    //public GameObject laserPointerContainer; // The container for the laser pointer
    //public KartFollowLaserSpot leftHandLaserPointerController; // The controller for the laser pointer if it is in the left hand
    //public KartFollowLaserSpot rightHandLaserPointerController; // The controller for the laser pointer if it is in the right hand
    public GameObject cartStopPositionMarker; // The indicator of where the player should stop his cart
    public TutorialLine tutorialLine; // 8

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the tutorial line finished
        if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLine.line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLine.played && TutorialManager.tutorialProgress == 3)
        {
            tutorialLine.played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLine.waitTime, tutorialLine.line);
        }
    }

    private void OnEnable()
    {
        //objectDetector.SetActive(true);
        //laserPointerContainer.GetComponent<LinearObjectMovement>().pause = false;
        //leftHandLaserPointerController.enabled = true;
        //rightHandLaserPointerController.enabled = true;
        cartStopPositionMarker.SetActive(true);
        TutorialManager.tutorialText.text = "";
    }
}
