using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class LevelEndProcess : MonoBehaviour
{
    public Collider[] catCanonTriggers; // The triggers that will trigger the cat canon to fire
    public LinearObjectMovement endRoomGate; // The end room gate

    public int remainingCatCount; // How many cats are left when the player reached the end
    public int finalScore; // What is the player's score when reaching the end
    public float finalTime; // How long does it take from the player from the start to the end trigger
    public KartFollowLaserSpot leftHandLaserPointerController; // The controller for the laser pointer if it is in the left hand
    public KartFollowLaserSpot rightHandLaserPointerController; // The controller for the laser pointer if it is in the right hand

    // Use this for initialization
    void Start()
    {
        leftHandLaserPointerController = GameManager.sLeftController.GetComponent<KartFollowLaserSpot>();
        rightHandLaserPointerController = GameManager.sRightController.GetComponent<KartFollowLaserSpot>();
    }

    // Update is called once per frame
    void Update()
    {
        // Slow the cart's top speed down when the current speed is low enough
        if (GameManager.gameFinished &&
            GameManager.sSpeedMultiplier != 2 &&
            GameManager.kartMovementInfo.currentSpeed <= 10f)
        {
            GameManager.sSpeedMultiplier = 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameFinish();
    }

    public void GameFinish()
    {
        GameManager.gameFinished = true;
        GameManager.kartMovementInfo.isKartShocked = true;
        GameManager.kartMovementInfo.minSpeedWhenShocked = 0;
        GameManager.kartMovementInfo.shockLastingTime = 3;
        GameManager.kartMovementInfo.lastShockedTime = Time.time;
        // Record the final cat count
        remainingCatCount = GameManager.gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().catCount; 
        finalScore = GameManager.score; // Record the final score
        finalTime = Time.time - GameManager.gameStartTime; // Record the final time
        BGMController.bgmController.ChangeBGM(
            BGMController.bgmController.tutorialBGMIntro, BGMController.bgmController.tutorialBGMLoop); // Change BGM back to tutorial

        // Turn on the exact number of pairs of cat canon based on how many cat are left in the cart
        for (int i = 0; i < remainingCatCount; i++)
        {
            catCanonTriggers[i].enabled = true;
        }

        // Disable grabbing the cats
        foreach (PlayerCatStayInBasket c in GameManager.gameManager.cats)
        {
            if (c.GetComponent<StartTouchingFeedback>())
            {
                c.GetComponent<StartTouchingFeedback>().isGrabbable = false;
            }
        }

        // Record missing cats
        foreach (CatInfo catInfo in GameManager.catsInfo.catsInfo)
        {
            bool safe = false;

            // If the cat name can be found in the remaining cats, then mark it as safe
            foreach (GameObject cat in GameManager.gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().cats)
            {
                if (catInfo.catName == cat.GetComponent<NonPlayerSideMirror>().playerSideCopy.GetComponent<InteractWithCat>().catName)
                {
                    safe = true;
                }
            }

            // If the name is not found in the remaining cats, then put the info in the missing cats
            if (!safe)
            {
                GameManager.missingCats.Add(catInfo);
            }
        }

        // Stop the player from turn off the laser pointer
        leftHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Undefined;
        rightHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Undefined;
        leftHandLaserPointerController.CallSubscribeActivationButton();
        rightHandLaserPointerController.CallSubscribeActivationButton();

        // Stop the player from drop the weapon or laser pointer
        leftHandLaserPointerController.GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed -=
            new ControllerInteractionEventHandler(leftHandLaserPointerController.GetComponent<HandPickItems>().DoButtonTwpClicked);
        rightHandLaserPointerController.GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed -=
            new ControllerInteractionEventHandler(rightHandLaserPointerController.GetComponent<HandPickItems>().DoButtonTwpClicked);

        StartCoroutine(endRoomGate.Animate());
    }
}
