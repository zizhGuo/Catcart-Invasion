using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonVRGameManager : MonoBehaviour
{
    // Kart Reference
    public GameObject KartTracer;
    public KartReference KartScript;
    public GameObject Kart;
    bool find = false;

    // Camera Control
    public bool isFollowPlayer;

    // Timer for lock.
    float counter;

    void Start()
    {
        KartScript = KartTracer.GetComponent<KartReference>();
        isFollowPlayer = false;
        counter = Time.time;
    }

    void Update()
    {
        //  Assign Kart refence as Kart object if it is found.
        if (KartScript.find && !find) {
            Kart = KartScript.vrPlayer;
            find = true;
            print("Game Manager： Kart find (from GameManagerScript)");
            print("Game Manager: isFollowPlayer: " + isFollowPlayer);
        }
        if (Time.time - counter > 1 && Input.GetKey(KeyCode.T))
        {
            isFollowPlayer = !isFollowPlayer;
            print("Game Manager: Switch the Camera Following item! " + isFollowPlayer);
            counter = Time.time;
        }
        //print("Kart's position: " + Kart.transform.position);



    }
}
