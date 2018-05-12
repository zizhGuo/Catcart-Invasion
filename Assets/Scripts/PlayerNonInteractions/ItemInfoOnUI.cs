using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoOnUI : MonoBehaviour
{
    /// <summary>
    /// This script contains some info of enemy that will be displayed on player's HUD UI
    /// It will also control the UI of this enemy's info that is actually showing on player's HUD UI
    /// </summary>

    public LayerMask playerHUDLayer; // The layerMask which let the UI raycast only collide with the player's HUD.
    public string itemName; // The name of the enemy to show on the HUD UI for enemy detial

    public GameObject playerHead;
    public GameObject playerEye; // The position of the player's eyes (it will be a bit in front of the head's position)
    public PlayerUI playerHUD; // The actual UI canvas on the HUD;
    public GameObject playerHUDdisplay; // The glass display game object on the HUD model;
    public GameObject infoOnHUD; // The UI game object which contains this enemy's info that is showed on the player's HUD UI
    public GameObject detailOnHUD; // The UI game object with enemy's detailed info if the enemy is within the HUD display area
    public GameObject arrowOnHUD; // The UI game object which is an arrow showing the enemy's relative position if the enemy is out of the HUD display area
    public RaycastHit hitInfo;

    /// <summary>
    /// Below is to calculate where the enemy arrow should be placed on the screen if the enemy is out of the player's HUD display region
    /// </summary>
    public Vector3 relativePositionToPlayer; // Enemy's "local" position relate to player's head
    public Vector3 vectorToCalculateAngle; // This vector will be the "local" position vector project onto the player head's local x-y plane
    public float relativeAngleToPlayerHeadUp; // This float will store the angle between vectorToCalculateAngle and player head's local up vector
    public Vector3 finalPositionOnHUD; // The calculated result of the local position for the arrow to be placed on the HUD

    // Use this for initialization
    void Start()
    {
        playerHead = FindObjectOfType<GameManager>().playerHead;
        playerEye = FindObjectOfType<PlayerEyeLocator>().gameObject;
        playerHUD = FindObjectOfType<GameManager>().playerUI;
        playerHUDdisplay = playerHUD.transform.parent.parent.gameObject;

        infoOnHUD = Instantiate(playerHUD.itemInfo, playerHUD.transform);
        detailOnHUD = infoOnHUD.GetComponent<EnemyInfoUIelements>().detail;
        //detailOnHUD.GetComponentInChildren<Text>().text = itemName;
        arrowOnHUD = infoOnHUD.GetComponent<EnemyInfoUIelements>().arrow;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.Normalize(playerHead.transform.position - transform.position), out hitInfo, Mathf.Infinity, playerHUDLayer)
            && playerHUDdisplay.transform.InverseTransformPoint(transform.position).z > 0) // If the enemy is within the HUD display area and is in front of the display (not behind the player)
        {
            {
                if (!detailOnHUD.activeInHierarchy) // If the enemy detail is not active
                {
                    infoOnHUD.transform.localRotation = Quaternion.identity;
                    detailOnHUD.SetActive(true);
                    arrowOnHUD.SetActive(false);
                }

                infoOnHUD.transform.LookAt(playerEye.transform);
                infoOnHUD.transform.position = hitInfo.point + (transform.position - playerHead.transform.position) * 1.5f;
            }
        }

        else // If the enemy is out of the HUD display area
        {
            //print(Vector3.Distance(transform.position, playerHUDdisplay.transform.position));
            relativePositionToPlayer = playerEye.transform.InverseTransformPoint(transform.position);
            relativePositionToPlayer.y += Vector3.Distance(transform.position, playerEye.transform.position) * 0.16f;
            //print(relativePositionToPlayer); //This is correct
            vectorToCalculateAngle = relativePositionToPlayer - playerEye.transform.InverseTransformDirection(playerEye.transform.forward) * relativePositionToPlayer.z;
            //print(vectorToCalculateAngle); //This is correct
            relativeAngleToPlayerHeadUp = Vector3.Angle(playerEye.transform.InverseTransformDirection(playerEye.transform.up), vectorToCalculateAngle) * Mathf.Sign(Vector3.Cross(playerEye.transform.InverseTransformDirection(playerEye.transform.up), vectorToCalculateAngle).z);
            //print(relativeAngleToPlayerHeadUp); //This is correct

            //Debug.DrawRay(playerEye.transform.position, (playerEye.transform.TransformPoint(relativePositionToPlayer) - playerEye.transform.position), Color.yellow);
            //Debug.DrawRay(playerEye.transform.position, playerEye.transform.TransformDirection(vectorToCalculateAngle), Color.green);


            if (relativeAngleToPlayerHeadUp >= -Mathf.Atan(487f / 342f) * Mathf.Rad2Deg && relativeAngleToPlayerHeadUp <= Mathf.Atan(487f / 342f) * Mathf.Rad2Deg) //If the angle is between the top left and right corner of the display
            {
                finalPositionOnHUD.y = 342;
                finalPositionOnHUD.x = 342f * -Mathf.Tan(relativeAngleToPlayerHeadUp * Mathf.Deg2Rad);
            }
            else if (relativeAngleToPlayerHeadUp <= -180f + Mathf.Atan(487f / 342f) * Mathf.Rad2Deg || relativeAngleToPlayerHeadUp >= 180f - Mathf.Atan(487f / 342f) * Mathf.Rad2Deg) //If the angle is between the bottom left and right corner of the display
            {
                finalPositionOnHUD.y = -342;
                finalPositionOnHUD.x = -342f * -Mathf.Tan(relativeAngleToPlayerHeadUp * Mathf.Deg2Rad);
            }
            else if (relativeAngleToPlayerHeadUp > Mathf.Atan(487f / 342f) * Mathf.Rad2Deg && relativeAngleToPlayerHeadUp < 180f - Mathf.Atan(487f / 342f) * Mathf.Rad2Deg) //If the angle is between the top left and bottom left corner of the display
            {
                finalPositionOnHUD.x = -487;
                finalPositionOnHUD.y = -487f * Mathf.Tan((relativeAngleToPlayerHeadUp + 90f) * Mathf.Deg2Rad);
            }
            else if (relativeAngleToPlayerHeadUp < -Mathf.Atan(487f / 342f) * Mathf.Rad2Deg && relativeAngleToPlayerHeadUp > -180f + Mathf.Atan(487f / 342f) * Mathf.Rad2Deg) //If the angle is between the top right and bottom right corner of the display
            {
                finalPositionOnHUD.x = 487;
                finalPositionOnHUD.y = 487f * Mathf.Tan((relativeAngleToPlayerHeadUp - 90f) * Mathf.Deg2Rad);
            }

            finalPositionOnHUD.y -= 184.5f;
            finalPositionOnHUD.z = 0;

            infoOnHUD.transform.localPosition = finalPositionOnHUD;
            infoOnHUD.transform.localEulerAngles = new Vector3(0, 0, relativeAngleToPlayerHeadUp);

            if (!arrowOnHUD.activeInHierarchy) // If the enemy arrow is not active
            {
                arrowOnHUD.SetActive(true);
                detailOnHUD.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        infoOnHUD.SetActive(true);
    }

    void OnDisable()
    {
        infoOnHUD.SetActive(false);
    }
}
