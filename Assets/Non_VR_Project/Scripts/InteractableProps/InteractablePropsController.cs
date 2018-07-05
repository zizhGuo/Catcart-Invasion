using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DigitalRuby.ThunderAndLightning;
using DG.Tweening;
public class InteractablePropsController : NetworkBehaviour
{

    [SerializeField] public GameObject trapPrefab;
    [SerializeField] GameObject NVRPlayer;
    [SerializeField] public ParticleSystem selectEffect;
    [SerializeField] private GameObject car;
    [SerializeField] private bool carLock = true;

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
        //Debug.Log(car);
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
    public void TestIFGettested() {
        Debug.Log("We detect the traffic lights script!");
        if (car != null) {
            Debug.Log("Cao le!");
        }
    }
    public void SetOffCar() {
        //if 
    }

    public void TriggerCar() {

    }
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Collider>().transform.gameObject.tag == "InteractableCar" && carLock) {
            Debug.Log("Detected the HUMMER from the traffic lights!");
            car = other.GetComponent<Collider>().transform.gameObject;
            carLock = false;
        }
    }

    public void DetectHummerAround()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 100, Quaternion.identity);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            //Debug.Log("Hit : " + hitColliders[i].name + i);
            //Increase the number of Colliders in the array
            i++;
            if (hitColliders[i].name == "hummer(Clone)") {
                Debug.Log("Hit : " + hitColliders[i].name + i);
                Debug.Log("Found the HUMMER!");
                hitColliders[i].transform.gameObject.GetComponent<InteractablePropsController>().GoFoward();
                break;
            }
        }
    }
    public void GoFoward() {
        // this.transform.Translate(-Vector3.right);
        var pos = this.transform.position;
        this.transform.DOMove(new Vector3(pos.x+(-Vector3.forward.x)*25, pos.y + (-Vector3.forward.y)*25, pos.z + (-Vector3.forward.z)*25), 1f).SetEase(Ease.InOutQuad);
        Debug.Log("GoForward function called!");
    }

}
