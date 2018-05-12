using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerKartSyncRotation : MonoBehaviour
{


    public GameObject playerKart; // The player side cat kart

    // Use this for initialization
    void Start()
    {
        playerKart = FindObjectOfType<GameManager>().playerKart;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = playerKart.transform.rotation;
    }
}
