using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent5 : TutorialEventModel
{
    public GameObject catDropper; // The tube where the cats will drop from
    public GameObject catContainer; // The container where the cats will drop into
    public GameObject catContainerTrigger; // The trigger detects if the container has been shot by the player pistol
    public GameObject objectDetector; // The detector that detects if objects is in the CatCart
    public Vector3 catDropperRelativePosition; // Where the dropper should appear relative to the player's cart
    public Vector3 catDropperRelativeEuler; // Which direction the dropper should appear relative to the player's cart
    public Vector3 catContainerRelativePosition; // Where the container should appear relative to the player's cart
    public Vector3 catContainerRelativeEuler; // Which direction the container should appear relative to the player's cart
    public GameObject[] cats; // The cats to be dropped
    public Vector3 catsRelativePosition; // Where the cats should appear relative to the player's cart
    public Vector3 catsRelativeEuler; // Which direction the cats should appear relative to the player's cart
    public TutorialLine[] tutorialLines; // 13, 14
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the animation for the cat container is finished, set the container trigger to be active
        if (!catContainerTrigger.activeInHierarchy && catContainer.GetComponent<LinearObjectMovement>().pause)
        {
            catContainerTrigger.SetActive(true);
            catContainer.GetComponent<AudioSource>().Stop();
            //gameObject.SetActive(false);
        }
        
        // If the tutorial line 12 finished
        if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLines[0].line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLines[0].played && TutorialManager.tutorialProgress == 5)
        {
            tutorialLines[0].played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[0].waitTime, tutorialLines[0].line);
        }

        // If the tutorial line 13 finished
        if (GameManager.gameManager.catCartVoiceOver.clip == tutorialLines[0].line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLines[1].played && TutorialManager.tutorialProgress == 5)
        {
            tutorialLines[1].played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[1].waitTime, tutorialLines[1].line);
        }
    }

    private void OnEnable()
    {
        // Place the catDropper and catContainer and the cats by the player's cart
        catDropper.transform.position = GameManager.gameManager.playerKart.transform.TransformPoint(catDropperRelativePosition);
        //catDropper.transform.eulerAngles = GameManager.gameManager.playerKart.transform.InverseTransformDirection(catDropperRelativeEuler);
        catDropper.transform.eulerAngles = GameManager.gameManager.playerKart.transform.eulerAngles + catDropperRelativeEuler;
        catContainer.transform.position = GameManager.gameManager.playerKart.transform.TransformPoint(catContainerRelativePosition);
        //catContainer.transform.eulerAngles = GameManager.gameManager.playerKart.transform.InverseTransformDirection(catContainerRelativeEuler);
        catContainer.transform.eulerAngles = GameManager.gameManager.playerKart.transform.eulerAngles + catContainerRelativeEuler;

        // Move and rotate cats relate to player's cart's transform
        foreach (GameObject c in cats)
        {
            c.transform.position = GameManager.gameManager.playerKart.transform.TransformPoint(catsRelativePosition);
            c.transform.eulerAngles = GameManager.gameManager.playerKart.transform.eulerAngles + catsRelativeEuler;
            c.GetComponent<MoveCatBackToContainer>().catReappearPosition = c.transform.position;
            c.GetComponent<MoveCatBackToContainer>().catReappearRotation = c.transform.rotation;
        }

        //catDropper.GetComponent<LinearObjectMovement>().animationSequence[0].targetLocalPosition.x = 
        //    catDropper.transform.localPosition.x;
        //catDropper.GetComponent<LinearObjectMovement>().animationSequence[0].targetLocalPosition.z =
        //    catDropper.transform.localPosition.z;
        //catDropper.GetComponent<LinearObjectMovement>().animationSequence[1].targetLocalPosition.x =
        //    catDropper.transform.localPosition.x;
        //catDropper.GetComponent<LinearObjectMovement>().animationSequence[1].targetLocalPosition.z =
        //    catDropper.transform.localPosition.z;

        StartCoroutine(catDropper.GetComponent<LinearObjectMovement>().Animate());
        StartCoroutine(catContainer.GetComponent<LinearObjectMovement>().Animate());
        catContainer.GetComponent<AudioSource>().Play();

        objectDetector.SetActive(false);

        SavePlayerData.calibrateTriggerClick = false;

#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPaused = true;
#endif
        TutorialManager.tutorialText.text = "Click trigger to shoot" + "\r\n" + "Hold down trigger to use melee";
    }
}
