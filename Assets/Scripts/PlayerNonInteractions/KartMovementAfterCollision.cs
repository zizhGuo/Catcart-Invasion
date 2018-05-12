using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class KartMovementAfterCollision : MonoBehaviour
{
    public float maxVelocityChangeAfterCollision; // The maximum velocity change applies to the player's cart if the player collides head-on
    public float bonusAccelForPro; // Acceleration rewarded for players intentionally bounce off the wall to move beyond maximum speed
    public AudioClip cartHitSFX; // The clip to be played when the cart hit something

    public VRTK_ControllerActions[] controllerActions; // The controller actions on both controllers

    // Use this for initialization
    void Start()
    {
        controllerActions = FindObjectsOfType<VRTK_ControllerActions>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().AddForce(maxVelocityChangeAfterCollision * //collision.contacts[0].normal
                                                                             Vector3.Reflect(GetComponent<Rigidbody>().velocity.normalized, collision.contacts[0].normal)
                                           * Mathf.Clamp((90 - Vector3.Angle(GetComponent<Rigidbody>().velocity.normalized, collision.contacts[0].normal)) / 90f, (bonusAccelForPro / maxVelocityChangeAfterCollision), 1)
                                           , ForceMode.VelocityChange);

        foreach (VRTK_ControllerActions controller in controllerActions)
        {
            controller.TriggerHapticPulse(1f, 0.4f, 0.02f);
        }

        // Play the cart hitting sound effect
        AudioSource.PlayClipAtPoint(cartHitSFX, GameManager.gameManager.catCartSFX.position);

        //print("Collision angle: " + (90 - Vector3.Angle(GetComponent<Rigidbody>().velocity.normalized, collision.contacts[0].normal)));
        //print("Velocity angle: " + Vector3.Angle(GetComponent<Rigidbody>().velocity.normalized, transform.forward) + ", velocity: " + GetComponent<Rigidbody>().velocity);
    }
}
