using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlayerKart : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += GameManager.kartDeltaPosition;
    }
}
