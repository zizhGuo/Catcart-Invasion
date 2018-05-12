using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPistolBulletBehaviorDefaultPhysics : MonoBehaviour
{
    /// <summary>
    /// Make the bullet fly faster
    /// When calculating the turn speed, don't just calculate the bullet speed, but also the enemy's speed
    /// Bullet is not destroying itself now
    /// 
    /// For each fixedupdate, decrease the added initial velocity, 
    ///                       give the bullet a forward force, 
    ///                       rotate the bullet,
    ///                       take the bullet's current velocity and get the speed as float,
    ///                       cap the speed if it exceeds the maximum speed,
    ///                       take the bullet's current speed and convert it to the bullet's forward direction as the new velocity, 
    ///                       add the decreased initial velocity.
    /// </summary>

    public float initialMoveSpeed; // The move speed of the pistol bullet
    public float turnSpeed; // The turn speed of the pistol bullet (the faster the quicker the bullet will turn to the target
    public float liveTime; // How long the bullet will exist before diappear
    public float homingRangeAngle; // If the angle between the shoot direction and the enemy is too large then the bullet won't chase this enemy
    public float accel; // The acceleration force for the bullet
    public float maxSpeed; // The fastest the bullet can travel

    public float homingRangeDist; // If the distance between the shoot direction and the enemy is too large then the bullet won't chase this enemy
    public GameObject target; // The target the bullet is chasing to
    public EnemyInfoOnUI[] targets; // The list contains all enemies
    public Quaternion currentRotation; // The current rotation of the bullet
    public Quaternion targetRotation; // The target rotation of the bullet
    public float angleDiff; // The angle between the bullet's current rotation and the target rotation (which is the rotation where the bullet lookAt the target)
    public float leastAngleDiff; // The enemy that is the closest to the shooting direction
    public Vector3 initialVelocity; // The initial velocity of the bullet (So the bullet will fly away even if the player is moving)

    // Use this for initialization
    void Start()
    {
        turnSpeed *= GameManager.sSpeedMultiplier;
        accel *= GameManager.sSpeedMultiplier;
        maxSpeed *= GameManager.sSpeedMultiplier;

        homingRangeDist = (maxSpeed * GameManager.sSpeedMultiplier) * liveTime;
        targets = FindObjectsOfType<EnemyInfoOnUI>(); // Include all the existing enemy in an array
        leastAngleDiff = homingRangeAngle;

        foreach (EnemyInfoOnUI t in targets)
        {
            float targetAngle = Vector3.Angle(transform.InverseTransformDirection(transform.forward), (transform.InverseTransformPoint(t.transform.position)));
            if (targetAngle <= homingRangeAngle) // If the initial direction is not too far off the target direction
            {
                    if (targetAngle <= leastAngleDiff)
                    {
                        leastAngleDiff = targetAngle;
                        target = t.gameObject; // Set the target to be this enemy
                    }
            }
        }

        GetComponent<Rigidbody>().velocity = transform.forward * initialMoveSpeed; // Give the bullet initial speed
        
        GetComponent<Rigidbody>().velocity += initialVelocity; // Add the player's current speed to the bullet

        Destroy(gameObject, liveTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        /// For each fixedupdate, decrease the added initial velocity, 
        ///                       give the bullet a forward force, 
        ///                       rotate the bullet,
        ///                       take the bullet's current velocity and get the speed as float,
        ///                       cap the speed if it exceeds the maximum speed,
        ///                       take the bullet's current speed and convert it to the bullet's forward direction as the new velocity, 
        ///                       add the decreased initial velocity.

        float currentSpeed; // The current speed of the bullet

        if (target == null) // If the bullet don't find any target then move straight
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * accel, ForceMode.Acceleration);
            currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            if(GetComponent<Rigidbody>().velocity.magnitude > maxSpeed) // If the speed of the bullet exceeds the max speed, then clamp it within the max speed
            {
                GetComponent<Rigidbody>().velocity *= (maxSpeed / GetComponent<Rigidbody>().velocity.magnitude);
            }

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

            currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                                            Mathf.Clamp((angleDiff / turnSpeed) * 2f /
                                                                                        (Vector3.Distance(targetPosition, transform.position) /
                                                                                        ((transform.forward * (currentSpeed) -
                                                                                          target.GetComponent<Rigidbody>().velocity).magnitude)),
                                                                                        0.05f, 1) * turnSpeed * Time.fixedUnscaledDeltaTime)); // Calculate how much the bullet should turn in this fixed update

            GetComponent<Rigidbody>().AddForce(transform.forward * accel, ForceMode.Acceleration);
            currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            if (GetComponent<Rigidbody>().velocity.magnitude > maxSpeed) // If the speed of the bullet exceeds the max speed, then clamp it within the max speed
            {
                GetComponent<Rigidbody>().velocity *= (maxSpeed / GetComponent<Rigidbody>().velocity.magnitude);
            }
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
}