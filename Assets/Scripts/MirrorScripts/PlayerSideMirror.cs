using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;

public class PlayerSideMirror : MonoBehaviour
{
    /// <summary>
    /// Basically, all the actions happened in the game will follows a "hidden" kart which can only turn but not move. 
    /// This script is attached to the game objects that will be mirrored to the "hidden" kart system, where the transform of the objects
    /// that the player interact with see will be mirrored to this hidden system.
    /// </summary>



    public bool doMirror; // Should this object currently mirroring the non-player side transform?
    public GameObject playerKart; // The player side kart
    public GameObject nonPlayerKart; // The non-player side kart
    public GameObject nonPlayerSideCopy; // Its copy on the non-player side
    public bool inKart; // Is this object in kart? If it is in kart and not being interacted by the player then it should mirroring transform from its non-player copy
    public Vector3 relativeVelocity; // The object's relative velocity to player's kart, this should apply as the initial velocity when the physics simulation transfer to non-player side
    public Vector3 lastRelativePosition;
    public Vector3 lastRelativeVelocity;
    public Vector3 lastRelativeAcceleration;

    // Use this for initialization
    void Start()
    {
        if (FindObjectOfType<MirrorGameManager>())
        {
            playerKart = FindObjectOfType<MirrorGameManager>().playerKart;
            nonPlayerKart = FindObjectOfType<MirrorGameManager>().nonPlayerKart;
        }

        else
        {
            playerKart = FindObjectOfType<GameManager>().playerKart;
            nonPlayerKart = FindObjectOfType<GameManager>().nonPlayerKart;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (doMirror)
        {
            transform.position = playerKart.transform.TransformPoint(nonPlayerKart.transform.InverseTransformPoint(nonPlayerSideCopy.transform.position)); // Mirror the non-player side position
            transform.rotation = nonPlayerSideCopy.transform.rotation; // Mirror the non-player side rotation
        }

        //print("player mirror");
    }

    void FixedUpdate()
    {
        lastRelativeVelocity = relativeVelocity;

        relativeVelocity = (playerKart.transform.InverseTransformPoint(transform.position) - lastRelativePosition) / Time.fixedUnscaledDeltaTime;
        relativeVelocity.x /= playerKart.transform.lossyScale.x;
        relativeVelocity.y /= playerKart.transform.lossyScale.y;
        relativeVelocity.z /= playerKart.transform.lossyScale.z;

        lastRelativeAcceleration = relativeVelocity - lastRelativeVelocity;

        lastRelativePosition = playerKart.transform.InverseTransformPoint(transform.position);

        //if (GetComponent<VRTK_InteractableObject>())
        //{
        //    if(GetComponent<VRTK_InteractableObject>().holdButtonToGrab && GetComponent<VRTK_InteractableObject>().IsGrabbed())
        //    {
        //        print("grab, " + transform.position);
        //        transform.position = GetComponent<VRTK_BaseJointGrabAttach>().controllerAttachPoint.position;
        //        print(transform.position);
        //    }
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "ObjectDetector")
        {
            if (transform.name == "LaserPointer")
            {
#if UNITY_EDITOR
                //UnityEditor.EditorApplication.isPaused = true;
#endif
                //print("laser enter cart");
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.name == "ObjectDetector")
        {
            if (transform.name == "LaserPointer")
            {
#if UNITY_EDITOR
                //UnityEditor.EditorApplication.isPaused = true;
#endif
                //print("laser stay cart");
            }

            inKart = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //print("exit kart");

        if (other.name == "ObjectDetector")
        {
            inKart = false;
        }
    }
}
