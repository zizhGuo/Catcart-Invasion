using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent8 : TutorialEventModel
{
    public GameObject exitButtonWrap; // The button the player need to press to exit the tutorial
    public GameObject catDropper; // The tube where the cats will drop from
    public GameObject catContainer; // The container where the cats will drop into
    //public GameObject exitButtonTrigger; // The button trigger
    public GameObject exitButtonFollowee; // The actual Cat-Paw shaped exit button
    //public GameObject exitButtonMeshDummy; // The copy skin of the exit button, turns off after the button flip animation is finished
    public GameObject playerDetector; // The player detector in the cart
    public KartFollowLaserSpot leftHandLaserPointerController; // The controller for the laser pointer if it is in the left hand
    public KartFollowLaserSpot rightHandLaserPointerController; // The controller for the laser pointer if it is in the right hand
    public GameObject bullseye; // The bullseye
    public GameObject cartObjectDetector; // The CatCart's object detector to sync position of items within the cart with the mirror cart
    public Vector3 exitButtonRelativePosition; // Where the exit button should appear relative to the player's cart
    public Vector3 exitButtonRelativeEuler; // Which direction the exit button should appear relative to the player's cart
    public TutorialLine[] tutorialLines; // 18, 19

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the tutorial line 17 finished
        if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLines[0].line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLines[0].played && TutorialManager.tutorialProgress == 7)
        {
            tutorialLines[0].played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[0].waitTime, tutorialLines[0].line);
        }

        if (exitButtonWrap.GetComponent<LinearObjectMovement>().pause && !exitButtonFollowee.activeInHierarchy)
        {
            //exitButtonTrigger.SetActive(true);
            exitButtonFollowee.SetActive(true);
            //exitButtonMeshDummy.SetActive(false);
            //gameObject.SetActive(false);
        }

        if (exitButtonWrap.GetComponent<LinearObjectMovement>().pause)
        {
            // If the tutorial line 18 finished
            if (GameManager.gameManager.catCartVoiceOver.clip == tutorialLines[0].line &&
                !GameManager.gameManager.catCartVoiceOver.isPlaying &&
                !tutorialLines[1].played && TutorialManager.tutorialProgress == 7)
            {
                tutorialLines[1].played = true;
                TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
                TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[1].waitTime, tutorialLines[1].line);
            }
        }
    }

    private void OnEnable()
    {
        exitButtonWrap.transform.position = GameManager.gameManager.playerKart.transform.TransformPoint(exitButtonRelativePosition);
        exitButtonWrap.transform.eulerAngles = GameManager.gameManager.playerKart.transform.eulerAngles + exitButtonRelativeEuler;

        cartObjectDetector.SetActive(true);
        StartCoroutine(exitButtonWrap.GetComponent<LinearObjectMovement>().Animate());
        catDropper.GetComponent<LinearObjectMovement>().pause = false;
        catContainer.GetComponent<LinearObjectMovement>().pause = false;
        playerDetector.transform.localScale /= 2f; // Reduce the player detector size
        //leftHandLaserPointerController.canDrive = true;
        //rightHandLaserPointerController.canDrive = true;
        //bullseye.GetComponent<RotateObject>().pause = false;
        TutorialManager.tutorialWrap.SetActive(false);
    }
}
