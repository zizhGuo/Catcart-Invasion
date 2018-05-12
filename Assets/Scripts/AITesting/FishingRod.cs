using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour {

    private Vector3 parentVelocity;
    private float parentSpeed;
    private Rigidbody rb;

    public float fishingDistanceMultiplier = 1;
    public float fishingOffset = 10;

	// Use this for initialization
	void Start () {

        rb = GetComponentInParent<Collider>().attachedRigidbody;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        parentVelocity = rb.velocity;
        parentSpeed = parentVelocity.magnitude;
        this.transform.localPosition = new Vector3(0, 0, fishingDistanceMultiplier*(fishingOffset + parentSpeed));

	}
}
