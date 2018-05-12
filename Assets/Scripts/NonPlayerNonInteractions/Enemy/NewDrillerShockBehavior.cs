using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDrillerShockBehavior : MonoBehaviour
{
    public GameObject shockParticle; // The particle effect used for shock effect
    public AudioClip cartSlowSFX; // The clip to be played when the cart is shocked

    public float shockLastingTime; // How long will a single shock last
    public SmallEnemyDie parentDie;
    public GameObject parentBehavior;
    public MoveWithPlayerKart parentMoveWithPlayer;
    public bool isSlapped;
    public bool exploded; // Has is actually exploded

    // Use this for initialization
    void Start()
    {
        parentBehavior = GetComponentInParent<DrillerJumpBehavior>().gameObject;

        parentDie = GetComponentInParent<SmallEnemyDie>();
        parentMoveWithPlayer = GetComponentInParent<MoveWithPlayerKart>();
        isSlapped = false;
        exploded = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.kartMovementInfo.isKartShocked = true;
        GameManager.kartMovementInfo.shockLastingTime = shockLastingTime;
        GameManager.kartMovementInfo.lastShockedTime = Time.time;
        //GameManager.kartMovementInfo.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);
        GameManager.sLeftController.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);
        GameManager.sRightController.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);

        exploded = true;
        Instantiate(shockParticle, transform.position, transform.rotation);

        // Plays the slow sound effect when the cart is shocked
        AudioSource.PlayClipAtPoint(cartSlowSFX, GameManager.gameManager.catCartSFX.position);

        parentDie.enabled = false;
        Destroy(transform.parent.gameObject);
    }
}
