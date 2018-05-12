using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour {

    [HideInInspector] public float waitTime = 0.2f; //the delay for spawning the particle effect

    public GameObject particleEffect;               //the prefab of the particle effect


	// Use this for initialization
	void Start () {

        StartCoroutine(ParticleSpawner());

	}
	
    IEnumerator ParticleSpawner()
    {
        yield return new WaitForSeconds(waitTime);
        Instantiate(particleEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
