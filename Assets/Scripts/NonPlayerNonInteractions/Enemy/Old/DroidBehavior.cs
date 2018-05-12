using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroidBehavior : MonoBehaviour
{

    GameObject playerKart;
    private NavMeshAgent nav;

    public GameObject explosionParticle;
    public float lifespan = 6f;
    public Animator anim;

    private float speed;

    // Use this for initialization
    void Start()
    {
        playerKart = FindObjectOfType<GameManager>().playerKart;
        nav = GetComponent<NavMeshAgent>();
        speed = nav.speed;
        speed = speed * 2 / 3;
        anim = GetComponent<Animator>();
        anim.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {

        nav.SetDestination(playerKart.transform.position);

        Destroy(gameObject, lifespan);

    }

    private void OnDestroy()
    {
        DroidSpawner spawner = FindObjectOfType<DroidSpawner>();
        spawner.totalCountOfEnemies--;

        GameObject newExplosion = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation); //Create explosion particle effect.

        GameManager.score += 1;
    }
}
