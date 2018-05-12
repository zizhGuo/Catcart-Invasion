using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public RotationInfo[] rotationSequence; // The array that stores the information of each segment of the entire animation sequence
    public GameObject objectToBeRotated; // The object that is going to be animated by this script. If this is null, then it will default to the GameObject the script is attached to
    public bool animationFinished; // Is the animation finished
    public bool pause; // Is the animation paused

    // Use this for initialization
    void Start()
    {
        animationFinished = false;
        pause = false;
        // Testing purpose
        //StartCoroutine(Animate());
    }

    /// <summary>
    /// The coroutine that controls the entire animation sequence
    /// </summary>
    /// <returns></returns>
    public IEnumerator Animate()
    {
        Quaternion startRotation = Quaternion.identity; // Use to store temporary Quaternion value for later use
        Quaternion targetRotation = Quaternion.identity;
        float rotateDuration; // The duration for each segment
        if (objectToBeRotated == null)
        {
            objectToBeRotated = gameObject;
        }

        for (int i = 0; i < rotationSequence.Length; i++)
        {
            if (rotationSequence[i].startLocalEuler == null || rotationSequence[i].startLocalEuler == Vector3.zero) // If starting position is null or zero, it will be the object's current position
            {
                rotationSequence[i].startLocalEuler = objectToBeRotated.transform.localEulerAngles;
            }

            // Calculate the duration for the current segment
            targetRotation.eulerAngles = rotationSequence[i].targetLocalEuler;
            rotateDuration = Quaternion.Angle(objectToBeRotated.transform.rotation, targetRotation) / rotationSequence[i].rotateSpeed;

            startRotation.eulerAngles = rotationSequence[i].startLocalEuler;

            for (float t = 0; t < 1; t += Time.deltaTime / rotateDuration)
            {
                objectToBeRotated.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            objectToBeRotated.transform.localEulerAngles = rotationSequence[i].targetLocalEuler;

            if (rotationSequence[i].pauseBeforeNext)
            {
                pause = true;
            }

            while (pause) // Pause the animation
            {
                yield return null;
            }

            yield return new WaitForSeconds(rotationSequence[i].waitTimeBeforeNext);
        }

        animationFinished = true;
    }
}

/// <summary>
/// Stores information about the start position and the target position of the current segment of animation, 
/// the constant speed that the object will move at, and how much time to wait before starting the next segment
/// </summary>
[Serializable]
public class RotationInfo
{
    public Vector3 startLocalEuler; // The start position of the object. If it is null or zero, it will be the object's current position
    public Vector3 targetLocalEuler; // The target position of the object
    public float rotateSpeed; // The rotation speed of the object for this segment (degree in angle/sec)
    public float waitTimeBeforeNext; // The waiting time before starting of the next segment
    public bool pauseBeforeNext; // Should the animation pause after the completion of this segment and before the next segment
}
