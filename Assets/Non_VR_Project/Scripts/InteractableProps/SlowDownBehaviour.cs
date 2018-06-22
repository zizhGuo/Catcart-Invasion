using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownBehaviour : MonoBehaviour {

    [SerializeField] float shockLastingTime = 5f;
    [SerializeField] ParticleSystem shockParticle;
    [SerializeField] bool exploded;
    // Use this for initialization
    void Start () {
        exploded = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Player") {
            GameManager.kartMovementInfo.isKartShocked = true;
            GameManager.kartMovementInfo.shockLastingTime = shockLastingTime;
            GameManager.kartMovementInfo.lastShockedTime = Time.time;
            //GameManager.kartMovementInfo.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);
            GameManager.sLeftController.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);
            GameManager.sRightController.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);

            exploded = true;
            Instantiate(shockParticle, transform.position, transform.rotation);
        }


        // Plays the slow sound effect when the cart is shocked
        //AudioSource.PlayClipAtPoint(cartSlowSFX, GameManager.gameManager.catCartSFX.position);
    }
}
