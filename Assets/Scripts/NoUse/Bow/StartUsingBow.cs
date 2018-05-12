using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUsingBow : MonoBehaviour
{
    public GameObject ballLightning; // The "arrow" of the bow, which creates a ball lightning and it will move forward
    public float arrowSizeToEnergySizeRatio; // How much the arrow size is going to increase with the given amount of energy (size *= energy * ratio)
    public float arrowSpeed; // How fast the arrow is travelling

    public GameObject bowFrame; // The frame of the bow
    public bool usingBow; // If the player is using the bow
    public bool aimingBow; // If the player is aiming the bow
    public Transform playerPistolPos; // The player's pistol's transform, it will be assigned when the player first use the bow
    public GameObject currentArrow; // The "arrow" that is about to be fired (which is already been placed on the bow)
    public CustomControllerEvents playerPistol; // The player's pistol, it will be assigned when the player first use the bow
    public float totalEnergyTransfered; // How much is the total energy transfered from the player's weapon when the player release the arrow

    // Use this for initialization
    void Start()
    {
        arrowSpeed *= GameManager.sSpeedMultiplier;

        bowFrame = transform.parent.parent.Find("BowFrameWrap").gameObject;
        usingBow = false;
        aimingBow = false;
        totalEnergyTransfered = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (aimingBow)
        {
            bowFrame.transform.LookAt(playerPistolPos);
            bowFrame.transform.localEulerAngles = new Vector3(0, bowFrame.transform.localEulerAngles.y, 0);


            if(!playerPistol.isTriggerClickDown || GameManager.currentEnergy <= 0) // If the player release the trigger or transfered all its current energy
            {
                shootArrow();
            }
        }
    }

    void FixedUpdate()
    {
        if (aimingBow) // If the player is aiming and prepare to fire the bow
        {
            totalEnergyTransfered += playerPistol.bowEnergyPerSec * Time.fixedUnscaledDeltaTime;
            currentArrow.transform.localScale = ballLightning.transform.localScale * (totalEnergyTransfered * arrowSizeToEnergySizeRatio + 1); // Increase the arrow size as the energy is transfered from player's weapon
            currentArrow.transform.localPosition = new Vector3(0, currentArrow.transform.localPosition.y, -0.1f - currentArrow.transform.localScale.z / 2f); // Move the arrow away from bow as its size increasing

            //Also need to adjust the arrow height according to player weapon's y coord.
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "RayGun") // If player's pistol is "mounted" on the bow
        {
            playerPistolPos = other.transform;
            playerPistol = other.transform.parent.Find("RightController").GetComponent<CustomControllerEvents>();

            if (!playerPistol.playerSword.sword.activeInHierarchy) // If the player is not currently using melee
            {
                usingBow = true; // If the player's pistol is "mounted" on the bow, then he won't be able to shoot pistol or switch to melee or shield
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.name == "RayGun")
        {
            if (playerPistol.isTriggerClickDown && usingBow) // If the player press down the trigger while using the bow
            {
                startBow();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.name == "RayGun")
        {
            if (!playerPistol.isTriggerClickDown) // If the player is not press the trigger when the pistol leave the bow
            {
                usingBow = false;
            }
        }
    }

    public void startBow()
    {
        if (!aimingBow) // If the player is not already prepare to shoot the arrow
        {
            aimingBow = true;
            currentArrow = Instantiate(ballLightning, bowFrame.transform);
            currentArrow.transform.localPosition = new Vector3(0, 0.174f, -0.1f);
        }
    }

    public void shootArrow()
    {
        currentArrow.transform.parent = null;
        currentArrow.GetComponent<BallLightningBehavior>().enabled = true;
        currentArrow.GetComponent<BallLightningBehavior>().energy = totalEnergyTransfered;
        currentArrow.GetComponent<Rigidbody>().isKinematic = false;
        currentArrow.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.position - playerPistolPos.position) * arrowSpeed, ForceMode.Impulse);
        currentArrow = null;
        totalEnergyTransfered = 0;

        aimingBow = false;
        usingBow = false;
    }
}
