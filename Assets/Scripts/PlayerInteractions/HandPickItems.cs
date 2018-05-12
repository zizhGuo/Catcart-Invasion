using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class HandPickItems : MonoBehaviour
{
    /// <summary>
    /// This script is attached to the controllers
    /// </summary>
    public GameObject playerPistolModel; // The weapon model attached on the controllers
    public GameObject laserPointer;
    public GameObject mirroredWeapon;
    public GameObject mirroredLaserPointer;

    public GameObject currentItem; // The current item hold by the player
    public string currentItemName; // The name of the current item
    public bool isWeapon; // If the item is weapon
    public bool isLaser; // If the item is laser pointer
    public GameObject playerKart; // The player side kart
    public GameObject nonPlayerKart; // The non player side kart
    public VRTK_ControllerActions controllerActions; // The controller it corresponding to
    public bool isLeftHand; // If this controller is left hand, else is right hand
    public PlayerUI playerUI; // The player's HUD UI

    // Use this for initialization
    void Start()
    {
        //GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(DoGripClicked);
        GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwpClicked);
        isWeapon = false;
        isLaser = false;

        //if (FindObjectOfType<MirrorGameManager>())
        //{
        //    playerKart = FindObjectOfType<MirrorGameManager>().playerKart;
        //    nonPlayerKart = FindObjectOfType<MirrorGameManager>().nonPlayerKart;
        //}

        //else
        {
            playerKart = FindObjectOfType<GameManager>().playerKart;
            nonPlayerKart = FindObjectOfType<GameManager>().nonPlayerKart;
        }

        controllerActions = GetComponent<VRTK_ControllerActions>();
        playerUI = FindObjectOfType<GameManager>().playerUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (name == "LeftController")
        {
            isLeftHand = true;
        }
        else
        {
            isLeftHand = false;
        }

        if (playerPistolModel == null)
        {
            playerPistolModel = transform.parent.Find("catgun_FBX").gameObject;
        }

        if (laserPointer == null)
        {
            laserPointer = transform.parent.Find("LaserPointer").gameObject;
        }

        if (currentItem != null && GetComponent<VRTK_InteractGrab>().enabled)
        {
            if (currentItemName == "CatPawStick")
            {
                grabbedCatPawStick();
            }
            else
            {
                GetComponent<VRTK_InteractGrab>().enabled = false;
                GetComponent<VRTK_InteractTouch>().enabled = false;
            }

            if (currentItemName == "PlayerWeapon")
            {
                grabbedWeapon();
            }

            if (currentItemName == "LaserPointer" || currentItemName == "LaserPointer_Temp")
            {
                grabbedLaser();
            }
        }

        if (currentItem != null)
        {
            if (currentItemName == "PlayerWeapon")
            {
                mirroredWeapon.transform.position = nonPlayerKart.transform.TransformPoint(playerKart.transform.InverseTransformPoint(playerPistolModel.transform.position));
                mirroredWeapon.transform.rotation = playerPistolModel.transform.rotation;
            }

            if (currentItemName == "LaserPointer" || currentItemName == "LaserPointer_Temp")
            {
                mirroredLaserPointer.transform.position = nonPlayerKart.transform.TransformPoint(playerKart.transform.InverseTransformPoint(laserPointer.transform.position));
                mirroredLaserPointer.transform.rotation = laserPointer.transform.rotation;
            }
        }
    }

    public void DoButtonTwpClicked(object sender, ControllerInteractionEventArgs e)
    {
        //print("GripClicked");
        dropItem();

        //if (currentItem != null)
        //{
        //    if (currentItemName == "PlayerWeapon")
        //    {
        //        unGrabbedWeapon();
        //    }

        //    if (currentItemName == "LaserPointer" || currentItemName == "LaserPointer_Temp")
        //    {
        //        unGrabbedLaser();
        //    }

        //    currentItem.SetActive(true);
        //    currentItem.GetComponent<ItemLeaveHand>().StopGrab(true);
        //    currentItem = null;
        //    currentItem.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.position = nonPlayerKart.transform.TransformPoint(playerKart.transform.InverseTransformPoint(transform.position));
        //    currentItem.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.rotation = transform.rotation;

        //    GetComponent<VRTK_InteractGrab>().enabled = true;
        //    GetComponent<VRTK_InteractTouch>().enabled = true;
        //}
    }

    public void dropItem()
    {
        if (currentItem != null)
        {
            //print("drop item");
            controllerActions.TriggerHapticPulse(0.5f, 0.3f, 0.01f);

            if (currentItemName == "PlayerWeapon")
            {
                unGrabbedWeapon();
            }

            if (currentItemName == "LaserPointer" || currentItemName == "LaserPointer_Temp")
            {
                unGrabbedLaser();
            }

            if (currentItemName == "CatPawStick")
            {
                unGrabbedCatPawStick();
            }

            if (isLeftHand)
            {
                ChangeHandIcon(isLeftHand, "EMPTY");
                GetComponent<PlayerHandGestureController>().ChangeGesture(0);
            }
            else
            {
                ChangeHandIcon(isLeftHand, "EMPTY");
                GetComponent<PlayerHandGestureController>().ChangeGesture(0);
            }

            currentItem.SetActive(true);

            // Enable any colliders in the non-player copy
            foreach (Collider c in currentItem.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponentsInChildren<Collider>())
            {
                c.enabled = true;
            }

            currentItem.GetComponent<ItemLeaveHand>().StopGrab(true);
            currentItem.GetComponent<StartTouchingFeedback>().lastUngrabTime = Time.time;
            currentItem = null;

            GetComponent<VRTK_InteractGrab>().enabled = true;
            if (currentItemName == "CatPawStick")
            {
                GetComponent<VRTK_InteractGrab>().ForceRelease(true);
            }
            else
            {
                GetComponent<VRTK_InteractTouch>().enabled = true;
            }
        }
    }

    public void grabbedCatPawStick()
    {

    }

    public void unGrabbedCatPawStick()
    {

    }

    /// <summary>
    /// Change the icon on the HUD UI for the corresponding hand
    /// </summary>
    /// <param name="leftHand"></param>
    /// <param name="item"></param>
    public void ChangeHandIcon(bool leftHand, string item)
    {
        if (leftHand)
        {
            // Deactivate all the icons
            playerUI.leftHandCat.SetActive(false);
            playerUI.leftHandWeapon.SetActive(false);
            playerUI.leftHandLaserPointer.SetActive(false);

            if (item == "CATILIZER")
            {
                playerUI.leftHandWeapon.SetActive(true);
                playerUI.leftHandPistol.SetActive(true);
            }
            if (item == "LASER POINTER")
            {
                playerUI.leftHandLaserPointer.SetActive(true);
            }
        }
        else
        {
            // Deactivate all the icons
            playerUI.rightHandCat.SetActive(false);
            playerUI.rightHandWeapon.SetActive(false);
            playerUI.rightHandLaserPointer.SetActive(false);

            if (item == "CATILIZER")
            {
                playerUI.rightHandWeapon.SetActive(true);
                playerUI.rightHandPistol.SetActive(true);
            }
            if (item == "LASER POINTER")
            {
                playerUI.rightHandLaserPointer.SetActive(true);
            }
        }
    }

    public void grabbedWeapon()
    {
        //GetComponentInParent<OpenCloseShield>().enabled = true;
        //GetComponentInParent<SwitchToMelee>().enabled = true;
        //GetComponent<CustomControllerEvents>().enabled = true;
        GetComponent<VRTK_InteractUse>().enabled = true;

        GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoTriggerClicked);
        GetComponent<VRTK_ControllerEvents>().TriggerUnclicked += new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoTriggerUnclicked);
        GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoGripClicked);
        GetComponent<VRTK_ControllerEvents>().GripReleased += new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoGripUnclicked);
        GetComponent<PlayerCatilizerControl>().hasWeapon = true;
        GetComponent<PlayerCatilizerControl>().justPicked = true;

        if (isLeftHand)
        {
            ChangeHandIcon(isLeftHand, "CATILIZER");
        }
        else
        {
            ChangeHandIcon(isLeftHand, "CATILIZER");
        }

        GetComponent<PlayerCatilizerControl>().GrabbedWeapon();
    }

    public void unGrabbedWeapon()
    {
        //GetComponentInParent<OpenCloseShield>().enabled = false;
        //GetComponentInParent<SwitchToMelee>().enabled = false;
        GetComponent<VRTK_InteractUse>().enabled = false;

        GetComponent<VRTK_ControllerEvents>().TriggerClicked -= new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoTriggerClicked);
        GetComponent<VRTK_ControllerEvents>().TriggerUnclicked -= new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoTriggerUnclicked);
        GetComponent<VRTK_ControllerEvents>().GripPressed -= new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoGripClicked);
        GetComponent<VRTK_ControllerEvents>().GripReleased -= new ControllerInteractionEventHandler(GetComponent<PlayerCatilizerControl>().DoGripUnclicked);
        GetComponent<PlayerCatilizerControl>().hasWeapon = false;

        //currentItem.transform.position = playerPistol.transform.position;
        //currentItem.transform.rotation = playerPistol.transform.rotation;
        //currentItem.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.position = nonPlayerKart.transform.TransformPoint(playerKart.transform.InverseTransformPoint(currentItem.transform.position));
        //currentItem.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.rotation = currentItem.transform.rotation;

        GetComponent<PlayerCatilizerControl>().UngrabbedWeapon();
        //playerPistolModel.SetActive(false);
    }

    public void grabbedLaser()
    {
        //GetComponent<KartFollowLaserSpot>().enabled = true;
        GetComponent<VRTK_StraightPointerRenderer>().enabled = true;
        GetComponent<KartFollowLaserSpot>().isGrabbed = true;
        GetComponent<KartFollowLaserSpot>().grabLaserPointer();

        if (isLeftHand)
        {
            ChangeHandIcon(isLeftHand, "LASER POINTER");
        }
        else
        {
            ChangeHandIcon(isLeftHand, "LASER POINTER");
        }

        laserPointer.SetActive(true);
    }

    public void unGrabbedLaser()
    {
        GetComponent<KartFollowLaserSpot>().isGrabbed = false;
        GetComponent<KartFollowLaserSpot>().isLaserActiveAndGuiding = false;
        GetComponent<KartFollowLaserSpot>().laserPointerReady = false;
        GetComponent<KartFollowLaserSpot>().unGrabLaserPointer();
        GetComponent<VRTK_StraightPointerRenderer>().enabled = false;

        //currentItem.transform.position = laserPointer.transform.position;
        //currentItem.transform.rotation = laserPointer.transform.rotation;
        //currentItem.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.position = nonPlayerKart.transform.TransformPoint(playerKart.transform.InverseTransformPoint(currentItem.transform.position));
        //currentItem.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.rotation = currentItem.transform.rotation;

        laserPointer.SetActive(false);
    }
}
