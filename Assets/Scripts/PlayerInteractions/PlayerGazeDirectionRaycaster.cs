using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGazeDirectionRaycaster : MonoBehaviour
{
    public LayerMask layerCanBeGazed; // Collision layer that can be gazed on

    public CheckIfGazed currentGazingObject; // The gameobject that is currently being gazed

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Gaze();
    }

    /// <summary>
    /// Cast a ray to detect if the player is looking at a gazable gameobject
    /// </summary>
    public void Gaze()
    {
        RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.forward);

        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerCanBeGazed)) // If the gazing ray hit a gazable object
        {
            //print(hit.collider.name);

            if(currentGazingObject != null && currentGazingObject != hit.transform.gameObject.GetComponentInChildren<CheckIfGazed>()) // If the ray is hit onto another object immediately after it leaves the previous object
            {
                currentGazingObject.isBeingGazed = false; // Tell the previous that it's not being gazed
            }

            currentGazingObject = hit.transform.gameObject.GetComponentInChildren<CheckIfGazed>(); // Store the gazed object
            currentGazingObject.isBeingGazed = true; // Tell the object that it's being gazed
        }
        else // If the gazing ray is not hitting any gameobject
        {
            if (currentGazingObject != null) // If the ray just left an object
            {
                currentGazingObject.isBeingGazed = false; // Tell the object that it's not being gazed
                currentGazingObject = null; // Clear the current gazing object
            }
        }
    }
}
