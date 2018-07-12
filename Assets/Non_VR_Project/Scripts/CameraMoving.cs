using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour // Used for controlling camera to scroll around
{
    float xAxisValue;
    float zAxisValue;
    float yAxisValue;
    public float speedMultiple = 1f;

    //public GameObject GM;
    //public NonVRGameManager GMscript;

    public GameObject vrPlayer = null;
    public bool find = false;
    float counter;
    bool isFollowPlayer = false;

    Vector3 Pos;
    int panSpeed = 150;



    void Start()
    {
        //GMscript = GM.GetComponent<NonVRGameManager>();
        counter = Time.time;
    }
    void Update()
    {
        /*
        xAxisValue = Input.GetAxis("Horizontal");
        zAxisValue = Input.GetAxis("Vertical");
        gameObject.transform.position = new Vector3(transform.position.x + xAxisValue * speedMultiple, transform.position.y, transform.position.z + zAxisValue* speedMultiple);

    */
        Vector3 pos = transform.position;

            if (Input.GetKey(KeyCode.W))
            {
                pos.z += panSpeed * Time.deltaTime;
                isFollowPlayer = false;
            }
            if (Input.GetKey(KeyCode.S))
            {
                pos.z -= panSpeed * Time.deltaTime;
                isFollowPlayer = false;
            }
            if (Input.GetKey(KeyCode.A))
            {
                pos.x -= panSpeed * Time.deltaTime;
                isFollowPlayer = false;
            }
            if (Input.GetKey(KeyCode.D))
            {
                pos.x += panSpeed * Time.deltaTime;
                isFollowPlayer = false;
            }
            transform.position = pos;
        if (transform.position.y > 50 && transform.position.y < 100)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + panSpeed * 10 * Time.deltaTime, transform.position.z);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - panSpeed * 10 * Time.deltaTime, transform.position.z);
            }
        }
        else if (transform.position.y <= 50)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + panSpeed * 10 * Time.deltaTime, transform.position.z);
        }
        else {
            transform.position = new Vector3(transform.position.x, transform.position.y - panSpeed * 10 * Time.deltaTime, transform.position.z);
        }


        if (vrPlayer != null && Input.GetKey(KeyCode.Space))
        { 
            gameObject.transform.position = new Vector3(vrPlayer.transform.position.x, transform.position.y, vrPlayer.transform.position.z);
        }



        //if (isFollowPlayer && KRscprit.vrPlayer != null) gameObject.transform.position = new Vector3(KRscprit.vrPlayer.transform.position.x, KRscprit.vrPlayer.transform.position.y, transform.position.z);

        //print("Camera: GM.Kart's position: " + GMscript.Kart.transform.position);
        //print("Camera: GM.isFollowing?: " + GMscript.isFollowPlayer);

        if (vrPlayer == null)
        {
            FindKart();
        }

        //if (vrPlayer != null && GMscript.isFollowPlayer)
        //{
        //    print("Camera following!");
        //    print(GMscript.Kart.transform.position);
        //    gameObject.transform.position = new Vector3(GMscript.KartScript.vrPlayer.transform.position.x, transform.position.y, GMscript.KartScript.vrPlayer.transform.position.z);
        //}

        //if (Input.GetMouseButton(1) && !isFollowPlayer)
        //{            
        //    isFollowPlayer = true;
        //}
        //if (isFollowPlayer) gameObject.transform.position = new Vector3(vrPlayer.transform.position.x, transform.position.y, vrPlayer.transform.position.z);

        if (isFollowPlayer)
        {
            //gameObject.transform.position = new Vector3(vrPlayer.transform.position.x, transform.position.y, vrPlayer.transform.position.z);
        }
        //

        //print(GMscript.isFollowPlayer);
        //if (GMscript.Kart != null && GMscript.isFollowPlayer)
        //{
        //    print("Camera following!");
        //    print(GMscript.Kart.transform.position);
        //    gameObject.transform.position = new Vector3(GMscript.KartScript.vrPlayer.transform.position.x, transform.position.y, GMscript.KartScript.vrPlayer.transform.position.z);
        //}
    }
    void FindKart()
    {
        if (GameObject.Find("Client side") != null && GameObject.Find("Cube(Clone)") != null && !find)
        {
            GameObject obj = GameObject.Find("Cube(Clone)");
            //obj.name = "VR_Player";
            vrPlayer = obj;
            find = true;
        }
    }
}
