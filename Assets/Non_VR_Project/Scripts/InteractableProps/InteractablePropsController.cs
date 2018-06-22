using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DigitalRuby.ThunderAndLightning;

public class InteractablePropsController : NetworkBehaviour
{

    [SerializeField] public GameObject trapPrefab;
    [SerializeField] GameObject NVRPlayer;
    [SerializeField] public ParticleSystem selectEffect;
    // Use this for initialization
    void Start () {
        //selectEffect = Instantiate(selectEffect) as ParticleSystem;
        //selectEffect.gameObject.SetActive(false);
    }
    //void HitByRay() {
    //    selectEffect.gameObject.SetActive(true);
    //    selectEffect.transform.position = this.gameObject.transform.position;
    //}

    void Update()
    {
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

    public GameObject GetPropScript() {
        return this.gameObject;
    }
    public void FallEffect() {
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        //rb.AddForce(transform.right*-1 * 1000f);
        //rb.AddForce(transform.forward * -1 * 1000f);
        //rb.AddForce(transform.up * 1000f);
        rb.AddForceAtPosition(transform.right * -1 * 1000f, new Vector3(rb.transform.position.x, rb.transform.position.y + 10, rb.transform.position.z));
    }
            
}
