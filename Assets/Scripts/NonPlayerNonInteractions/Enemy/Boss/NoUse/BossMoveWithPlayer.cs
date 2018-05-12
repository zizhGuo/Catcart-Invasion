using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMoveWithPlayer : MonoBehaviour
{
    

    public Vector3 bossDeltaPosition; // How much the boss move each frame
    public bool beginMove; // Does the boss fight begins

    // Use this for initialization
    void Start()
    {
        bossDeltaPosition = Vector3.zero;
        beginMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (beginMove)
        {
            bossDeltaPosition.x = GameManager.kartDeltaPosition.x;

            transform.position += bossDeltaPosition;
        }
    }
}
