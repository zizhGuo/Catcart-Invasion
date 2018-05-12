using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DetectDoorClosing : MonoBehaviour
{
    public Vector3 doorPosition; //The localPosition for the door
    public GameObject door; //The door game object on the kart
    public DetectIfPlayerIsInKart playerDetector; //The detector for the player is on kart or not

    public bool isDoorClosed; //Is the door closed or not
    public Vector3 originalHingeAxis; //The defaultHingeAxis;

    // Use this for initialization
    void Start()
    {
        isDoorClosed = true;
        originalHingeAxis = door.GetComponent<HingeJoint>().axis;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDoorClosed)
        {
            if (!door.GetComponent<VRTK_InteractableObject>().IsGrabbed())
            {
                door.transform.localRotation = Quaternion.identity;
                door.transform.localPosition = doorPosition;
                door.GetComponent<HingeJoint>().axis = Vector3.zero;
            }
        }

        if(door.GetComponent<VRTK_InteractableObject>().IsGrabbed() && door.GetComponent<HingeJoint>().axis == Vector3.zero)
        {
            door.GetComponent<HingeJoint>().axis = originalHingeAxis;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "KartDoor" && !other.GetComponent<VRTK_InteractableObject>().IsGrabbed() && !isDoorClosed) //If the door is just being closed by the player
        {
            isDoorClosed = true;

            if (playerDetector.playerIsInKart && !DetectIfPlayerIsInKart.playerPlayArea.doFollow) //If the player is already in the kart
            {
                playerDetector.getInKart();
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "KartDoor" && !other.GetComponent<VRTK_InteractableObject>().IsGrabbed() && !isDoorClosed) //If the door is just being closed by the player
        {
            isDoorClosed = true;

            if (playerDetector.playerIsInKart && !DetectIfPlayerIsInKart.playerPlayArea.doFollow) //If the player is already in the kart
            {
                playerDetector.getInKart();
            }
        }

        if (other.tag == "KartDoor" && !other.GetComponent<VRTK_InteractableObject>().IsGrabbed() && isDoorClosed) //If the door is closed and the player is not grabbing it
        {
            door.transform.localRotation = Quaternion.identity;
            door.transform.localPosition = doorPosition;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "KartDoor")
        {
            isDoorClosed = false;
        }
    }

    //public IEnumerator takeOffRoutine()
    //{
    //    yield return new WaitForFixedUpdate();

    //    door.transform.parent = null;

    //    door.GetComponent<Rigidbody>().useGravity = true;
    //    door.GetComponent<Rigidbody>().isKinematic = false;

    //}
}
