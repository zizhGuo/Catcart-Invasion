using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class ChangePlayerHandGesture : VRTK_InteractableObject
{
    public float lastUngrabTime; // The last time it is ungrabbed
    //public VRTK_InteractableObject interactableComponent; // The VRTK_InteractableObject subclass that is attached to this object

    private void Start()
    {
        //interactableComponent = GetComponent<VRTK_InteractableObject>();
    }

    public override void StartTouching(GameObject currentTouchingObject)
    {
        if (Time.time - lastUngrabTime >= 0.35f) // Prevent it to run right after the object is ungrabbed
        {
            currentTouchingObject.GetComponent<PlayerHandGestureController>().ChangeGesture(1); // Change gesture to expand
        }

        if (!GameManager.gameStart) // Show the controller model in tutorial
        {
            //currentTouchingObject.GetComponent<VRTK_ControllerActions>().SetControllerOpacity(0.7f);

            if (holdButtonToGrab) // Mark the trigger button Red if it needs to be holding trigger to grab
            {
                currentTouchingObject.GetComponent<VRTK_ControllerActions>().ToggleHighlightTrigger(true, Color.red, 0.5f);
            }
            else
            {
                currentTouchingObject.GetComponent<VRTK_ControllerActions>().ToggleHighlightTrigger(true, Color.yellow, 0.5f);
            }
        }

        base.StartTouching(currentTouchingObject);
    }

    public override void StopTouching(GameObject previousTouchingObject)
    {
        previousTouchingObject.GetComponent<PlayerHandGestureController>().ChangeGesture(0); // Change gesture to relax

        // Hide the controller model
        //previousTouchingObject.GetComponent<VRTK_ControllerActions>().SetControllerOpacity(0f);
        previousTouchingObject.GetComponent<VRTK_ControllerActions>().ToggleHighlightTrigger(true, Color.clear, 0.5f);

        base.StopTouching(previousTouchingObject);
    }

    public override void Grabbed(GameObject currentGrabbingObject)
    {
        //print("On grab, active? " + gameObject.activeInHierarchy);

        if (holdButtonToGrab) // If the object need to be hold button to be grabbed
        {
            currentGrabbingObject.GetComponent<PlayerHandGestureController>().ChangeGesture(2); // Change gesture to pick
        }
        else
        {
            currentGrabbingObject.GetComponent<PlayerHandGestureController>().ChangeGesture(3); // Change gesture to pick
        }

        if (!GameManager.gameStart) // Hide the controller model in tutorial
        {
            //currentGrabbingObject.GetComponent<VRTK_ControllerActions>().SetControllerOpacity(0f);
            currentGrabbingObject.GetComponent<VRTK_ControllerActions>().ToggleHighlightTrigger(true, Color.clear, 0.5f);
        }

        base.Grabbed(currentGrabbingObject);
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        //print("On ungrab, active? " + gameObject.activeInHierarchy);

        if (gameObject.activeInHierarchy) // Prevent it to run if it is inactive after being grabbed
        {
            previousGrabbingObject.GetComponent<PlayerHandGestureController>().ChangeGesture(0); // Change gesture to relax
        }

        lastUngrabTime = Time.time;
        base.Ungrabbed(previousGrabbingObject);
    }
}
