using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CustomControllerEvents : MonoBehaviour
{
    public Transform playArea;
    public GameObject catKart;
    public float switchMeleeDelay; // The time needed for the trigger to be pressed down to change the pistol to the melee weapon
                                   // If the trigger is being hold longer than this value, then the weapon will change to melee form, else it will shoot when the trigger is released
    public float swordEnergyPerSec; // How much energy the sword will comsume each second
    public float energyRegenPerSec; // How much energy will be regenerated per second if the player is not using the sword
    public float pistolEnergyConsump; // How much energy will be used for each pistol shot
    //public float extraEnergyPerSec; // How much extra energy will be gained when the player is shaking the weapon
    public float minimumStartMeleeReq; // The minimum energy required to start using melee
    public float lowEnergyPercent; // If the current energy is lower than this percentage of total energy, it is considered to be low energy
    public Transform otherController; // The other controller
    public float superChargeDistance; // How close the weapon has to be with the suepr charge location to start super charge
    public float superChargeEnergyPerSec; // How much more energy super charge will give each second

    //public static float currentEnergy; // The amount of energy the player currently has
    public float triggerDownTime; // The last time the trigger is pressed down (Time.time - triggerDownTime = how long the trigger is being pressed add the way down)
    public ShootPistol playerPistol; // The player's pistol
    public SwitchToMelee playerSword; // The script for switch to player's sword
    public bool isTriggerClickDown;
    public OpenCloseShield playerShield; // The script for switch to player's shield
    public GameManager gameManager;
    public bool isTriggerReleased; // If the trigger is released or not (THIS IS only used for the open and close sword conditions, not for general use!!!!!!!!
    public VRTK_ControllerActions controllerActions; // The controller actions (haptic pulse etc.)
    public Transform weaponSuperChargingLocation; // The location where the weapon can be super-charged
    public bool isSuperCharging; // If the weapon is super-charging. The weapon cannot be used when it is super-charging

    /// <summary>
    /// Bow
    /// </summary>
    public StartUsingBow bow; // The bow
    public float bowEnergyPerSec; // How fast the energy is transfered from the player's weapon to the bow per sec

    // Use this for initialization
    void Start()
    {
        //GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        //GetComponent<VRTK_ControllerEvents>().TriggerUnclicked += new ControllerInteractionEventHandler(DoTriggerUnclicked);
        //GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);

        gameManager = FindObjectOfType<GameManager>();
        bow = FindObjectOfType<StartUsingBow>();

        //playerPistol = FindObjectOfType<ShootPistol>();
        //playerSword = FindObjectOfType<SwitchToMelee>();

        isTriggerClickDown = false;
        isTriggerReleased = true;
        //currentEnergy = playerWeaponEnergy;
        controllerActions = GetComponentInChildren<VRTK_ControllerActions>();
        weaponSuperChargingLocation = GameManager.gameManager.catBasket.GetComponent<PlayerKartBasket>().superChargingLocation;
    }

    // Update is called once per frame
    void Update()
    {
        // Detect if the player's weapon is close to the super charging location
        if (transform.Find("PlayerWeapon") && 
            Vector3.Distance(transform.position, weaponSuperChargingLocation.position) <= superChargeDistance)
        {
            isSuperCharging = true;
            GameManager.canRechargeEnergy = true;
            print("enable energy recharge");

            // Deactivate the catpaw or shield if the weapon is super charging
            if (playerSword.swordOpened)
            {
                isTriggerReleased = true;
                playerSword.stopUsingMelee();
            }
        }
        else
        {
            isSuperCharging = false;
        }

        if (transform.Find("PlayerWeapon") || (!transform.Find("PlayerWeapon") && !otherController.Find("PlayerWeapon"))) // If the weapon is in this controller or is not in any of the two controllers
        {
            if (GameManager.currentEnergy <= lowEnergyPercent * GameManager.sPlayerTotalWeaponEnergy)
            {
                if (gameManager.playerUI.gameObject.activeInHierarchy)
                {
                    gameManager.playerUI.startLowEnergyAni();
                }
            }

            if (GameManager.currentEnergy > lowEnergyPercent * GameManager.sPlayerTotalWeaponEnergy)
            {
                if (gameManager.playerUI.gameObject.activeInHierarchy)
                {
                    gameManager.playerUI.stopLowEnergyAni();
                }
            }
        }

        if (otherController.Find("PlayerWeapon")) // If the weapon is in the other hand (controller)
        {
            return;
        }

        if (Time.time - triggerDownTime > switchMeleeDelay // If the player is holding the trigger
            && isTriggerClickDown // If the trigger is down
            && !isTriggerReleased // If the trigger is not release
            && !playerSword.swordOpened && playerSword.openSwordCoroutine == null // If the sword is not already opened
            && GameManager.currentEnergy >= minimumStartMeleeReq) // If the current energy is enough to start using sword
                                                                  //&& !bow.usingBow) // If the player is not using the bow
        {
            playerSword.startUsingMelee();
        }

        if (GameManager.currentEnergy <= 0 && playerSword.swordOpened && playerSword.closeSwordCoroutine == null) //If the player is using sword but the energy is empty
        {
            isTriggerReleased = true;
            playerSword.stopUsingMelee();
        }
    }

    void FixedUpdate()
    {
    //    if ((!transform.Find("PlayerWeapon") && !otherController.Find("PlayerWeapon")) && GetComponent<HandPickItems>().playerMelee) // If the weapon is not in any of the two controllers
    //    {
    //        //print("weapon not in any controller");
    //        if (GetComponent<HandPickItems>().playerMelee.activeInHierarchy)
    //        {
    //            //print("melee still active");
    //            isTriggerClickDown = false;
    //            isTriggerReleased = true;
    //            playerSword.stopUsingMelee();
    //            GetComponent<HandPickItems>().playerMelee.SetActive(false);
    //            GetComponent<HandPickItems>().playerShield.SetActive(false);
    //        }

    //        if (!playerSword.sword.activeInHierarchy // If the player is not using sword
    //        && GameManager.currentEnergy < GameManager.sPlayerTotalWeaponEnergy // If the energy is not full
    //        && GameManager.canRechargeEnergy) // If can recharge energy
    //        //&& !bow.aimingBow) //If the player is not aiming and prepare to fire the bow
    //        {
    //            GameManager.currentEnergy = Mathf.Clamp(GameManager.currentEnergy + energyRegenPerSec * Time.fixedUnscaledDeltaTime * 0.5f, 0, 1000); //Give energy
                
    //            // Add more energy if the weapon is super charging
    //            if (isSuperCharging)
    //            {
    //                GameManager.currentEnergy = Mathf.Clamp(GameManager.currentEnergy + superChargeEnergyPerSec * Time.fixedUnscaledDeltaTime * 0.5f, 0, 1000);
    //            }
    //        }
    //    }

    //    if (transform.Find("PlayerWeapon")) // If the weapon is in this controller
    //    {
    //        //print(transform.name + " has weapon");
    //        if (!playerSword.sword.activeInHierarchy // If the player is not using sword
    //        && GameManager.currentEnergy < GameManager.sPlayerTotalWeaponEnergy // If the energy is not full
    //        && GameManager.canRechargeEnergy) // If can recharge energy
    //        //&& !bow.aimingBow) //If the player is not aiming and prepare to fire the bow
    //        {
    //            GameManager.currentEnergy = Mathf.Clamp(GameManager.currentEnergy + energyRegenPerSec * Time.fixedUnscaledDeltaTime, 0, 1000); //Give energy

    //            // Add more energy if the weapon is super charging
    //            if(isSuperCharging)
    //            {
    //                GameManager.currentEnergy = Mathf.Clamp(GameManager.currentEnergy + superChargeEnergyPerSec * Time.fixedUnscaledDeltaTime, 0, 1000);
    //            }
    //        }
    //    }

    //    if (otherController.Find("PlayerWeapon")) // If the weapon is in the other hand (controller)
    //    {
    //        return;
    //    }

    //    if (playerSword.sword.activeInHierarchy) //If the player is using sword
    //    {
    //        GameManager.currentEnergy -= swordEnergyPerSec * Time.fixedUnscaledDeltaTime / Time.timeScale; //Consume energy
    //    }

    //    //Bow (not in game now)
    //    //if(bow.aimingBow)  //If the player is aiming and prepare to fire the bow
    //    //{
    //    //    GameManager.currentEnergy -= bowEnergyPerSec * Time.fixedUnscaledDeltaTime;
    //    //}
    }

    //public void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e) //This is for if the player want to adjust playArea's current facing direction to kart's forward direction, 
    //                                                                               //so the player don't need to turn the body to face the front of the kart
    //{
    //    Vector3 newEuler;

    //    newEuler.x = playArea.eulerAngles.x;
    //    newEuler.y = catKart.transform.eulerAngles.y;
    //    newEuler.z = playArea.eulerAngles.z;

    //    playArea.eulerAngles = newEuler;
    //}

    public void DoTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        if (otherController.Find("PlayerWeapon"))
        {
            return;
        }

        // Disable the weapon if it is super charging
        if (isSuperCharging)
        {
            return;
        }

        isTriggerClickDown = false;
        isTriggerReleased = true;

        //if (bow.usingBow) // If the pistol is currently mounted on the bow then he cannot use pistol or melee
        //{
        //    return;
        //}

        if (Time.time - triggerDownTime <= switchMeleeDelay && !playerShield.isShield &&
            GameManager.currentEnergy >= 100 && playerPistol.isActiveAndEnabled)
        {
            controllerActions.TriggerHapticPulse(1f, 0.1f, 0.2f);
            GameManager.currentEnergy -= pistolEnergyConsump;
            playerPistol.shoot();
        }

        if (Time.time - triggerDownTime > switchMeleeDelay && playerSword.swordOpened && playerSword.closeSwordCoroutine == null) //If the trigger is released and the sword is still opened (the sword is not closed because of no energy)
        {
            playerSword.stopUsingMelee();
        }
    }
    public void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        // Don't run if the weapon is in the other controller
        if (otherController.Find("PlayerWeapon"))
        {
            return;
        }

        // Disable the weapon if it is super charging
        if (isSuperCharging)
        {
            return;
        }

        isTriggerClickDown = true;
        isTriggerReleased = false;

        //if (bow.usingBow) // If the pistol is currently mounted on the bow then he cannot use pistol or melee
        //{
        //    return;
        //}

        triggerDownTime = Time.time;
    }

    /// <summary>
    /// Indication that the weapon is super charging
    /// </summary>
    public void SuperChargeIndication()
    {

    }
}
