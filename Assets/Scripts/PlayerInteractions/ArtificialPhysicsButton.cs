using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make a button follows a rigidbody collider on certain local axis with limitations
/// </summary>
public class ArtificialPhysicsButton : MonoBehaviour
{
    public Transform followedRigidbody; // The rigidbody collider it follows
    public bool moveX; // Move along local x axis
    public float minX; // Lowest local x coordinate can be
    public float maxX; // Maximum local x coordinate can be
    public bool moveY; // Move along local y axis
    public float minY; // Lowest local y coordinate can be
    public float maxY; // Maximum local y coordinate can be
    public bool moveZ; // Move along local z axis
    public float minZ; // Lowest local z coordinate can be
    public float maxZ; // Maximum local z coordinate can be

    public Vector3 initialLocalPosition;
    public Vector3 newLocalPosition;

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
        newLocalPosition = initialLocalPosition;

        if (moveX)
        {
            newLocalPosition.x = Mathf.Clamp(followedRigidbody.localPosition.x, minX, maxX);
        }
        if (moveY)
        {
            newLocalPosition.y = Mathf.Clamp(followedRigidbody.localPosition.y, minY, maxY);
        }
        if (moveZ)
        {
            newLocalPosition.z = Mathf.Clamp(followedRigidbody.localPosition.z, minZ, maxZ);
        }

        transform.localPosition = newLocalPosition;
    }
}
