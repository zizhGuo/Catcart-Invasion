using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DigitalRuby.ThunderAndLightning;
public class InteractablePropsTrigger : NetworkBehaviour
{
    [SerializeField]
    Camera mainCamera;
    Vector3 mousePos;
    bool _hit;
    RaycastHit hit;
    
    int layerMask;
    //[SerializeField] GameObject trapPrefab;

    public GameObject Lightning_Prefab;
    public ParticleSystem SelectEffectPrefab1;
    public GameObject SelectEffectPrefab2;
    [SerializeField] GameObject StartPointPrefab;
    [SerializeField] GameObject EndPointPrefab;
    [SerializeField] ParticleSystem SelectEffect1; // obselete
    [SerializeField] GameObject SelectEffect2;

    [SerializeField] float tempTimerDuration = 1f; // Lighgting Triggering Cool-Down Time
    [SerializeField] float tempTimeCurrent;
    [SerializeField] public Vector3 cameraPos_NVR;
    [SerializeField] public Vector3 spawnPosition; // Used as an argument to transfer the position of spawnable game obejcts
    [SerializeField] public bool isLightning_NVR;
    bool _lightStrikeLock = true;
    float _lightStrikeCurrentTime;

    [SyncVar] bool VR_lightning;
    [SyncVar] public Vector3 cameraPos_VR;
    [SyncVar] public Vector3 spawnPosition_VR;
    [SyncVar] bool isLightning_VR;
    [Command]
    void CmdSyncLightningPos(Vector3 a, Vector3 b, bool c) // Send the position of spawned Lightning strike to server
    {
        this.cameraPos_VR = a;
        this.spawnPosition_VR = b;
        this.isLightning_VR = c;
    }
    // Use this for initialization
    void Start () {
        if (FindObjectOfType<CameraMoving>()) {
            mainCamera = FindObjectOfType<CameraMoving>().gameObject.GetComponent<Camera>();
        }
        tempTimeCurrent = Time.time;

        SelectEffect1 = Instantiate(SelectEffectPrefab1) as ParticleSystem;
        SelectEffect1.gameObject.SetActive(false);

        SelectEffect2 = Instantiate(SelectEffectPrefab2) as GameObject;
        SelectEffect2.gameObject.SetActive(false);
        isLightning_NVR = false;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!isLocalPlayer && isServer) { // VR side

            if (isLightning_VR) // Sychronized the lightning strike in VR scene
            {
                var _startPoint = Instantiate(StartPointPrefab);
                _startPoint.transform.position = cameraPos_VR;

                var _endPoint = Instantiate(EndPointPrefab);
                _endPoint.transform.position = spawnPosition_VR;

                SpawnLightning(Lightning_Prefab, _startPoint, _endPoint, this.gameObject);
            }

            //Debug.Log("Camera Position from NVR: " + cameraPos_VR);
            //Debug.Log("Spawning Position from NVR: " + spawnPosition_VR);
        }
        if (isLocalPlayer && !isServer) // Non-VR side
        {
            mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            _hit = Physics.Raycast(ray, out hit, float.MaxValue);

            //Update the positions of camera or spawn position
            cameraPos_NVR = mainCamera.transform.position;                 // Camera Position
            spawnPosition = new Vector3(hit.point.x, 0f, hit.point.z);     // Spawn Point Position
            CmdSyncLightningPos(cameraPos_NVR, spawnPosition, isLightning_NVR);
            if (isLightning_NVR)
            { // Lighting Lock to extend striking time in VR scene

                if (_lightStrikeLock)
                {
                    _lightStrikeCurrentTime = Time.time;
                    _lightStrikeLock = false;
                }
                if (Time.time - _lightStrikeCurrentTime > 5 * Time.deltaTime)
                {
                    isLightning_NVR = false;
                    _lightStrikeLock = true;
                }
            }

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000, Color.red);
            if (_hit)
            {
                hit.transform.SendMessage("HitByRay");

                Debug.DrawLine(mainCamera.transform.position, hit.point, Color.yellow);
                if (hit.transform.name == "Sphere-Interactable(Clone)" && Input.GetKey(KeyCode.Mouse1) && Time.time - tempTimeCurrent > tempTimerDuration)
                {
                    tempTimeCurrent = Time.time;
                    var _startPoint = Instantiate(StartPointPrefab);
                    _startPoint.transform.position = mainCamera.transform.position;
                    var _endPoint = Instantiate(EndPointPrefab);
                    _endPoint.transform.position = spawnPosition;
                    SpawnLightning(Lightning_Prefab, _startPoint, _endPoint, this.gameObject);

                    Debug.Log("Hit the target!");
                    //Debug.Log("Right click!!");
                     //hit.transform.gameObject.GetComponent<TestColorChanged>().Deactivated();
                    hit.transform.gameObject.GetComponent<InteractablePropsController>().DoMove();
                    //hit.transform.gameObject.GetComponent<TestInteration>().CmdDoDebug();
                    //CmdDebugLogger(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<InteractablePropsController>().CmdDebugLogger(hit.transform.gameObject, this.gameObject);


                }

                if (hit.transform.name == "StreetLamp(Clone)" ){
                    //var streeLampScript = hit.transform.gameObject.GetComponent<InteractablePropsController>();
                    //streeLampScript.SelectEffect(hit.transform.gameObject, streeLampScript.selectEffect);
                    ActivateSelectEffect(SelectEffect2, hit.transform.gameObject);

                    if (Input.GetKey(KeyCode.Mouse1) && Time.time - tempTimeCurrent > tempTimerDuration && !isLightning_NVR) {

                        isLightning_NVR = true;
                        
                        Debug.Log("HIt the StreetLamp(Clone)!");
                        tempTimeCurrent = Time.time;
                        var _startPoint = Instantiate(StartPointPrefab);
                        _startPoint.transform.position = mainCamera.transform.GetChild(0).transform.position;
                        var _endPoint = Instantiate(EndPointPrefab);
                        _endPoint.transform.position = hit.transform.position;
                        SpawnLightning(Lightning_Prefab, _startPoint, _endPoint, this.gameObject);
                        StartCoroutine(WaitForLightningStrike(hit.transform.gameObject));
                        //hit.transform.gameObject.GetComponent<InteractablePropsController>().DoMove();
                        //hit.transform.gameObject.GetComponent<InteractablePropsController>().FallEffect();
                    }

                }

                else if (hit.transform.name == "Lamp_Hammer(Clone)")
                {
                    //var streeLampScript = hit.transform.gameObject.GetComponent<InteractablePropsController>();
                    //streeLampScript.SelectEffect(hit.transform.gameObject, streeLampScript.selectEffect);
                    ActivateSelectEffect(SelectEffect2, hit.transform.gameObject);

                    if (Input.GetKey(KeyCode.Mouse1) && Time.time - tempTimeCurrent > tempTimerDuration && !isLightning_NVR)
                    {

                        isLightning_NVR = true;
                        CmdSyncLightningPos(cameraPos_NVR, spawnPosition, isLightning_NVR);
                        //Debug.Log("HIt the Hammer!!!!");
                        tempTimeCurrent = Time.time;
                        var _startPoint = Instantiate(StartPointPrefab);
                        _startPoint.transform.position = mainCamera.transform.GetChild(0).transform.position;
                        var _endPoint = Instantiate(EndPointPrefab);
                        _endPoint.transform.position = hit.transform.GetChild(0).transform.position;
                        SpawnLightning(Lightning_Prefab, _startPoint, _endPoint, this.gameObject);
                        StartCoroutine(WaitForLightningStrike(hit.transform.gameObject));
                        //hit.transform.gameObject.GetComponent<InteractablePropsController>().DoMove();
                        //hit.transform.gameObject.GetComponent<InteractablePropsController>().FallEffect();
                    }

                }

                else {
                    //if (hit.transform.gameObject.GetComponent<InteractablePropsController>()) {
                    //    var streeLampScript = hit.transform.gameObject.GetComponent<InteractablePropsController>();
                    //    streeLampScript.DisableSelectEffect(streeLampScript.selectEffect);
                    //}
                    DisactivateSelectEffect(SelectEffect2);
                    if (Input.GetKey(KeyCode.Mouse1) && Time.time - tempTimeCurrent > tempTimerDuration)
                    {
                        tempTimeCurrent = Time.time;
                        var _startPoint = Instantiate(StartPointPrefab);
                        _startPoint.transform.position = mainCamera.transform.GetChild(0).transform.position;
                        var _endPoint = Instantiate(EndPointPrefab);
                        _endPoint.transform.position = spawnPosition;
                        SpawnLightning(Lightning_Prefab, _startPoint, _endPoint, this.gameObject);
                    }                        
                }
            }
            
        }

    }

    void SpawnLightning(GameObject Lightning_Prefab, GameObject _startPoint, GameObject _endPoint, GameObject player)
    {
        var _lightning = Instantiate<GameObject>(Lightning_Prefab);
        var controller = _lightning.GetComponent<LightningBoltPrefabScript>();
        controller.Source = _startPoint.gameObject;
        controller.Destination = _endPoint.gameObject;
        //NetworkServer.SpawnWithClientAuthority(_lightning, player);

    }

    void ActivateSelectEffect(GameObject se, GameObject go) {
        se.gameObject.SetActive(true);
        se.transform.position = go.transform.position;
        if (go.transform.name == "Lamp_Hammer(Clone)")
        {
            se.transform.position = go.transform.GetChild(0).transform.position;   // Put the select effect pos to the flag pos
        }
    }
    void DisactivateSelectEffect(GameObject se)
    {
        se.gameObject.SetActive(false);
    }

    private IEnumerator WaitForLightningStrike(GameObject ob)
    {
        yield return new WaitForSeconds(0.2f);
        if (ob.GetComponent<InteractablePropsController>() && ob.name == "StreetLamp(Clone)") {
            ob.GetComponent<InteractablePropsController>().FallEffect();
        }
        if (ob.GetComponent<InteractablePropsController>() && ob.name == "Lamp_Hammer(Clone)")
        {
            ob.GetComponent<InteractablePropsController>().TestIFGettested();
            ob.GetComponent<InteractablePropsController>().DetectHummerAround();
        }
    }

    private IEnumerator WaitForLightningTrigger()
    {
        yield return new WaitForSeconds(2f);
    }
}
