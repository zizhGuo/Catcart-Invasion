using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaserWaveBehavior : MonoBehaviour
{
    /// <summary>
    /// Handles collisions with player and the taser wave's relative movement
    /// Taser wave is a type of boss attack
    /// </summary>

    public GameObject waveWrap; // The wrap of the wave that has the rigidbody
    public float moveSpeed; // The relative speed of the wave to the boss

    // Use this for initialization
    void Start()
    {
        waveWrap = GetComponentInParent<RandomLightningMovement>().gameObject;
        waveWrap.GetComponent<Rigidbody>().velocity = FindObjectOfType<BossBehavior>().GetComponent<Rigidbody>().velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (waveWrap.GetComponent<MoveWithBoss>().boss != null && waveWrap.GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            waveWrap.GetComponent<Rigidbody>().AddForce(-waveWrap.GetComponent<MoveWithBoss>().boss.transform.forward * moveSpeed, ForceMode.VelocityChange);
        }
        //else
        //{
        //    print(waveWrap.GetComponent<Rigidbody>().velocity);
        //}
    }

    void OnTriggerEnter(Collider col)
    {
        //print(col.name);

        if (col.tag == "Player")
        {
            if (col.gameObject.GetComponent<PlayerInfo>()) // If it shot at head
            {
                PlayerInfo player = col.gameObject.GetComponent<PlayerInfo>();

                //if (!player.isInvulnerable) // If the player is not invulnerable
                {
                    //player.effectiveShots.Add(Time.time); // Add current shot to the list for all the shots player've taken

                    //if (player.effectiveShots.Count >= player.continuousShotToDisarm) // If player've taken enough continuous shots
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
        }
    }
}