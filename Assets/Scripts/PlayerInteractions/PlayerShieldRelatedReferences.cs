using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldRelatedReferences : MonoBehaviour
{
    /// <summary>
    /// The opening and closing of the shield will be determined by the controller's Z orientation. 
    /// Close to 0 means the player is holding the controller like a pistol, so the shield should close.
    /// Close to 90 means the player is holding the controller like a shield, so the shield should open.
    /// </summary>
    public GameObject controller; // The controller which is the weapon
    public GameObject head; // Player's head
    public GameObject shield; // Player's shield
    public float openShieldAngle; // If the angle between the controller and head is greater than this value then the shield will open
    public float closeShieldAngle; // If the angle between the controller and head is greater than this value then the shield will close
    public Vector3 shieldSize; // The size for the shield
    public GameObject gun; // Player's default pistol weapon
    public GameObject sword; // Player's default melee weapon
    public GameObject gunModel; // Player's gun model used for transform animation
    public GameObject[] gunBarrelRings; // The coil rings on the gun barrel
    public float animationTime; // The time duration for the open or close shield animation

    public bool isShield; // Is the shield currently opened or not
    public Vector3 localEular; // The controller's local orientation
    public Coroutine openShieldCoroutine; // The coroutine for opening the sword
    public Coroutine closeShieldCoroutine; // The coroutine for closing the sword

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
