using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class StartTouchingFeedback : ChangePlayerHandGesture
{
    //public float lastUngrabTime; // The last time it is ungrabbed

    public override void StartTouching(GameObject currentTouchingObject)
    {
        if (Time.time - lastUngrabTime >= 0.35f) // Prevent it to run right after the object is ungrabbed
        {
            currentTouchingObject.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse(1f, 0.1f, 0.2f);
        }

        base.StartTouching(currentTouchingObject);
    }
}
