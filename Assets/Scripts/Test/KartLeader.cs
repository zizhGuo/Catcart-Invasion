using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartLeader : MonoBehaviour
{
    public GameObject catKart;

    public float maximumSpeed; //As long as the player is using laser pointer to guide the Kart, the kart will not move faster than this speed.
    public float minimumSpeed; //As long as the player is using laser pointer to guide the Kart, the kart will not move slower than this speed.
    public float acceleration;
    public float dis2speedFactor; //The distance between the pointer and the kart will be divided by this number and then apply to the target speed.
    public AudioSource engineSFX;

    public float currentSpeed;
    public float targetSpeed;
    public bool isLaserActive;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentSpeed = catKart.GetComponent<Rigidbody>().velocity.magnitude; //Get the current speed of the kart regardless of direction.
        targetSpeed = Mathf.Clamp((Vector3.Distance(transform.position, catKart.transform.position) / dis2speedFactor), minimumSpeed, maximumSpeed); //Get the target speed based on the distance between the laser pointer and the kart, regardless of direction.
        catKart.transform.LookAt(transform);
        catKart.GetComponent<Rigidbody>().velocity = (catKart.transform.forward.normalized) * currentSpeed; //Turn the kart to where the pointer currently aiming at.

        if (currentSpeed < targetSpeed)
        {
            catKart.GetComponent<Rigidbody>().AddForce((catKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
        }
        else if (currentSpeed > targetSpeed && Vector3.Angle(catKart.GetComponent<Rigidbody>().velocity, catKart.transform.forward) <= 30)
        {
            //print(Vector3.Angle(catKart.GetComponent<Rigidbody>().velocity, catKart.transform.forward));
            catKart.GetComponent<Rigidbody>().AddForce(-(catKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
        }
    }
}
