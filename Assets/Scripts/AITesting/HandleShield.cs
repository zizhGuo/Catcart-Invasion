using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleShield : MonoBehaviour {

    public GameObject shield;                   // Gameobject for the shield, which is the child of this transform
    public float flickerTime = 1;                    //Time in seconds when the shield toggle on and off

    private bool status;                        //current status of the shield, activated or deactivated
    private bool isWaiting;                     //to see if the shield is waiting to be activated or deactivated, or if the waiting is done

	// Use this for initialization
	void Start () {
        status = true;
        isWaiting = false;
        //StartCoroutine(activate(status));
	}
	
	// Update is called once per frame
	void Update () {

        if(isWaiting == false)
        {
            StartCoroutine(activate(status));
        }
	}

    public IEnumerator activate (bool stat)
    {
        isWaiting = true;
        yield return new WaitForSeconds(1);
        if(stat == false)
        {
            shield.SetActive(true);
            status = true;
        }
        else
        {
            shield.SetActive(false);
            status = false;
        }
        isWaiting = false;

    }
}
