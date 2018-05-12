using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detect if the player's catgun's laser sight is shooting at an enemy
/// </summary>
public class DetectLaserSightHitEnemy : MonoBehaviour
{
    public LayerMask layerShootByLaserSight; // Collision layer that can detect laser sight

    public EnemyInfoOnUI currentAimingObject; // The gameobject that is currently being aimed

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LaserAim();
    }
    
    /// <summary>
    /// Cast a ray to detect if the player is looking at a gazable gameobject
    /// </summary>
    public void LaserAim()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * 100);

        // If the laser ray hit an enemy
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerShootByLaserSight) &&
            hit.transform.GetComponent<EnemyInfoOnUI>()) 
        {
            //print(hit.collider.name);

            if (currentAimingObject != null && currentAimingObject != hit.transform.GetComponent<EnemyInfoOnUI>()) // If the ray is hit onto another object immediately after it leaves the previous object
            {
                currentAimingObject.isAimedByPlayerGun = false; // Tell the previous that it's not being aimed
            }

            currentAimingObject = hit.transform.GetComponent<EnemyInfoOnUI>(); // Store the aimed object
            currentAimingObject.isAimedByPlayerGun = true; // Tell the object that it's being gazed
        }
        else // If the gazing ray is not hitting any gameobject
        {
            if (currentAimingObject != null) // If the ray just left an object
            {
                currentAimingObject.isAimedByPlayerGun = false; // Tell the object that it's not being gazed
                currentAimingObject = null; // Clear the current gazing object
            }
        }
    }
}
