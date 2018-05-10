using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyRenederer : NetworkBehaviour {

    public Renderer rend;
	// Use this for initialization
	void Start () {
        rend =  GetComponent<Renderer>();
        if (isServer) rend.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
