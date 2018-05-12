using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeRelatedReferences : MonoBehaviour
{
    public GameObject gun; // Player's default pistol weapon
    public GameObject shield; // Player's default shield
    public GameObject sword; // Player's default melee weapon
    public float slowTimeFactor; // How slow the time will be compare to the normal time (0.5 means the time will goes half as slow)
    public float switchTime; // The time duration for the open or close sword animation
    public bool slowMo; // Does the melee trigger slow-mo or not
    public GameObject gunModel; // Player's gun model used for transform animation
    public Transform gunBarrel; // Player's gun barrel model used for transform animation
    public GameObject slapEnemySounds; // The game object that contains the audio sources to be play automatically when an enemy is slapped

    public bool wasPistol; // Was the player using pistol or shield before he switch to the sword?
                           // When the player stops using sword, it will change back to the last item (shield or pistol)
    public float swordLength; // The length of the sword
    public Coroutine openSwordCoroutine; // The coroutine for opening the sword
    public Coroutine closeSwordCoroutine; // The coroutine for closing the sword
    public bool swordOpened; // Is the sword actually opened?

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
