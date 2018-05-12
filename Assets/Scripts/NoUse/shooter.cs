using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooter : MonoBehaviour
{

    public GameObject projectilePrefab;
    GameObject projectileInstance;

    public float waitTime;
    public Vector3 spawnLocation;
    public float multiplier;

    private float temp;
    

    // Use this for initialization
    void Start()
    {
        temp = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        //Determining the position for the projectile to be spawned, essentially in front of the enemy drone
        if (waitTime <= 0.0f)
        {
            spawnLocation = transform.position + transform.forward * multiplier;
            Instantiate(projectilePrefab, spawnLocation, transform.rotation);
            waitTime = temp;
        }
    }
}