using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedMoveWithPlayerKart : MonoBehaviour
{
    public float delayedTime; // How much delayed we want this object to follow player's kart

    public GameObject playerKart; // Player side kart
    public Vector3 delayedKartPosition; // The position the player's kart was at

    // Use this for initialization
    void Start()
    {
        playerKart = FindObjectOfType<GameManager>().playerKart;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
