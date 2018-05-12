using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] tutorialEventTriggers; // A list of triggers that when triggered, will start the corresponding event
    public GameObject[] tutorialEvents; // A list of events that will start when the corresponding trigger is triggered
    public Canvas tutorialCanvas; // The GUI canvas for tutorial 
    public GameObject catSucker; // The tube that can suck up cats

    public static int tutorialProgress; // Which is the last tutorial step that the player completed
    public bool isTutorialFinished; // If the tutorial is finished
    public static Text tutorialText; // The text on the tutorial
    public static GameObject tutorialWrap; // The gameobject that contain's the tutorial GUI elements
    public static TutorialManager tutorialManager; // The static reference to this TutorialManager
    public static List<Coroutine> tutorialLineCoroutines = new List<Coroutine>(); // Keep track of the activate tutorial line coroutines

    /// <summary>
    /// Testing
    /// </summary>

    public int testTutorialProgress; // Testing
    public GameObject playerWeapon;
    public int tutorialLineCoroutineCount;

    // Use this for initialization
    void Start()
    {
        // If we skip the tutorial
        if (GameManager.gameManager.skipTutorial)
        {
            //tutorialEvents[1].SetActive(true);
            //tutorialEventTriggers[1].GetComponent<TutorialTrigger2>().catCartStartKeyhole.SetActive(false);
            //tutorialEvents[6].SetActive(true);
            //tutorialEvents[7].SetActive(true);
            //tutorialEvents[8].SetActive(true);
            //tutorialEvents[7].GetComponent<TutorialEvent8>().playerDetector.transform.localScale *= 2f;
            //tutorialEvents[7].GetComponent<TutorialEvent8>().playerDetector.SetActive(true);
            //tutorialEvents[7].GetComponent<TutorialEvent8>().cartObjectDetector.SetActive(true);
            //GameManager.sLeftController.GetComponent<KartFollowLaserSpot>().enabled = true;
            //GameManager.sRightController.GetComponent<KartFollowLaserSpot>().enabled = true;

            isTutorialFinished = true;
            GameManager.gameStartProc();
            return;
        }

        isTutorialFinished = false;
        tutorialProgress = 0;
        tutorialText = tutorialCanvas.GetComponentInChildren<Text>();
        tutorialWrap = tutorialCanvas.gameObject;
        tutorialManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameManager.skipTutorial)
        {
            GameManager.sLeftController.GetComponent<VRTK_ControllerActions>().ToggleControllerModel(false, null);
            GameManager.sRightController.GetComponent<VRTK_ControllerActions>().ToggleControllerModel(false, null);

            return;
        }

        // Check tutorial progress if the game hasn't start (meaning the tutorial is not finished yet)
        if (!GameManager.gameStart)
        {
            CheckEventTriggers();

            // Make the controller models transparent
            GameManager.sLeftController.GetComponent<VRTK_ControllerActions>().SetControllerOpacity(0f);
            GameManager.sRightController.GetComponent<VRTK_ControllerActions>().SetControllerOpacity(0f);

            // Start the game when the tutorial is finished
            if (tutorialProgress >= tutorialEventTriggers.Length)
            {
                GameManager.gameStartProc(); // Start Game
                StopAllPlayLineCoroutines();

                // Change the BGM to beginning BGM
                StartCoroutine(BGMController.bgmController.ChangeBGM(
                    BGMController.bgmController.beginningBGMIntro, BGMController.bgmController.beginningBGMLoop));

                //gameObject.SetActive(false); // Deactivate tutorial related script events
            }
        }

        ///
        /// Testing
        ///
        testTutorialProgress = tutorialProgress;
        tutorialLineCoroutineCount = tutorialLineCoroutines.Count;
    }

    private void FixedUpdate()
    {
        ///
        /// Testing
        ///

        //print("local: " + playerWeapon.transform.localPosition);
        //print(playerWeapon.transform.position);
    }

    /// <summary>
    /// Check if any tutorial event trigger is triggered
    /// </summary>
    public void CheckEventTriggers()
    {
        for (int i = tutorialProgress; i < tutorialEventTriggers.Length; i++)
        {
            if (tutorialEventTriggers[i] != null && !tutorialEventTriggers[i].activeInHierarchy)
            {
                tutorialEvents[i].SetActive(true);
                tutorialProgress = i + 1; // So the completed tutorial segments won't be checked again
            }
        }
    }

    /// <summary>
    /// Check if the tutorial is finished
    /// </summary>
    public void CheckTutorialFinish()
    {

    }

    /// <summary>
    /// Start the coroutine that plays a new voice over line
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="lineClip"></param>
    public void StartPlayLineCoroutine(float waitTime, AudioClip lineClip)
    {
        //print("start line");
        tutorialLineCoroutines.Add(StartCoroutine(TutorialLine.PlayLine(waitTime, lineClip)));
    }

    /// <summary>
    /// Stop all current ongoing tutorial line coroutines
    /// </summary>
    public void StopAllPlayLineCoroutines()
    {
        for (int i = tutorialLineCoroutines.Count - 1; i > -1; i--)
        {
            if (!GameManager.gameManager.catCartVoiceOver.isPlaying)
            {
                StopCoroutine(tutorialLineCoroutines[i]);
                tutorialLineCoroutines.Remove(tutorialLineCoroutines[i]);
            }
        }
    }
}

/// <summary>
/// Stores information about a tutorial line
/// </summary>
[Serializable]
public class TutorialLine
{
    public float waitTime; // How long does it wait to play the line
    public AudioClip line; // The line to be played
    public bool played; // Is this line played already

    public static IEnumerator PlayLine(float waitTime, AudioClip lineClip)
    {
        //Debug.Log("start line inner");

        yield return new WaitForSeconds(waitTime);

        GameManager.gameManager.catCartVoiceOver.clip = lineClip;
        GameManager.gameManager.catCartVoiceOver.PlayOneShot(lineClip);
    }
}
