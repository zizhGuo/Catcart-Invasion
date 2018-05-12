using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUIElement : MonoBehaviour
{
    public Vector3 rotationDirection; // The direction for the rotation

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationDirection * Time.deltaTime);
    }
}
