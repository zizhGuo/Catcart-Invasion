using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGetHit : MonoBehaviour
{
    public BossBehavior bossInfo;

    public Vector3 initialPosition; // The initial local position
    public Quaternion initialRotation; // The initial local rotation

    // Use this for initialization
    void Start()
    {
        bossInfo = GetComponentInParent<BossBehavior>();
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }

    void FixedUpdate()
    {
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        //print(collision.gameObject.name);

        if (collision.gameObject.GetComponent<NASCatcherEnemyBehavior>()) // It's hit by a catcher drone
        {
            if (collision.gameObject.GetComponent<NASCatcherEnemyBehavior>().isSlapped)
            {
                //print("disable shield");

                Destroy(collision.gameObject);
                bossInfo.turnOffShield();
            }
        }
        if (collision.gameObject.GetComponent<DrillerMissileBehavior>()) // It's hit by a driller missile
        {
            bossInfo.targetSpeed = bossInfo.defaultSpeed *0.6f;
            bossInfo.isSlowed = true;
            //Instantiate(collision.gameObject.GetComponent<DrillerMissileBehavior>().shockAttack.GetComponent<ShockBehavior>().shockParticle, collision.transform.position, collision.transform.rotation);
            Destroy(collision.gameObject);
        }
    }
}
