using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent2 : TutorialEventModel
{
    public GameObject playerDetector; // The detector that detects if the player is in the CatCart
    //public GameObject laserPointerContainer; // The container for the laser pointer
    //public MeshRenderer laserPointerMesh; // The laser pointer mesh
    public GameObject catPawStick; // The Cat-Paw Stick
    public KartFollowLaserSpot leftHandLaserPointerController; // The controller for the laser pointer if it is in the left hand
    public KartFollowLaserSpot rightHandLaserPointerController; // The controller for the laser pointer if it is in the right hand
    public GameObject leftHandCatPawStick; // The Cat-Paw Stick on the left hand
    public GameObject rightHandCatPawStick; // The Cat-Paw Stick on the right hand
    public GameObject trainningTrack; // The tutorial trainning track
    public TutorialLine tutorialLine; // 7

    //public float yPositionToActivateLaserPointer; // The laser pointer should be set active when the container is lowered than this value

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!laserPointer.activeInHierarchy && laserPointerContainer.transform.position.y <= yPositionToActivateLaserPointer)
        //{
        //    laserPointer.SetActive(true);
        //    this.enabled = false;
        //}

        if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLine.line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLine.played && TutorialManager.tutorialProgress == 2)
        {
            tutorialLine.played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLine.waitTime, tutorialLine.line);
        }

        if (!leftHandLaserPointerController.enabled)
        {
            leftHandLaserPointerController.enabled = true;
            rightHandLaserPointerController.enabled = true;
        }
    }

    private void OnEnable()
    {
        playerDetector.SetActive(true);
        GameManager.gameManager.alwaysSyncPlayAreaWithCart = true;
        FindObjectOfType<DetectIfPlayerIsInKart>().getInKart();
        //laserPointerMesh.enabled = true;
        catPawStick.SetActive(false);
        leftHandCatPawStick.SetActive(false);
        rightHandCatPawStick.SetActive(false);
        leftHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        rightHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        leftHandLaserPointerController.enabled = false;
        rightHandLaserPointerController.enabled = false;
        //StartCoroutine(laserPointerContainer.GetComponent<LinearObjectMovement>().Animate());
        TutorialManager.tutorialWrap.SetActive(true);
        TutorialManager.tutorialText.text = "Click trigger to toggle Laserpointer";
        StartCoroutine(trainningTrack.GetComponent<LinearObjectMovement>().Animate());
    }
}
