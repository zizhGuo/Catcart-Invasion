using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithBoss : MonoBehaviour
{
    /// <summary>
    /// Let some boss's attack move with the boss
    /// </summary>

    public GameObject boss; // The boss
    public Vector3 bossDeltaPosition; // The position boss changed from last frame
    public Vector3 bossLastPosition; // The position of the boss last frame;

    // Use this for initialization
    void Start()
    {
        boss = FindObjectOfType<BossBehavior>().gameObject;
        bossLastPosition = boss.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        bossDeltaPosition = boss.transform.position - bossLastPosition;

        transform.position += bossDeltaPosition;

        bossLastPosition = boss.transform.position;
    }
}
