using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger3 : TutorialTriggerModel
{
    //public GameObject laserPointer; // The laser pointer

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 2)
        {
            return;
        }

        // If the player picked up the laser pointer with either left or right hand
        //if (GameManager.sLeftController.GetComponent<HandPickItems>().currentItemName == "LaserPointer" ||
        //    GameManager.sLeftController.GetComponent<HandPickItems>().currentItemName == "LaserPointer_Temp" ||
        //    GameManager.sRightController.GetComponent<HandPickItems>().currentItemName == "LaserPointer" ||
        //    GameManager.sRightController.GetComponent<HandPickItems>().currentItemName == "LaserPointer_Temp")
        //{
        //    gameObject.SetActive(false);
        //} 
        // If the player turns on the laser pointer
        if (GameManager.kartMovementInfo != null && GameManager.kartMovementInfo.isLaserActive)
        {
            gameObject.SetActive(false);
        }
    }
}
