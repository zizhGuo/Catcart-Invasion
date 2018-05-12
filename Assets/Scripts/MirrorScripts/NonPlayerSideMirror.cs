using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class NonPlayerSideMirror : MonoBehaviour
{
    /// <summary>
    /// Basically, all the actions happened in the game will follows a "hidden" kart which can only turn but not move. 
    /// This script is attached to the game objects that will be mirrored from the "hidden" kart system, where the transform of the objects
    /// that the player actually see will be mirrored from this hidden system.
    /// </summary>

    public Vector3 weaponDashboardLocalPosition; // The local position to teleport the weapon back on the dashboard
    public Vector3 weaponDashboardLocalEulerAngles; // The local euler angles to teleport the weapon back on the dashboard
    public Vector3 laserPointerDashboardLocalPosition; // The local position to teleport the laser pointer back on the dashboard

    public bool doMirror; // Should this object currently mirroring the player side transform?
    public GameObject playerKart; // The player side kart
    public GameObject nonPlayerKart; // The non-player side kart
    public GameObject playerSideCopy; // Its copy on the player side
    public bool wasGravityEnabled; // Was gravity enabled before the item is grabbed?
    public HeadWearHUD playerHeadTrigger; // Trigger attached to player's head

    // Use this for initialization
    void Start()
    {
        if (FindObjectOfType<MirrorGameManager>())
        {
            playerKart = FindObjectOfType<MirrorGameManager>().playerKart;
            nonPlayerKart = FindObjectOfType<MirrorGameManager>().nonPlayerKart;
        }

        else
        {
            playerKart = FindObjectOfType<GameManager>().playerKart;
            nonPlayerKart = FindObjectOfType<GameManager>().nonPlayerKart;
        }

        playerHeadTrigger = FindObjectOfType<HeadWearHUD>();
        wasGravityEnabled = playerSideCopy.GetComponent<Rigidbody>().useGravity;
    }

    // Update is called once per frame
    void Update()
    {

        //print("non player mirror");
        //if(transform.name == "NonPlayerMirror_LaserPointer" && !doMirror)
        //{
        //    print("!domirror");
        //}

        if (playerSideCopy != null && playerSideCopy.GetComponent<VRTK_InteractableObject>())
        {
            //print(playerSideCopy.GetComponent<VRTK_InteractableObject>().IsGrabbed() + ", " + playerSideCopy.GetComponent<PlayerSideMirror>().inKart);
            // If the player side copy is outside of kart or being grabbed, then transfer the physics simulation to player's side
            if (!playerSideCopy.activeInHierarchy || playerSideCopy.GetComponent<VRTK_InteractableObject>().IsGrabbed() || !playerSideCopy.GetComponent<PlayerSideMirror>().inKart)
            {
                if (!doMirror)
                {
                    playerSideCopy.GetComponent<Rigidbody>().isKinematic = false;
                }

                doMirror = true;
                GetComponent<Rigidbody>().useGravity = false;
                wasGravityEnabled = playerSideCopy.GetComponent<Rigidbody>().useGravity;
                playerSideCopy.GetComponent<PlayerSideMirror>().doMirror = false;
            }


            //else if (!playerSideCopy.GetComponent<VRTK_InteractableObject>().IsGrabbed())
            //{
            //    doMirror = false;
            //    GetComponent<Rigidbody>().velocity = Vector3.zero;
            //    playerSideCopy.GetComponent<PlayerSideMirror>().doMirror = true;
            //}

            // Turn off non-player side mirror (and turn on the physics simulation on non-player side) when the player is not grabbing this object and the object is in the cart
            if (!playerSideCopy.GetComponent<VRTK_InteractableObject>().IsGrabbed() && 
                playerSideCopy.GetComponent<PlayerSideMirror>().inKart)
            {
                if (transform.name == "NonPlayerMirror_LaserPointer")
                {
#if UNITY_EDITOR
                    //UnityEditor.EditorApplication.isPaused = true;
#endif
                    //print("non player laser ungrab cart");
                }

                bool isCatAndJumping = false;

                // If the object is a cat and the cat is jumping then don't start transfer the physics simulation yet
                if (playerSideCopy.GetComponent<PlayerCatStayInBasket>())
                {
                    if (playerSideCopy.GetComponent<PlayerCatStayInBasket>().isJumping)
                    {
                        isCatAndJumping = true;
                    }
                }

                if (!isCatAndJumping)
                {
                    // If it is the player's HUD
                    if (playerSideCopy.tag == "HUDwhole")
                    {
                        // If the player is wearing the HUD
                        if (playerHeadTrigger.isWearingHUD)
                        {
                            if (!doMirror)
                            {
                                doMirror = true;
                                GetComponent<Rigidbody>().useGravity = false;
                                wasGravityEnabled = playerSideCopy.GetComponent<Rigidbody>().useGravity;
                                playerSideCopy.GetComponent<PlayerSideMirror>().doMirror = false;
                            }

                            return;
                        }
                    }

                    // Set up the non-player side physics simulation when it's being transfered from player side everytime
                    if (doMirror)
                    {
                        //GetComponent<Rigidbody>().velocity = playerSideCopy.GetComponent<PlayerSideMirror>().lastRelativeVelocity;
                        //GetComponent<Rigidbody>().AddForce(playerSideCopy.GetComponent<PlayerSideMirror>().lastRelativeVelocity, ForceMode.VelocityChange);
                        //print("non mirror" + GetComponent<Rigidbody>().velocity + playerSideCopy.GetComponent<PlayerSideMirror>().lastRelativeVelocity);

                        StartCoroutine(StartMirrorSync());

                        if (playerSideCopy.GetComponent<PlayerCatStayInBasket>())
                        {
                            GetComponent<Rigidbody>().velocity = Vector3.zero;
                        }
                        else
                        {
                            // If the player ungrab or dropped the weapon within the cart, then teleport the weapon to the cart's dashboard every time
                            if (playerSideCopy.name == "PlayerWeapon")
                            {
                                GetComponent<Rigidbody>().velocity = Vector3.zero;
                                playerSideCopy.transform.position = playerKart.transform.TransformPoint(weaponDashboardLocalPosition);
                                //playerSideCopy.transform.eulerAngles = playerKart.transform.TransformDirection(weaponDashboardLocalEulerAngles);
                                transform.position = nonPlayerKart.transform.TransformPoint(weaponDashboardLocalPosition);
                                //transform.eulerAngles = nonPlayerKart.transform.TransformDirection(weaponDashboardLocalEulerAngles);
                            }
                            // If the player ungrab or dropped the weapon within the cart, then teleport the weapon to the cart's dashboard every time
                            else if (playerSideCopy.name == "LaserPointer")
                            {
                                GetComponent<Rigidbody>().velocity = Vector3.zero;
                                playerSideCopy.transform.position = playerKart.transform.TransformPoint(laserPointerDashboardLocalPosition);
                                transform.position = nonPlayerKart.transform.TransformPoint(laserPointerDashboardLocalPosition);
                            }
                            else
                            {
                                GetComponent<Rigidbody>().velocity = Vector3.zero;
                                //<Rigidbody>().AddForce(playerSideCopy.GetComponent<PlayerSideMirror>().relativeVelocity, ForceMode.VelocityChange);
                            }
                        }

                        playerSideCopy.GetComponent<Rigidbody>().isKinematic = true;
                        playerSideCopy.GetComponent<Rigidbody>().useGravity = wasGravityEnabled;

                        foreach (Collider c in GetComponents<Collider>())
                        {
                            c.enabled = true;
                        }

                        //StartCoroutine(StartMirrorSync());
                    }

                    doMirror = false;
                    GetComponent<Rigidbody>().useGravity = true;
                    playerSideCopy.GetComponent<PlayerSideMirror>().doMirror = true;
                }
            }
        }
        
        // Mirroring player's side physics
        if (playerSideCopy != null && doMirror)
        {
            transform.position = nonPlayerKart.transform.TransformPoint(playerKart.transform.InverseTransformPoint(playerSideCopy.transform.position)); // Mirror the player side position
            transform.rotation = playerSideCopy.transform.rotation; // Mirror the player side rotation
            GetComponent<Rigidbody>().AddForce(GameManager.kartDeltaAcceleration * 5, ForceMode.Acceleration); // Apply the change of acceleration of player's kart as inertia to the non-player side rigidbody
        }
    }

    /// <summary>
    /// Runs everytime when an object is ungrabbed inside the cart or come inside the cart ungrabbed
    /// to sync player side copy with the non-player side copy
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartMirrorSync()
    {
        //playerSideCopy.GetComponent<Rigidbody>().useGravity = false;
        doMirror = false;
        playerSideCopy.GetComponent<PlayerSideMirror>().doMirror = true;
        transform.position = nonPlayerKart.transform.TransformPoint(
                                playerKart.transform.InverseTransformPoint(playerSideCopy.transform.position));
        transform.rotation = playerSideCopy.transform.rotation;

        float oldMass = playerSideCopy.GetComponent<Rigidbody>().mass;
        //GetComponent<Rigidbody>().mass = 0;
        //playerSideCopy.GetComponent<Rigidbody>().mass = 0;
        playerSideCopy.GetComponent<PlayerSideMirror>().inKart = true;

        yield return new WaitForEndOfFrame();

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        playerSideCopy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerSideCopy.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = nonPlayerKart.transform.TransformPoint(
                                playerKart.transform.InverseTransformPoint(playerSideCopy.transform.position));
        transform.rotation = playerSideCopy.transform.rotation;
        playerSideCopy.GetComponent<PlayerSideMirror>().inKart = true;

        yield return new WaitForEndOfFrame();

        //GetComponent<Rigidbody>().mass = oldMass;
        //playerSideCopy.GetComponent<Rigidbody>().mass = oldMass;
        transform.position = nonPlayerKart.transform.TransformPoint(
                                playerKart.transform.InverseTransformPoint(playerSideCopy.transform.position));
        transform.rotation = playerSideCopy.transform.rotation;
        playerSideCopy.GetComponent<PlayerSideMirror>().inKart = true;

        //yield return new WaitForEndOfFrame();
        //playerSideCopy.GetComponent<Rigidbody>().useGravity = true;
    }

    void FixedUpdate()
    {

    }
}
