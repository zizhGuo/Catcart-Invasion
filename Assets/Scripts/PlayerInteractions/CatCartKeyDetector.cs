using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCartKeyDetector : MonoBehaviour
{
    public Vector3 keyPositionWhenPlugged; // The position to keep the key when it is plugged in the keyhole
    public Vector3 keyEulerWhenPlugged; // The euler angles of the key when it is plugged in the keyhole

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "CatPawStick")
        {
            other.transform.parent = transform;
            other.transform.localPosition = keyPositionWhenPlugged;
            other.transform.localEulerAngles = keyEulerWhenPlugged;
            other.transform.parent = transform.parent;
            gameObject.SetActive(false);
        }
    }
}
