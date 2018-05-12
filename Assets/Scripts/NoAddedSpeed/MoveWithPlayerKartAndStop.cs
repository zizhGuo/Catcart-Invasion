using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlayerKartAndStop : MonoBehaviour
{
    /// <summary>
    /// In order to make this work the gameobject this attached to must have a rigidbody
    /// </summary>

    public Vector3 initialRelativePosition;
    public GameObject playerKart; // Player side kart;

    // Use this for initialization
    public void Start()
    {
        //playerKart = FindObjectOfType<GameManager>().playerKart;
        //initialRelativePosition = transform.position - playerKart.transform.position;
        GetComponent<Rigidbody>().AddForce(GameManager.kartCurrentVelocity, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
