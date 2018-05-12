using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger1 : TutorialTriggerModel
{
    public CheckIfButtonPressed button; // The button that will trigger this trigger
    //public KartFollowLaserSpot leftHandLaserPointerController; // The controller for the laser pointer if it is in the left hand
    //public KartFollowLaserSpot rightHandLaserPointerController; // The controller for the laser pointer if it is in the right hand
    public GameObject catPawStick; // The cat paw stick
    public TutorialLine[] tutorialLines; // The tutorial lines

    // Use this for initialization
    void Start()
    {
        tutorialLines[0].played = true;
        TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[0].waitTime, tutorialLines[0].line);
        //print("line1");
    }

    // Update is called once per frame
    void Update()
    {
        // If the player picks up the cat paw stick and the tutorial line has not played
        if (!catPawStick.activeInHierarchy && !tutorialLines[1].played)
        {
            tutorialLines[1].played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[1].waitTime, tutorialLines[1].line);
        }

        if (button.buttonDown)
        {
            gameObject.SetActive(false);
        }

        //leftHandLaserPointerController.enabled = false;
        //rightHandLaserPointerController.enabled = false;
    }
}
