using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

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
        mainCamera = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        _hit = Physics.Raycast(ray, out hit, float.MaxValue);

        Debug.DrawLine(ray.origin, ray.origin+ray.direction*1000, Color.red);
        if (_hit)
        {
            Debug.DrawLine(transform.position, hit.point, Color.yellow);
        }

        spawnPosition = new Vector3(hit.point.x, 0f, hit.point.z); //Converting the coordiante from Hit to spawning position in acual game scene
    }
}
