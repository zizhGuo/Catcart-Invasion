using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class testMovement : MonoBehaviour {

    public float speed;

    private Rigidbody rb;
    private float moveHorizontal;
    private float moveVertical;
    private Vector3 movement;

    private NavMeshAgent nav;

	// Use this for initialization
	void Start () {
        //transform.GetComponent<Rigidbody>().velocity = transform.forward.normalized * speed;

        rb = GetComponent<Rigidbody>();
        //nav = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void FixedUpdate()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        transform.Rotate(new Vector3(0, moveHorizontal * 200, 0) * Time.deltaTime);

        if(rb.velocity.magnitude > 100)
        {
            rb.velocity = 100 * transform.forward.normalized;
        }
        else
        {
            rb.velocity = rb.velocity.magnitude * transform.forward.normalized;
        }

        //nav.SetDestination(transform.position + movement);

        rb.velocity = rb.velocity + transform.forward.normalized * moveVertical;

    }
}
