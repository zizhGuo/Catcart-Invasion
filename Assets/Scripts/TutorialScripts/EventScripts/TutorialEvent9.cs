using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent9 : TutorialEventModel
{
    public GameObject exitButtonWrap; // The button the player need to press to exit the tutorial
    public GameObject tutorialGate; // The exit gate for the tutorial room
    public GameObject exitTrackWrap; // The exit track that guides the player out of the tutorial room
    //public GameObject playerHUD; // The player's HUD
    //public Vector3 localPositionForHUD; // Where the HUD appears on the cart initially
    public Collider leftHandCatPawCollider; // The collider for the left hand Cat-Paw (it has no use after the player exits tutorial)
    public Collider rightHandCatPawCollider; // The collider for the right hand Cat-Paw
    public KartFollowLaserSpot leftHandLaserPointerController; // The controller for the laser pointer if it is in the left hand
    public KartFollowLaserSpot rightHandLaserPointerController; // The controller for the laser pointer if it is in the right hand
    public GameObject exitButtonFollowee; // The rigidbody collider the exit button follows 
    //public GameObject exitButtonMeshDummy; // The copy skin of the exit button, turns off after the button flip animation is finished
    //public GameObject cartObjectDetector; // The CatCart's object detector to sync position of items within the cart with the mirror cart
    public TutorialLine tutorialLine; // 20
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (exitButtonWrap.GetComponent<LinearObjectMovement>().animationFinished)
        {
            Destroy(exitButtonWrap.GetComponentInChildren<SpringJoint>());
            foreach (Collider c in exitButtonWrap.GetComponentsInChildren<Collider>())
            {
                Destroy(c);
            }
        }

        // If the tutorial line 19 finished
        if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLine.line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLine.played && TutorialManager.tutorialProgress == 8)
        {
            tutorialLine.played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLine.waitTime, tutorialLine.line);
        }
    }

    private void OnEnable()
    {
        exitButtonWrap.GetComponent<LinearObjectMovement>().pause = false;
        StartCoroutine(tutorialGate.GetComponent<LinearObjectMovement>().Animate());
        StartCoroutine(exitTrackWrap.GetComponent<LinearObjectMovement>().Animate()); 
        //cartObjectDetector.SetActive(true);

        // Change the maximum speed when player start level
        GameManager.sSpeedMultiplier = 12;

        // Disable colliders on the left and right Cat-Paw
        leftHandCatPawCollider.enabled = false;
        rightHandCatPawCollider.enabled = false;

        // Enable the use of laser pointer
        //leftHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        //rightHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        //leftHandLaserPointerController.CallSubscribeActivationButton();
        //rightHandLaserPointerController.CallSubscribeActivationButton();

        // Enable the player to drive the cart
        leftHandLaserPointerController.canDrive = true;
        rightHandLaserPointerController.canDrive = true;

        //exitButtonFollowee.SetActive(false);
        //exitButtonMeshDummy.SetActive(true);
    }
}
