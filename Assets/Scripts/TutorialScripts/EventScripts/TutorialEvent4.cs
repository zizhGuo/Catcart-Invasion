using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent4 : TutorialEventModel
{
    public GameObject objectDetector; // The detector that detects if objects is in the CatCart
    public GameObject playerWeapon;
    public KartFollowLaserSpot leftHandLaserPointerController; // The controller for the laser pointer if it is in the left hand
    public KartFollowLaserSpot rightHandLaserPointerController; // The controller for the laser pointer if it is in the right hand
    public GameObject playerHUD; // The player's HUD
    public Vector3 localPositionForHUD; // Where the HUD appears on the cart initially
    public GameObject trainningTrack; // The tutorial trainning track
    public TutorialLine[] tutorialLines; // 10, 11, 12

    public HeadWearHUD playerHead; // Head trigger that detects and controls wearing HUD

    public bool started; // Has the coroutine started

    // Use this for initialization
    void Start()
    {
        playerHead = FindObjectOfType<HeadWearHUD>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (GetComponent<LinearObjectMovement>().animationFinished && playerWeapon.transform.parent != null)
        //{
        //    playerWeapon.transform.parent = null;
        //}

        // If the tutorial line 9 finished
        if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLines[0].line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLines[0].played && TutorialManager.tutorialProgress == 4)
        {
            tutorialLines[0].played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[0].waitTime, tutorialLines[0].line);
        }

        // If the tutorial line 10 finished
        if (GameManager.gameManager.catCartVoiceOver.clip == tutorialLines[0].line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLines[1].played && TutorialManager.tutorialProgress == 4)
        {
            tutorialLines[1].played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[1].waitTime, tutorialLines[1].line);
        }

        // Active the weapon after the cart is stopped
        if (!playerHUD.activeInHierarchy && GameManager.currentSpeed <= 1)
        {
            objectDetector.SetActive(true);

            trainningTrack.GetComponent<LinearObjectMovement>().pause = false;
            StartCoroutine(HUDappear());
        }

        // Remove the "wear HUD" tutorial text after the player wears the HUD
        if (playerHead.isWearingHUD)
        {
            TutorialManager.tutorialText.text = "";

            // If the tutorial line 11 finished
            if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLines[2].line &&
                !GameManager.gameManager.catCartVoiceOver.isPlaying &&
                !tutorialLines[2].played && TutorialManager.tutorialProgress == 4)
            {
                tutorialLines[2].played = true;
                TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
                TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[2].waitTime, tutorialLines[2].line);
            }

            if (!playerWeapon.activeInHierarchy)
            {
                StartCoroutine(WeaponAppear());
            }
        }
    }

    // Show HUD on dashboard
    public IEnumerator HUDappear()
    {
        playerHUD.SetActive(true);
        playerHUD.transform.parent = GameManager.gameManager.playerKart.transform;
        playerHUD.transform.localPosition = localPositionForHUD;
        playerHUD.transform.parent = null;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent =
            GameManager.gameManager.nonPlayerKart.transform;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.localPosition = localPositionForHUD;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent = null;

        playerHUD.GetComponent<Rigidbody>().mass = 0;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().mass = 0;

        yield return new WaitForEndOfFrame();

        playerHUD.transform.parent = GameManager.gameManager.playerKart.transform;
        playerHUD.transform.localPosition = localPositionForHUD;
        playerHUD.transform.parent = null;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent =
            GameManager.gameManager.nonPlayerKart.transform;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.localPosition = localPositionForHUD;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent = null;

        playerHUD.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerHUD.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        yield return new WaitForEndOfFrame();

        playerHUD.GetComponent<Rigidbody>().mass = 1;
        playerHUD.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().mass = 1;
    }

    // Show Weapon on dashboard
    public IEnumerator WeaponAppear()
    {
        playerWeapon.SetActive(true);
        playerWeapon.GetComponent<PlayerSideMirror>().inKart = true;
        playerWeapon.GetComponent<PlayerSideMirror>().doMirror = true;
        playerWeapon.transform.parent = GameManager.gameManager.playerKart.transform;
        playerWeapon.transform.localPosition = GetComponent<LinearObjectMovement>().animationSequence[0].targetLocalPosition;
        playerWeapon.transform.parent = null;
        playerWeapon.GetComponent<PlayerSideMirror>().inKart = true;
        playerWeapon.GetComponent<PlayerSideMirror>().doMirror = true;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent =
            GameManager.gameManager.nonPlayerKart.transform;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.localPosition =
            GetComponent<LinearObjectMovement>().animationSequence[0].targetLocalPosition;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent = null;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerSideMirror>().doMirror = false;

        playerWeapon.GetComponent<Rigidbody>().mass = 0;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().mass = 0;

        yield return new WaitForEndOfFrame();

        playerWeapon.GetComponent<PlayerSideMirror>().inKart = true;
        playerWeapon.GetComponent<PlayerSideMirror>().doMirror = true;
        playerWeapon.transform.parent = GameManager.gameManager.playerKart.transform;
        playerWeapon.transform.localPosition = GetComponent<LinearObjectMovement>().animationSequence[0].targetLocalPosition;
        playerWeapon.transform.parent = null;
        playerWeapon.GetComponent<PlayerSideMirror>().inKart = true;
        playerWeapon.GetComponent<PlayerSideMirror>().doMirror = true;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent =
            GameManager.gameManager.nonPlayerKart.transform;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.localPosition =
            GetComponent<LinearObjectMovement>().animationSequence[0].targetLocalPosition;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.parent = null;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerSideMirror>().doMirror = false;

        playerWeapon.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerWeapon.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


        yield return new WaitForEndOfFrame();

        playerWeapon.GetComponent<Rigidbody>().mass = 1;
        playerWeapon.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Rigidbody>().mass = 1;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //StartCoroutine(GetComponent<LinearObjectMovement>().Animate());
        leftHandLaserPointerController.canDrive = false;
        rightHandLaserPointerController.canDrive = false;
        
        // Stop player from turning on the laser pointer
        //leftHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Undefined;
        //rightHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Undefined;
        //leftHandLaserPointerController.CallSubscribeActivationButton();
        //rightHandLaserPointerController.CallSubscribeActivationButton();

        TutorialManager.tutorialText.text = "Please put the helmet on your head";
    }
}
