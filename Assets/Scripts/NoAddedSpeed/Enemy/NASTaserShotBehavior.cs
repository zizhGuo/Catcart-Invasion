using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NASTaserShotBehavior : MonoBehaviour
{
    public GameObject particleSystem;
    GameObject systemParticle;
    public float lifeSpawn;

    public bool isReflected;
    public float moveSpeed;        //this speed is provided by the shooter Drone
    public GameObject playerKart; // Player side kart
    public Vector3 delayedPlayerVelocity; // The velocity the Taser shot is adjusting to
    //public GameObject playerHead;

    // Use this for initialization
    void Start()
    {   //instantiating the particle effect to be the child of the projectile
        //systemParticle = Instantiate(particleSystem, transform.position, Quaternion.identity);
        //systemParticle.transform.parent = gameObject.transform;
        isReflected = false;
        playerKart = FindObjectOfType<GameManager>().playerKart;

        //playerHead = FindObjectOfType<GameManager>().playerHead;
        //playerHead = FindObjectOfType<testMovement>().gameObject;
        //transform.LookAt(playerHead.transform);

        //GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed, ForceMode.VelocityChange);       //Proper way to move projectiles

        Destroy(gameObject, lifeSpawn);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isReflected)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed, ForceMode.VelocityChange);       // Add projectile's own speed
            GetComponent<Rigidbody>().AddForce(delayedPlayerVelocity, ForceMode.VelocityChange);       // Align the projectile speed with player's speed
        }

        StartCoroutine(delayingVelocity(GameManager.kartLastVelocity));
    }

    public IEnumerator delayingVelocity(Vector3 v)
    {
        yield return new WaitForSeconds(0.1f);
        delayedPlayerVelocity = v;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            //print(col.collider.tag);
            if(col.gameObject.GetComponent<PlayerInfo>()) // If it shot at head
            {
                PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();

                if (!player.isInvulnerable) // If the player is not invulnerable
                {
                    player.effectiveShots.Add(Time.time); // Add current shot to the list for all the shots player've taken

                    if(player.effectiveShots.Count >= player.continuousShotToDisarm) // If player've taken enough continuous shots
                    {
                        player.lastDisarmTime = Time.time;
                        player.isInvulnerable = true;

                        if (FindObjectOfType<GameManager>().leftController.GetComponent<HandPickItems>().currentItemName == "PlayerWeapon")
                        {
                            FindObjectOfType<GameManager>().leftController.GetComponent<HandPickItems>().dropItem();
                        }

                        if (FindObjectOfType<GameManager>().rightController.GetComponent<HandPickItems>().currentItemName == "PlayerWeapon")
                        {
                            FindObjectOfType<GameManager>().rightController.GetComponent<HandPickItems>().dropItem();
                        }
                    }
                }
            }

            Destroy(gameObject);
        }

        if (col.collider.tag == "Enemy" && isReflected)
        {
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }
}