using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class PlayerEmergencyStopGame : MonoBehaviour
{
    public VignetteAndChromaticAberration playerFOVLimiter; // The script that controls how much the black cover the camera
    public float timeToQuit; // How long the player has to press down both grip button to quit and return to the start menu

    public float bothGripStartPressTime; // The last time the grip buttons on both controllers are pressed down
    public bool leftGripDown; // Is the left grip button pressed down
    public bool rightGripDown; // Is the right grip button pressed down
    public VRTK_ControllerEvents leftControllerEvent;
    public VRTK_ControllerEvents rightControllerEvent;

    // Use this for initialization
    void Start()
    {
        bothGripStartPressTime = 0;
        playerFOVLimiter = FindObjectOfType<VignetteAndChromaticAberration>();
        leftControllerEvent = GameManager.sLeftController.GetComponent<VRTK_ControllerEvents>();
        rightControllerEvent = GameManager.sRightController.GetComponent<VRTK_ControllerEvents>();
    }

    // Update is called once per frame
    void Update()
    {
        // If at least one grip is not clicked yet
        if (!leftGripDown || !rightGripDown)
        {
            // if both grip are start to be pressed
            if (leftControllerEvent.gripPressed && rightControllerEvent.gripPressed)
            {
                bothGripStartPressTime = Time.time;
            }
        }

        leftGripDown = leftControllerEvent.gripPressed;
        rightGripDown = rightControllerEvent.gripPressed;
        //print(leftControllerEvent.gripPressed + " grip press " + rightControllerEvent.gripPressed);
        //print(leftControllerEvent.gripPressed + " trigger press " + rightControllerEvent.gripPressed);
        //print(leftControllerEvent.gripPressed + " trigger click " + rightControllerEvent.gripPressed);

        if (leftGripDown && rightGripDown)
        {
            playerFOVLimiter.intensity = Mathf.Clamp01((Time.time - bothGripStartPressTime) / timeToQuit);

            if (Time.time - bothGripStartPressTime > timeToQuit + 1)
            {
                SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
            }
        }
    }
}
