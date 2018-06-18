using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestColorChanged : NetworkBehaviour
{

	// Use this for initialization
	void Start () {
		
	}

    [Command]
    public void CmdDeactivated()
    {
        Debug.Log("CmdDeactivated on itself!");
        gameObject.SetActive(false);
    }
    public void Deactivated()
    {
        Debug.Log("Deactivated on itself!");
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void DoMove() {
        this.transform.Translate(1, 0, 0);
    }
}
