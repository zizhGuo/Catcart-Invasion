using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatsLostBehaviour : MonoBehaviour {

    bool ifHit = false;
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().transform.gameObject.tag == "Player" && !ifHit)
        {
            Debug.Log("Hit by the hummer!");
            ifHit = true;
            int i = 0;
            GameObject[] cats = GameObject.FindGameObjectsWithTag("Cat");
            foreach (GameObject cat in cats)
            {
                if (cat.name.Contains("PlayerCats"))
                {
                    Debug.Log("Cat's name: " + cat.name + " " + i);
                    cat.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}
