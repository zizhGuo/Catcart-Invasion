using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class HUDLeaveHead : ChangePlayerHandGesture
{
    public VRTK_InteractableObject playerHUD; //The HUD for the player, which is this gameobject itself
    public HeadWearHUD playerHead; //The player's head trigger
    public GameObject HUDmodel; // The HUD model

    public PlayerUI playerUI; // The player's HUD UI

    // Use this for initialization
    void Start()
    {
        playerHUD = GetComponent<VRTK_InteractableObject>();
        playerHead = FindObjectOfType<HeadWearHUD>();
        playerUI = FindObjectOfType<GameManager>().playerUI;
    }

    public override void PrimaryControllerUngrab(GameObject previousGrabbingObject)
    {
        //print(previousGrabbingObject.name);
        //if (previousGrabbingObject.name == "LeftController")
        //{
        //    playerUI.leftHandItem.text = "LEFT HAND" + "\r\n" + "EMPTY";
        //}
        //else
        //{
        //    playerUI.rightHandItem.text = "RIGHT HAND" + "\r\n" + "EMPTY";
        //}

        if (!playerHead.isWearingHUD) //When the HUD is not on player's head
        {
            previousParent = null;
            previousKinematicState = false;

            GetComponent<Rigidbody>().useGravity = true;
        }

        base.PrimaryControllerUngrab(previousGrabbingObject);
    }

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        //print(currentGrabbingObject.name);
        //if (currentGrabbingObject.name == "LeftController")
        //{
        //    playerUI.leftHandItem.text = "LEFT HAND" + "\r\n" + "HUD";
        //}
        //else
        //{
        //    playerUI.rightHandItem.text = "RIGHT HAND" + "\r\n" + "HUD";
        //}

        base.Grabbed(currentGrabbingObject);
    }

    public override void StartTouching(GameObject currentTouchingObject)
    {
        currentTouchingObject.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse(1f, 0.1f, 0.2f);

        base.StartTouching(currentTouchingObject);
    }
}
