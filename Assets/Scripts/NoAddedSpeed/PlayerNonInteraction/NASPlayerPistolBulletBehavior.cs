using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NASPlayerPistolBulletBehavior : MonoBehaviour
{
    /// <summary>
    /// The bullet will just travel as if the kart is not moving at all
    /// </summary>

    public float initialMoveSpeed; // The move speed of the pistol bullet
    public float turnSpeed; // The turn speed of the pistol bullet (the faster the quicker the bullet will turn to the target
    public float liveTime; // How long the bullet will exist before diappear
    public float homingRangeAngle; // If the angle between the shoot direction and the enemy is too large then the bullet won't chase this enemy
    public bool homingByDistance; // If the bullet will seek the nearest target or the target that has the least angle difference in between
    public float accel; // The acceleration force for the bullet
    public float maxSpeed; // The fastest the bullet can travel

    public float homingRangeDist; // If the distance between the shoot direction and the enemy is too large then the bullet won't chase this enemy
    public GameObject target; // The target the bullet is chasing to
    public EnemyInfoOnUI[] targets; // The list contains all enemies
    public float nearestDist; // The nearest enemy to the bullet
    public Quaternion currentRotation; // The current rotation of the bullet
    public Quaternion targetRotation; // The target rotation of the bullet
    public float angleDiff; // The angle between the bullet's current rotation and the target rotation (which is the rotation where the bullet lookAt the target)
    public float leastAngleDiff; // The enemy that is the closest to the shooting direction

    // Use this for initialization
    void Start()
    {
        homingRangeDist = maxSpeed * liveTime;
        nearestDist = Mathf.Pow(homingRangeDist, 2);
        targets = FindObjectsOfType<EnemyInfoOnUI>(); // Include all the existing enemy in an array
        leastAngleDiff = homingRangeAngle;

        foreach (EnemyInfoOnUI t in targets)
        {
            float targetAngle = Vector3.Angle(transform.InverseTransformDirection(transform.forward), (transform.InverseTransformPoint(t.transform.position)));
            //print("Enemy angle: " + targetAngle);
            if (targetAngle <= homingRangeAngle) // If the initial direction is not too far off the target direction
            {
                if (homingByDistance)
                {
                    if ((t.transform.position - transform.position).sqrMagnitude < nearestDist) // If the distance between the bullet and the target is the nearest amongst all the enemies in the array
                    {
                        nearestDist = (t.transform.position - transform.position).sqrMagnitude;
                        target = t.gameObject; // Set the target to be this enemy
                    }
                }

                else
                {
                    if (targetAngle <= leastAngleDiff)
                    {
                        leastAngleDiff = targetAngle;
                        target = t.gameObject; // Set the target to be this enemy
                    }
                }
            }
        }

        GetComponent<Rigidbody>().velocity = transform.forward * initialMoveSpeed; // Give the bullet initial speed

        Destroy(gameObject, liveTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        float currentSpeed; // The current speed of the bullet

        if (target == null) // If the bullet don't find any target then move straight
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * accel, ForceMode.Acceleration);
            currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            GetComponent<Rigidbody>().velocity = transform.forward * currentSpeed;
            return;
        }

        else
        {
            Vector3 targetPosition = target.transform.position + target.GetComponent<Rigidbody>().velocity *
                                     (1 - (Vector3.Angle(transform.InverseTransformPoint(target.transform.position + target.GetComponent<Rigidbody>().velocity - GetComponent<Rigidbody>().velocity),
                                                         transform.InverseTransformDirection(transform.forward)) / 180f)); // Get the position the bullet is aiming at 
                                                                                                                           //(It will change as the relative speed to the bullet changes)
                                                                                                                           //print("Aiming target angle: " + Vector3.Angle(transform.InverseTransformPoint(target.transform.position + target.GetComponent<Rigidbody>().velocity - GetComponent<Rigidbody>().velocity),
                                                                                                                           //                                              transform.InverseTransformDirection(transform.forward)));

            targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

            angleDiff = Vector3.Angle(currentRotation.eulerAngles, targetRotation.eulerAngles); // Calculate the angle between current rotation and target rotation

            GetComponent<Rigidbody>().AddForce(transform.forward * accel, ForceMode.Acceleration);
            currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                                            Mathf.Clamp((angleDiff / turnSpeed) * 2f /
                                                                                        (Vector3.Distance(targetPosition, transform.position) /
                                                                                        ((transform.forward * currentSpeed -
                                                                                          target.GetComponent<Rigidbody>().velocity).magnitude)),
                                                                                        0.05f, 1) * turnSpeed * Time.fixedUnscaledDeltaTime)); // Calculate how much the bullet should turn in this fixed update

            GetComponent<Rigidbody>().velocity = transform.forward * currentSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {

    }
}