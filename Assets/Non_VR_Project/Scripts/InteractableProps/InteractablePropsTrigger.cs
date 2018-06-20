﻿using System.Collections;
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
    public Vector3 spawnPosition; // Used as an argument to transfer the position of spawnable game obejcts
    int layerMask;
    //[SerializeField] GameObject trapPrefab;

    public GameObject Lightning_Prefab;
    [SerializeField] GameObject StartPointPrefab;
    [SerializeField] GameObject EndPointPrefab;

    [SerializeField] float tempTimerDuration = 1f;
    [SerializeField] float tempTimeCurrent;


    // Use this for initialization
    void Start () {
        if (FindObjectOfType<CameraMoving>()) {
            mainCamera = FindObjectOfType<CameraMoving>().gameObject.GetComponent<Camera>();
        }
        tempTimeCurrent = Time.time;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isLocalPlayer && !isServer)
        {
            mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            _hit = Physics.Raycast(ray, out hit, float.MaxValue);
            spawnPosition = new Vector3(hit.point.x, 0f, hit.point.z);
            
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000, Color.red);
            if (_hit)
            {
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

                if (hit.transform.name == "StreetLamp(Clone)" && Input.GetKey(KeyCode.Mouse1) && Time.time - tempTimeCurrent > tempTimerDuration) {
                    Debug.Log("HIt the StreetLamp(Clone)!");
                    tempTimeCurrent = Time.time;
                    var _startPoint = Instantiate(StartPointPrefab);
                    _startPoint.transform.position = mainCamera.transform.GetChild(0).transform.position;
                    var _endPoint = Instantiate(EndPointPrefab);
                    _endPoint.transform.position = hit.transform.position;
                    SpawnLightning(Lightning_Prefab, _startPoint, _endPoint, this.gameObject);
                    //hit.transform.gameObject.GetComponent<InteractablePropsController>().DoMove();
                    hit.transform.gameObject.GetComponent<InteractablePropsController>().FallEffect();
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
}