using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaFollowKart : MonoBehaviour
{
    public Transform playerSeat;
    public DetectIfPlayerIsInKart playerDetector; // The detector to detect if the player is in the kart
    public GameObject throwlerDummy; // Whenever the kart hit on an obstacle, the kart will create a dummy which will simulate it's movement for a while without colliding into anything
                                     // The play area will be attached to this dummy so that the kart will remains by the obstacle while the play area is "thrown out" of the kart
    public bool automaticTurn; // Will the play area turn with the kart?

    public Vector3 newEuler;
    public bool doFollow; // Will the play area follows the kart or not
    public Vector3 playerOffset; // The offset of the player from the center of the play area
    public float areaToKartDist; // The distance from the center of the play area to the player seat when the player first get in the kart
    public bool areaToKartDistSet; // If the distance from the center of the play area to the player seat when the player first get in the kart has been set for this entering the kart
    public bool isThrown;
    public GameObject newThrowlerDummy;
    public float lastTimeHitObstacle; // The most recent time when the player kart hit an obstacle
    public float playerAngleOffset; // The offset of the angle between kart's forward direction and the direction from kart to the center of the play area
    public GameObject playerHead;

    // Use this for initialization
    void Start()
    {
        doFollow = false;
        isThrown = false;
        playerDetector = FindObjectOfType<DetectIfPlayerIsInKart>();
        playerHead = FindObjectOfType<SteamVR_Camera>().gameObject;
        areaToKartDistSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.time - lastTimeHitObstacle >= 2)
        //{
        //    playerDetector.gameObject.SetActive(true);
        //}
        //Debug.Log("aaaaa");

        //if (!newThrowlerDummy.Equals(null))
        //{
        //    //print("have dummy, its velocity is: " + newThrowlerDummy.GetComponent<Rigidbody>().velocity);

        //    if (newThrowlerDummy.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        //    {
        //        print("dummy stop");
        //        isThrown = false;
        //        Destroy(newThrowlerDummy);
        //    }
        //}

        //Debug.Log("bbbbb");

        if (isThrown)
        {
            //print(newThrowlerDummy.GetComponent<Rigidbody>().velocity.magnitude);
            transform.position = newThrowlerDummy.transform.position;
        }

        //Debug.Log("ccccc");

        if (doFollow)
        {
            //Debug.Log("follow cart");

            isThrown = false;

            if (!automaticTurn)
            {
                transform.position = playerSeat.position - playerOffset;
            }

            if (automaticTurn)
            {
                //playerOffset.x = playerHead.transform.localPosition.x;
                //playerOffset.z = playerHead.transform.localPosition.z;

                //print("forward: " + Vector3.Angle(Vector3.forward, new Vector3(-playerHead.transform.localPosition.normalized.x, 0, -playerHead.transform.localPosition.normalized.z)));
                //print("right: " + Vector3.Angle(Vector3.right, new Vector3(-playerHead.transform.localPosition.normalized.x, 0, -playerHead.transform.localPosition.normalized.z)));
                if (!areaToKartDistSet) // If the player just stepped on the cart, setup the angle offset between the playarea and the player seat (in the catcart)
                {
                    playerAngleOffset = Vector3.Angle(Vector3.forward, new Vector3(-playerHead.transform.localPosition.normalized.x, 0, -playerHead.transform.localPosition.normalized.z));
                    if (Vector3.Angle(Vector3.right, new Vector3(-playerHead.transform.localPosition.normalized.x, 0, -playerHead.transform.localPosition.normalized.z)) >= 90) // On the left side
                    {
                        playerAngleOffset = 360 - playerAngleOffset;
                    }
                }

                transform.rotation = playerSeat.rotation;
                //transform.position = kart.TransformPoint(new Vector3(playerOffset.magnitude * Mathf.Sin(playerAngleOffset), 0, playerOffset.magnitude * Mathf.Cos(playerAngleOffset)));
                //transform.position = playerSeat.TransformPoint((new Vector3(playerOffset.magnitude * Mathf.Sin(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.x, 0, 
                //                                                            playerOffset.magnitude * Mathf.Cos(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.z)));

                if (!areaToKartDistSet) // If the player just stepped on the cart, setup the position offset between the playarea and the player seat (in the catcart)
                {
                    transform.position = playerSeat.TransformPoint((new Vector3(playerOffset.magnitude * Mathf.Sin(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.x, 0,
                                                                                playerOffset.magnitude * Mathf.Cos(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.z)));
                    areaToKartDist = Vector3.Distance(new Vector3(playerSeat.position.x, 0, playerSeat.position.z), new Vector3(transform.position.x, 0, transform.position.z)); // The distance between the center of the playarea and the kart seat
                    areaToKartDistSet = true;
                }

                transform.position = playerSeat.TransformPoint((new Vector3(areaToKartDist * Mathf.Sin(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.x, 0, 
                                                                            areaToKartDist * Mathf.Cos(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.z)));
                
                //print("position: " + playerSeat.TransformPoint((new Vector3(playerOffset.magnitude * Mathf.Sin(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.x, 0, playerOffset.magnitude * Mathf.Cos(playerAngleOffset * Mathf.Deg2Rad) / playerSeat.transform.lossyScale.z))));
                //print("player position" + new Vector3(playerOffset.magnitude * Mathf.Cos(playerAngleOffset * Mathf.Deg2Rad), 0, playerOffset.magnitude * Mathf.Sin(playerAngleOffset * Mathf.Deg2Rad)));
                //print("player angle offset: " + playerAngleOffset);
                //print("player offset: " + playerOffset);
                //print("player local magnitude: " + playerOffset.magnitude);
            }
        }

        //Debug.Log("zzzzz");
    }

    public void hitObstacle()
    {
        isThrown = true;
        doFollow = false;
        lastTimeHitObstacle = Time.time;
        playerDetector.gameObject.SetActive(false);
        newThrowlerDummy = Instantiate(throwlerDummy, transform.position, transform.rotation);
        newThrowlerDummy.GetComponent<Rigidbody>().velocity = playerSeat.GetComponent<Rigidbody>().velocity;
        //print(newThrowlerDummy.GetComponent<Rigidbody>().velocity);
    }
}
