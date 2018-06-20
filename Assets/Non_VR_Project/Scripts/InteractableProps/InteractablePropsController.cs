using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DigitalRuby.ThunderAndLightning;

public class InteractablePropsController : NetworkBehaviour
{

    [SerializeField] public GameObject trapPrefab;
    [SerializeField] GameObject NVRPlayer;
    // Use this for initialization
    void Start () {
		
	}

    [Command]
    public void CmdDeactivated()
    {
        Debug.Log("CmdDeactivated on itself!");
        gameObject.SetActive(false);
    }
    [Command]
    public void CmdDoDebug()
    {
        Debug.Log("Command function called!");
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
    [Command]
    public void CmdDebugLogger(GameObject trigger, GameObject player)
    {
        Debug.Log("Command function on player CALLED!");
        GameObject trap = Instantiate(trapPrefab, trigger.transform.position, trigger.transform.rotation);
        NetworkServer.SpawnWithClientAuthority(trap, player);
    }
    public void FallEffect() {
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(transform.forward * 1000f);
    }
}
