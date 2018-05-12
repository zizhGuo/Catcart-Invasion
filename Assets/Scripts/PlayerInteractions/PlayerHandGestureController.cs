using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Gesture index:
/// 0: Relax
/// 1: Expand
/// 2: Pick
/// 3: Grab
/// </summary>
public class PlayerHandGestureController : MonoBehaviour
{
    public GameObject[] gestureObjects; // All the hand gesture game objects

    public GameObject currentGesture; // The current hand gesture object that is active

    // Use this for initialization
    void Start()
    {
        currentGesture = gestureObjects[0];
        currentGesture.SetActive(true);
        //GetComponent<VRTK_ControllerActions>().ToggleControllerModel(false, null); // Make the controller model invisible
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeGesture(int gestureIndex)
    {
        currentGesture.SetActive(false);
        currentGesture = gestureObjects[gestureIndex];
        currentGesture.SetActive(true);
    }
}

/// <summary>
/// Stores information about the position and the rotation of a hand gesture, 
/// the corresponding GameObject for that hand gesture, and the name of that gesture
/// </summary>
[Serializable]
public class GestureInfo
{
    public GameObject gestureObject; // The corresponding game object for that gesture
    public Vector3 gestureLocalPosition; // The local position for the gesture
    public Vector3 gestureLocalEulerAngles; // The local euler angles for the gesture
    public string gestureName; // The name of the gesture
}