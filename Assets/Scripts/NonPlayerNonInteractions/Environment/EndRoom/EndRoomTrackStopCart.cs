using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class EndRoomTrackStopCart : MonoBehaviour
{
    public GameObject catSucker; // The tube that will suck cats back in
    public GameObject suckerHandle; // The handle for the sucker
    public GameObject suckerHandleDummy; // The dummy for the handle
    //public GameObject suckerTrigger; // The trigger that detects cat
    public Transform suckerTubeWrap; // The transform of the wrap of the sucker tube
    public Text[] catStatusText; // The text that shows the status of the nine cats
    public Text[] catNameText; // The text that shows the names of the nine cats
    public LinearObjectMovement entranceTrack; // The track at the entrance of the end room
    public Text scoreText; // The text that displays the score
    public Text timeText; // The text that displays the time
    public LevelEndProcess levelResult; // The object that has the level results
    public LinearObjectMovement scoreBoard; // The gameobject that displays the score
    public AudioClip catCartLineForCatSucker; // The CatCart line to be played when the sucker is dropped
    public Slider scoreBar; // The score bar that fills for letter grade

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

    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player's cart hits the collider
        if (collision.transform.name == "CatKart")
        {
            StartCoroutine(ShowScoreProcess());
        }
    }

    /// <summary>
    /// Stop player's cart, suck up cats, show score board and scores
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowScoreProcess()
    {
        GameManager.gameFinished = true;
        PlayerKartBasket playerBasket = FindObjectOfType<PlayerKartBasket>();

        // Close the end room gate
        levelResult.endRoomGate.pause = false;

        // Stop the player from driving the cart
        leftHandLaserPointerController.canDrive = false;
        rightHandLaserPointerController.canDrive = false;

        GameManager.kartMovementInfo.laserOffStopKart(true); // Stop the cart
        GameManager.gameManager.playerKart.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Let player turn off the laser pointer
        leftHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        rightHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        leftHandLaserPointerController.CallSubscribeActivationButton();
        rightHandLaserPointerController.CallSubscribeActivationButton();

        // Wait for the player to turn off the laser pointer
        while (GameManager.kartMovementInfo.isLaserActive)
        {
            yield return null;
        }

        // Stop player from turning on the laser pointer
        leftHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Undefined;
        rightHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Undefined;
        leftHandLaserPointerController.CallSubscribeActivationButton();
        rightHandLaserPointerController.CallSubscribeActivationButton();

        // Enable the player from drop the weapon or laser pointer
        leftHandLaserPointerController.GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed +=
            new ControllerInteractionEventHandler(leftHandLaserPointerController.GetComponent<HandPickItems>().DoButtonTwpClicked);
        rightHandLaserPointerController.GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed +=
            new ControllerInteractionEventHandler(rightHandLaserPointerController.GetComponent<HandPickItems>().DoButtonTwpClicked);

        StartCoroutine(entranceTrack.Animate()); // Move down entrance track

        // Display cat names on the score board
        for (int i = 0; i < GameManager.catsInfo.catsInfo.Length; i++)
        {
            catNameText[i].text = GameManager.catsInfo.catsInfo[i].catName;
        }
        StartCoroutine(scoreBoard.Animate()); // Move down score board

        catSucker.transform.position =
            new Vector3(GameManager.gameManager.catBasket.transform.position.x,
                        catSucker.transform.position.y,
                        GameManager.gameManager.catBasket.transform.position.z); // Align the cat sucker with the cat basket
        StartCoroutine(catSucker.GetComponent<LinearObjectMovement>().Animate()); // Move down cat sucker
        // Wait for the sucker tube come down
        while (!catSucker.GetComponent<LinearObjectMovement>().pause)
        {
            yield return null;
        }

        // Turn on sucker handle
        suckerHandleDummy.SetActive(false);
        suckerHandle.SetActive(true);
        GameManager.gameManager.catCartVoiceOver.PlayOneShot(catCartLineForCatSucker); // Play the CatCart cat sucker line

        // Wait for the player returns all the cats
        while (playerBasket.catCount != 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4); // Wait for the last cat to suck up

        // Turn off sucker handle
        suckerHandle.GetComponent<StartTouchingFeedback>().isGrabbable = false; // Stop player from keep grabbing the sucker handle
        suckerHandleDummy.SetActive(true);
        suckerHandle.SetActive(false);

        catSucker.GetComponent<LinearObjectMovement>().animationSequence[1].targetLocalPosition.y -=
            suckerTubeWrap.localPosition.y; // The sucker should raise up according to how high the sucker tube is
        catSucker.GetComponent<LinearObjectMovement>().pause = false; // Resume sucker going up animation

        // Wait for the sucker to come up
        while (!catSucker.GetComponent<LinearObjectMovement>().animationFinished)
        {
            yield return null;
        }

        // Display the status of missing cats as "MIA"
        for (int i = 0; i < catStatusText.Length; i++)
        {
            if (catStatusText[i].text == "" || catStatusText[i].text == null)
            {
                catStatusText[i].text = "MIA";
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Displays score
        for (float x = 0; Mathf.Log(x + 1) < 1; x += Time.deltaTime * 1f)
        {
            scoreText.text = "SCORE  : " + levelResult.finalScore * Mathf.Log(x + 1);
            scoreBar.value = Mathf.Clamp(levelResult.finalScore * Mathf.Log(x + 1), scoreBar.minValue, scoreBar.maxValue);
            yield return null;
        }
        scoreText.text = "SCORE  : " + levelResult.finalScore;
        scoreBar.value = Mathf.Clamp(levelResult.finalScore, scoreBar.minValue, scoreBar.maxValue);
        if (levelResult.finalScore > SavePlayerData.allTimeHighScore) // Record high score if the player beats the current high score
        {
            SavePlayerData.allTimeHighScore = levelResult.finalScore;
            PlayerPrefs.SetInt("AllTimeHighScore", SavePlayerData.allTimeHighScore);
            PlayerPrefs.Save();
        }
        yield return new WaitForSeconds(0.5f);
        // Displays time
        timeText.text = "TIME  :  " + FormatTime(levelResult.finalTime);

        // Enable the use of laser pointer
        leftHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        rightHandLaserPointerController.activationButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Press;
        leftHandLaserPointerController.CallSubscribeActivationButton();
        rightHandLaserPointerController.CallSubscribeActivationButton();
        // Enable the player to drive the cart
        leftHandLaserPointerController.canDrive = true;
        rightHandLaserPointerController.canDrive = true;

        // Wait for the player to turn on laser pointer again
        while (!GameManager.kartMovementInfo.isLaserActive)
        {
            yield return null;
        }

        scoreBoard.pause = false;
    }

    /// <summary>
    /// Format the time then return in string
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public string FormatTime(float time)
    {
        string timeString = ""; // Stores the formated time text
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        seconds = seconds % 60;
        int miliseconds = (int)((time * 100) % 100);
        timeString = minutes.ToString() + "'" + seconds.ToString() + "\"" + miliseconds;

        return timeString;
    }

    /// <summary>
    /// Show the name of the cat that is just sucked up on the leaderboard
    /// </summary>
    /// <param name="cat"></param>
    public void ShowCatOnLeaderboard(PlayerCatStayInBasket cat)
    {
        for (int i = 0; i < GameManager.gameManager.cats.Length; i++)
        {
            if (cat == GameManager.gameManager.cats[i])
            {
                catStatusText[i].text = "SAFE";
            }
        }
    }
}
