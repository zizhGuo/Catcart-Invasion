using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAlleyChaserBehavior : MonoBehaviour
{
    public GameObject chaserEnemy; // The chaserEnemy
    public float distanceToActivate; // How close player has to be to trigger the behavior
    public GameObject positionToDeactivate; // Where should the chaser be deactivated

    public GameObject playerKart;

    // Use this for initialization
    void Start()
    {
        playerKart = FindObjectOfType<GameManager>().playerKart;
    }

    // Update is called once per frame
    void Update()
    {
        if((Vector3.Distance(transform.position, playerKart.transform.position) - distanceToActivate) <= 1)
        {
            chaserEnemy.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
