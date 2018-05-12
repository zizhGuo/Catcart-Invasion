using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public BossBehavior bossStatus; // 

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (bossStatus.started)
        {
            moveBoss();
        }
    }

    public void moveBoss() // Make the boss move
    {
        bossStatus.distanceFromPlayer = Vector3.Distance(transform.position, bossStatus.playerkart.transform.position);

        if (bossStatus.isSlowed) // If the boss is slowed by its own missile
        {
            if (bossStatus.distanceFromPlayer <= bossStatus.catPlacementDistance - 1) // If the player is too close
            {
                bossStatus.targetSpeed = bossStatus.chargeSpeed;
                chargeBack();

                ///
                /// Temporary boss behavior
                ///
                if (bossStatus.bossHealth == 0)
                {
                    bossStatus.targetSpeed = bossStatus.defaultSpeed;
                }
            }
        }
        else if (bossStatus.isChargingBack) // If the boss is rapidly charging back
        {
            if (bossStatus.distanceFromPlayer >= bossStatus.maxDistanceToStopCharging)
            {
                bossStatus.isChargingBack = false;
                bossStatus.targetSpeed = bossStatus.defaultSpeed;

                ///
                /// Temporary boss behavior
                ///
                if (bossStatus.bossHealth == 0)
                {
                    bossStatus.isSlowed = false;
                }
            }
        }
        else if (bossStatus.distanceFromPlayer <= bossStatus.minDistanceToChargeBack) // If the boss is close to the player
        {
            bossStatus.targetSpeed = 60.01f;
            chargeBack();
        }
        else if (bossStatus.distanceFromPlayer >= bossStatus.minDistanceToSlowDown) // If the player is too far behind then the boss slow down to let player catch up
        {
            bossStatus.targetSpeed = Mathf.Clamp(bossStatus.defaultSpeed - (bossStatus.distanceFromPlayer - bossStatus.minDistanceToSlowDown) * bossStatus.slowForCatchUpVelocityRatio, 0, bossStatus.defaultSpeed);
        }

        if (GetComponent<Rigidbody>().velocity.magnitude < bossStatus.targetSpeed)
        {
            GetComponent<Rigidbody>().AddForce(transform.forward.normalized * bossStatus.bossAcceleration, ForceMode.Acceleration);
            //print("should acc, " + GetComponent<Rigidbody>().velocity);
        }
        else if (GetComponent<Rigidbody>().velocity.magnitude > bossStatus.targetSpeed && Vector3.Angle(GetComponent<Rigidbody>().velocity, transform.forward) < 90)
        {
            GetComponent<Rigidbody>().AddForce(-transform.forward.normalized * bossStatus.bossAcceleration, ForceMode.Acceleration);
        }
    }

    public void chargeBack() // let boss charge back
    {
        bossStatus.isSlowed = false;
        bossStatus.isChargingBack = true;
        //targetSpeed = chargeSpeed;

        ///
        /// Temporary boss behavior
        ///
        if (bossStatus.bossHealth == 0 && bossStatus.targetSpeed == bossStatus.defaultSpeed)
        {
            bossStatus.isSlowed = true;
        }
    }
}
