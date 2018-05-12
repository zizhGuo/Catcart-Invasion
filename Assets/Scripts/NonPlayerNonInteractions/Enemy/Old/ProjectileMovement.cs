using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [HideInInspector] public float moveSpeed = 8;        //this speed is provided by the shooter Drone
    public GameObject particleSystem;
    GameObject systemParticle;
    public float lifeSpawn = 6f;

    public bool isReflected;
    //public GameObject playerHead;

    // Use this for initialization
    void Start()
    {   //instantiating the particle effect to be the child of the projectile
        systemParticle = Instantiate(particleSystem, transform.position, Quaternion.identity);
        systemParticle.transform.parent = gameObject.transform;
        isReflected = false;

        //playerHead = FindObjectOfType<GameManager>().playerHead;
        //playerHead = FindObjectOfType<testMovement>().gameObject;
        //transform.LookAt(playerHead.transform);

        GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed, ForceMode.Impulse);       //Proper way to move projectiles

        Destroy(gameObject, lifeSpawn);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void move()
    {   //Making the projectile move in a certain direction
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player" || col.collider.tag == "HUDwhole" || col.collider.tag == "HUDglass")
        {
            FindObjectOfType<GameManager>().leftController.GetComponent<HandPickItems>().dropItem();
            FindObjectOfType<GameManager>().rightController.GetComponent<HandPickItems>().dropItem();
            Destroy(gameObject);
        }

        if (col.collider.tag == "Enemy" && isReflected)
        {
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }
}
