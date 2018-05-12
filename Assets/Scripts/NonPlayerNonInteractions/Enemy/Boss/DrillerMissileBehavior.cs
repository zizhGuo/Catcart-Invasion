using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerMissileBehavior : MonoBehaviour
{
    /// <summary>
    /// Driller missile's behavior:
    /// 1. Launch from the boss
    /// 2. Aim at a coordinate relative to the boss
    /// 3. Fly away from the boss (relative to the boss. so if the boss is relatively faster to the player, the missile should also be. 
    ///                            player should be able to drive ahead of the missile (or catch the missile and slap it to the boss before the missile is deployed under the ground)
    /// 4. After it drills into the ground it will stop move and turn on the area hint so player with HUD can see its area
    /// 5. If the player reaches its detecting range then the missile will detonate underground and release shockwave to temporarily slow or stop player's kart 
    ///    (unlike driller enemy the player won't be able to stop its attack if the player triggers it)
    /// </summary>

    public GameObject missileBody; // The body of the driller vehicle
    public GameObject areaHint; // The hint showing the driller detecting range if the player is wearing HUD
    public float diggingAltitude; // What's the driller enemy's Y axis when it is underground
    public float detectingRange; // How close the player's kart have to be to trigger the driller
    public float shockLastingTime; // How long will a single shock last
    public GameObject shockAttack; // The shock area the driller is going to release
    public float shockDelay; // How much time the attack will wait to activate after it is triggered (so the player has some time to respond)
    public float flySpeed; // How fast the driller missile will fly relate to the boss
    public Vector3 initialDirection; // Where the driller will initially look at
    public GameObject digInParticle; // The particle effect when the driller drone dig into the ground
    public Vector3 drillInLocation; // The target drill location relative to the boss
    public float turnSpeed; // How fast the missile can turn

    public GameObject playerKart; // The player side kart
    public bool isInPosition; // If the driller is in ground and deployed
    public GameObject boss; // The boss
    public Vector3 oldRotationEuler; // The rotation before look at the target location
    public Vector3 newRotationEuler; // The rotation for the missile after the current frame
    public Vector3 targetRotationEuler; // The target rotation that aim at the target location
    public float angleBetweenOldAndTargetEuler; // The angle between the old euler angles and target euler angles
    public Vector3 targetRotationForward; // The forward direction of the target euler angles

    // Use this for initialization
    void Start()
    {
        playerKart = FindObjectOfType<GameManager>().playerKart;
        shockAttack.GetComponent<ShockBehavior>().shockLastingTime = shockLastingTime;
        shockAttack.GetComponent<ShockBehavior>().shockDelay = shockDelay;
        GetComponentInChildren<DrillerAreaHintAnimation>(true).range = detectingRange / transform.lossyScale.x;
        shockAttack.transform.localScale /= transform.lossyScale.x / detectingRange;
        boss = FindObjectOfType<BossBehavior>().gameObject;

        isInPosition = false;

        transform.eulerAngles = initialDirection;

        drillInLocation = boss.transform.InverseTransformPoint(transform.position - boss.transform.forward * boss.GetComponent<BossBehavior>().missileForwardRange);
        drillInLocation.y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInPosition) // If the missile is not in position, that means the missile should still be flying
        {
            rotateMissile();
            moveMissile();
        }
        else // Else the missile will just look at player
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.LookAt(playerKart.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); // Keep it parallel to the ground
        }

        if (transform.position.y <= diggingAltitude && !isInPosition) // If the missile's 
        {
            isInPosition = true;
            areaHint.SetActive(true);
            shockAttack.SetActive(true);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<MoveWithBoss>().enabled = false;
        }
    }

    public void rotateMissile()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        oldRotationEuler = transform.eulerAngles; // Store the missile's current rotation
        transform.LookAt(boss.transform.TransformPoint(drillInLocation)); // Look at its target location
        targetRotationEuler = transform.eulerAngles; // Store its target rotation
        targetRotationForward = transform.forward; // Store the forward vector of the target rotation
        transform.eulerAngles = oldRotationEuler; // Change the rotation back to its current rotation

        angleBetweenOldAndTargetEuler = Vector3.Angle(transform.forward, targetRotationForward); // Calculate the angle between the current rotation and the target rotation
        newRotationEuler.x = Mathf.LerpAngle(oldRotationEuler.x, targetRotationEuler.x, Time.deltaTime * turnSpeed / angleBetweenOldAndTargetEuler); // Calculate the rotation after this frame
        //newRotationEuler.y = Mathf.LerpAngle(oldRotationEuler.y, targetRotationEuler.y, Time.deltaTime * turnSpeed / angleBetweenOldAndTargetEuler);
        newRotationEuler.y = 270;
        newRotationEuler.z = Mathf.LerpAngle(oldRotationEuler.z, targetRotationEuler.z, Time.deltaTime * turnSpeed / angleBetweenOldAndTargetEuler);

        transform.eulerAngles = newRotationEuler; // Change the rotation to the new rotation

        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void moveMissile()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * flySpeed;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") // If the missile collide with player or player cart
        {
            shockAttack.SetActive(true);
        }
    }
}
