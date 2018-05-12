using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger8 : TutorialTriggerModel
{
    public GateButtonBullseyeTrigger gateBullseye; // The bullseye next to the exit gate, the exit button is on the back side

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the bullseye is shot
        if (gateBullseye.isShot)
        {
            gameObject.SetActive(false);
        }
    }
}
