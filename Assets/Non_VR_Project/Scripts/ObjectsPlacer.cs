using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectsPlacer : MonoBehaviour
{
    Camera mainCamera;
    Vector3 mousePos;
    bool _hit;
    RaycastHit hit;
    public Vector3 spawnPosition; // Used as an argument to transfer the position of spawnable game obejcts
    int layerMask;

    public int chosenNum;

    void Start()
    {
        
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        _hit = Physics.Raycast(ray, out hit, float.MaxValue);

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000, Color.red);
        if (_hit)
        {
            Debug.DrawLine(transform.position, hit.point, Color.yellow);
            if (hit.transform.name == "Sphere-Interactable" && Input.GetKey(KeyCode.Mouse1))
            {
                //hit.transform.gameObject.SetActive(false);
                Debug.Log("Right click!!");
                //hit.transform.gameObject.GetComponent<TestColorChanged>().CmdChangeColor();
            }
        }

        spawnPosition = new Vector3(hit.point.x, 0f, hit.point.z); //Converting the coordiante from Hit to spawning position in acual game scene
    }
}
