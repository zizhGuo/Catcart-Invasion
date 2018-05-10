using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartReference : MonoBehaviour
{
    public GameObject vrPlayer = null;
    public bool find = false;

    void Start()
    {
        print("Script start!");
    }

    void FindKart()
    {
        //print("Finding!");
            if (GameObject.Find("Client side") != null && GameObject.Find("Cube(Clone)") != null && !find)
            {
                GameObject obj = GameObject.Find("Cube(Clone)");
                obj.name = "VR_Player";
                vrPlayer = obj;
                find = true;
                //print("Found it");
            }
    }
    void Update()
    {
        if (vrPlayer == null) {
            FindKart();
        }
        //if (vrPlayer != null) print(vrPlayer.transform.position);
    }
}
