using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class SavePlayerData : MonoBehaviour
{


    public static float triggerClickTime; // How fast can the current player click the trigger
    public static int allTimeHighScore; // The high score record
    public static object lastClickTriggerController; // The last controller with its trigger being clicked down
    public static float triggerDownTime; // Last time the trigger is clicked down
    public static bool calibrateTriggerClick; // Does the game calibrate the trigger click

    // Use this for initialization
    void Start()
    {
        // Set trigger click time
        if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            PlayerPrefs.SetFloat("TriggerClickTime", 0);
            triggerClickTime = 0;
            calibrateTriggerClick = true;
        }
        else
        {
            triggerClickTime = PlayerPrefs.GetFloat("TriggerClickTime");
        }

        // Set high score
        if (!PlayerPrefs.HasKey("AllTimeHighScore"))
        {
            allTimeHighScore = 0;
            PlayerPrefs.SetInt("AllTimeHighScore", allTimeHighScore);
            PlayerPrefs.Save();
        }
        else
        {
            allTimeHighScore = PlayerPrefs.GetInt("AllTimeHighScore", allTimeHighScore);
        }

        foreach (VRTK_ControllerEvents cE in FindObjectsOfType<VRTK_ControllerEvents>())
        {
            cE.TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);
            cE.TriggerUnclicked += new ControllerInteractionEventHandler(DoTriggerUnclicked);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Call when the trigger is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        lastClickTriggerController = sender;
        triggerDownTime = Time.time;
    }

    /// <summary>
    /// Call when the trigger is unclicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DoTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        if (!calibrateTriggerClick) // Don't calibrate the trigger click time if it is not allowed
        {
            return;
        }

        if (sender == lastClickTriggerController) // If the controller is the one whose trigger is being clicked the most recent
        {
            // If the time for the player to click the trigger is longer than the shortest time and is shorter than 1 sec
            if (Time.time - triggerDownTime > triggerClickTime && Time.time - triggerDownTime <= 0.35f)
            {
                triggerClickTime = Time.time - triggerDownTime;
                PlayerPrefs.SetFloat("TriggerClickTime", triggerClickTime);
                PlayerPrefs.Save();
                print(triggerClickTime);
            }
        }

        sender = null;
    }
}
