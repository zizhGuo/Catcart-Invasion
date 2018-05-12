using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger9 : TutorialTriggerModel
{
    public CheckIfButtonPressed gateButton; // The exit button for the tutorial gate

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 7)
        {
            return;
        }

        // If the bullseye is shot
        if (gateButton.buttonDown)
        {
            gameObject.SetActive(false);
        }
    }
}
