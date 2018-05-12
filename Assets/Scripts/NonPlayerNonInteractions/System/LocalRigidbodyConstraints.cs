using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Freeze a rigidbody's movement along x/y/z axis in local space
/// </summary>
public class LocalRigidbodyConstraints : MonoBehaviour
{
    public bool freezeX;
    public bool freezeY;
    public bool freezeZ;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);

        if (freezeX)
        {
            localVelocity.x = 0;
        }
        if (freezeY)
        {
            localVelocity.y = 0;
        }
        if (freezeZ)
        {
            localVelocity.z = 0;
        }

        GetComponent<Rigidbody>().velocity = transform.TransformDirection(localVelocity);
    }
}
