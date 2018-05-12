using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockBehavior : MonoBehaviour
{
    public GameObject shockParticle; // The particle effect used for shock effect
    public bool missileShock; // If the shock is coming from missile (then there will be no delay for the shock)

    public float shockLastingTime; // How long will a single shock last
    public bool activated; // Has the attack procedure already been activated?
    public float shockDelay; // How much time the attack will wait to activate after it is triggered (so the player has some time to respond)
    public bool playerStayed; // If the player is still within the shock range after it is triggered
    public SmallEnemyDie parentDie;
    public GameObject parentBehavior;
    public MoveWithPlayerKart parentMoveWithPlayer;
    public bool isSlapped;
    public bool exploded; // Has is actually exploded

    // Use this for initialization
    void Start()
    {
        activated = false;

        if (!missileShock) // If this is not the missile
        {
            parentBehavior = GetComponentInParent<NASDrillerEnemyBehavior>().gameObject;
        }
        else
        {
            parentBehavior = GetComponentInParent<DrillerMissileBehavior>().gameObject;
        }

        parentDie = GetComponentInParent<SmallEnemyDie>();
        parentMoveWithPlayer = GetComponentInParent<MoveWithPlayerKart>();
        isSlapped = false;
        exploded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (missileShock)
        {
            return;
        }

        if (activated)
        {
            if (GetComponentInParent<Rigidbody>().velocity.y < 0 && GetComponentInParent<Rigidbody>().useGravity)
            {
                //GetComponentInParent<Rigidbody>().velocity = transform.parent.forward * 2f;
                GetComponentInParent<Rigidbody>().useGravity = false;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!activated && other.tag == "Player")
        {
            if (!missileShock) // If this is not the missile
            {
                parentBehavior.GetComponent<NASDrillerEnemyBehavior>().areaHint.SetActive(false);
                parentBehavior.GetComponent<NASDrillerEnemyBehavior>().enabled = false;
            }
            else
            {
                parentBehavior.GetComponent<DrillerMissileBehavior>().areaHint.SetActive(false);
                parentBehavior.GetComponent<DrillerMissileBehavior>().enabled = false;
            }

            activated = true;
            playerStayed = true;
            StartCoroutine(activateShock());
            //transform.parent.GetComponent<Rigidbody>().AddForce(transform.parent.forward * 0.5f, ForceMode.VelocityChange);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (activated && other.tag == "Player")
        {
            playerStayed = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (activated && other.tag == "Player")
        {
            playerStayed = false;
        }
    }

    public IEnumerator activateShock()
    {
        if (!missileShock) // Play driller drone animation if it is the driller not the missile
        {
            parentMoveWithPlayer.enabled = true;
            GetComponentInParent<Rigidbody>().velocity = Vector3.zero;
            GetComponentInParent<Rigidbody>().AddForce(transform.up * GetComponentInParent<NASDrillerEnemyBehavior>().moveSpeedDuringJump * 1.5f, ForceMode.VelocityChange);
            GetComponentInParent<Rigidbody>().AddForce(transform.parent.forward * GetComponentInParent<NASDrillerEnemyBehavior>().moveSpeedDuringJump * 0.8f, ForceMode.VelocityChange);
            GetComponentInParent<Rigidbody>().useGravity = true;
        }

        yield return new WaitForSeconds(shockDelay);

        if (!isSlapped && transform.parent != null)
        {
            if (playerStayed && GameManager.kartMovementInfo != null)
            {
                GameManager.kartMovementInfo.isKartShocked = true;
                GameManager.kartMovementInfo.shockLastingTime = shockLastingTime;
                GameManager.kartMovementInfo.lastShockedTime = Time.time;
                //GameManager.kartMovementInfo.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);
                GameManager.sLeftController.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);
                GameManager.sRightController.GetComponent<HandPickItems>().controllerActions.TriggerHapticPulse(0.4f, 0.25f, 0.01f);
            }

            exploded = true;
            Instantiate(shockParticle, transform.position, transform.rotation);
        }

        yield return null;

        if (!missileShock)
        {
            parentDie.enabled = false;
        }

        Destroy(transform.parent.gameObject);
    }
}
