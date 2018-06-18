using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TestClickerOnPlayer : NetworkBehaviour
{
    [SerializeField]
    Camera mainCamera;
    Vector3 mousePos;
    bool _hit;
    RaycastHit hit;
    public Vector3 spawnPosition; // Used as an argument to transfer the position of spawnable game obejcts
    int layerMask;

    // Use this for initialization
    void Start () {
        if (FindObjectOfType<CameraMoving>()) {
            mainCamera = FindObjectOfType<CameraMoving>().gameObject.GetComponent<Camera>();
        }
        

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isLocalPlayer && !isServer)
        {
            if (Input.GetKey(KeyCode.Mouse1)) Debug.Log("Right click on clicker!");
            mousePos = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            _hit = Physics.Raycast(ray, out hit, float.MaxValue);

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000, Color.red);
            if (_hit)
            {
                Debug.DrawLine(mainCamera.transform.position, hit.point, Color.yellow);
                if (hit.transform.name == "Sphere-Interactable(Clone)" && Input.GetKey(KeyCode.Mouse1))
                {
                    Debug.Log("Hit the target!");
                    //hit.transform.gameObject.SetActive(false);
                    //Debug.Log("Right click!!");
                    //hit.transform.gameObject.GetComponent<TestColorChanged>().CmdChangeColor();
                    //hit.transform.gameObject.GetComponent<TestColorChanged>().CmdDeactivated();
                    //CmdDisableTheGameObject(hit.transform.gameObject);
                    //hit.transform.gameObject.GetComponent<TestColorChanged>().Deactivated();
                    hit.transform.gameObject.GetComponent<TestColorChanged>().DoMove();
                }
            }
            spawnPosition = new Vector3(hit.point.x, 0f, hit.point.z);
        }

    }

    [Command]
    void CmdDisableTheGameObject(GameObject gb)
    {
        Debug.Log("Command function called!");
        gb.SetActive(false);
    }
}
