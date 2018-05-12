using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The script that is responsible for all the main menu navigation and event
/// </summary>
public class MenuNavAndEvent : MonoBehaviour
{
    public GameObject triggerHoldTutorial; // The menu game object that teaches the player click and hold trigger
    public GameObject mainMenu; // The object that holds every main menu elements
    public bool isGameOverMenu; // If this is the game over menu
    public Text playerScore; // Player's score displayed at the game over screen
    public Text surveyText; // The survey text
    public GameObject creditWrap; // The credit page

    // Use this for initialization
    void Start()
    {
        if (isGameOverMenu)
        {
            playerScore.text = "Your score is: " + GameManager.score;
            return;
        }

        StartCoroutine(HideTriggerColorCodeTutorial());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Function to start game with tutorial
    public void StartGameWithTutorial()
    {
        SceneManager.LoadSceneAsync("TestNewGreyBoxTutorialRework", LoadSceneMode.Single);
    }

    // Function to start game
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("TestNewGreyBox", LoadSceneMode.Single);
    }

    public IEnumerator HideTriggerColorCodeTutorial()
    {
        yield return new WaitForSeconds(7);

        triggerHoldTutorial.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Function to open the survey in the player's default browser
    /// </summary>
    public void OpenSurveyWebpage()
    {
        Application.OpenURL("https://goo.gl/forms/YPa1URtXr2lijECx1");
        surveyText.text = "The survey has opened in your browser";
    }

    public void ShowCredit()
    {
        mainMenu.SetActive(false);
        creditWrap.SetActive(true);
    }

    public void HideCredit()
    {
        creditWrap.SetActive(false);
        mainMenu.SetActive(true);
    }
}
