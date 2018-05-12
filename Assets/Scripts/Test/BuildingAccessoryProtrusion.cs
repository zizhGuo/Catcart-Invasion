using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BuildingAccessoryProtrusion : MonoBehaviour {

    public Transform accessories;                   //the transform of the object containing windows and doors
    [HideInInspector]public Transform playerKart;                    //the transform of the player's cart
    public float distanceForActivation = 30;       //Distance specified by the designer, activates when the player is within this distance
    public float hidingDistance = 0.26f;            //how far inside the building the windows are going in
    public float speedMultiplier = 10;

    private float distanceFromKart;                 //current distance from the player's cart
    private int statusOfWindows;                    //status of the accessories. 0 dormant inside, 1 want to come out, 2 are being displayed outside, 3 want to go in
    private Rigidbody rb;                           //to store rigidbody of the object holding the accessories
    private Vector3 hidingPoint;                    //global position of the hiding point of the building accessories
    private Vector3 showPoint;                      //global position of where the windows should be when they are out
    private float distanceToMove;                   //distance towards the point where the windows should be moved
    private Vector3 directionToMove;                //to denote the distance where the windows should move
    private Scene currentScene;                

    // Use this for initialization
    void Start()
    {

        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ProgressVersion")
        {
            playerKart = FindObjectOfType<ObjectLocator>().objectToLocate.transform;
        }
        else
        {
            playerKart = GameManager.gameManager.playerKart.transform;
        }
        rb = accessories.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);

        //distanceForActivation = 200;

        showPoint = transform.TransformPoint(new Vector3(0, 0, 0));
        hidingPoint = transform.TransformPoint(new Vector3(0, 0, -hidingDistance));

        distanceFromKart = (playerKart.transform.position - transform.position).magnitude;
        if (distanceFromKart <= distanceForActivation)
        {
            statusOfWindows = 1;
            accessories.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            statusOfWindows = 0;
            accessories.transform.localPosition = new Vector3(0, 0, -hidingDistance);
        }
    }
	
	// Update is called once per frame
	void Update () {

        //if (distanceFromKart <= distanceForActivation)
        //{
        //    if(isPlayerClose == false)
        //    {
        //        isPlayerClose = true;
        //        accessories.gameObject.SetActive(true);
        //        StartCoroutine(accessories.GetComponents<LinearObjectMovement>()[0].Animate());
        //    }
        //}
        //else
        //{
        //    if(isPlayerClose == true)
        //    {
        //        isPlayerClose = false;
        //        StartCoroutine(accessories.GetComponents<LinearObjectMovement>()[1].Animate());
        //        accessories.gameObject.SetActive(false);
        //    }
        //}
    }

    private void FixedUpdate()
    {

        distanceFromKart = (playerKart.transform.position - transform.position).magnitude;

        switch (statusOfWindows)
        {
            case 0:
                if (distanceFromKart <= distanceForActivation)
                {
                    statusOfWindows = 1;
                }
                break;

            case 1:
                directionToMove = showPoint - accessories.transform.position;
                distanceToMove = directionToMove.magnitude;
                rb.velocity = directionToMove.normalized * distanceToMove * speedMultiplier;

                if (distanceToMove <= 0.01)
                {
                    rb.velocity = Vector3.zero;
                    statusOfWindows = 2;
                }

                if (distanceFromKart > distanceForActivation)
                {
                    statusOfWindows = 3;
                }

                break;

            case 2:
                if (distanceFromKart > distanceForActivation)
                {
                    statusOfWindows = 3;
                }
                break;

            case 3:
                directionToMove = hidingPoint - accessories.transform.position;
                distanceToMove = directionToMove.magnitude;
                rb.velocity = directionToMove.normalized * distanceToMove * speedMultiplier;

                if(distanceToMove < 0.01)
                {
                    rb.velocity = Vector3.zero;
                    statusOfWindows = 0;
                }

                if(distanceFromKart <= distanceForActivation)
                {
                    statusOfWindows = 1;
                }

                break;

            default:break;
        }

    }
}
