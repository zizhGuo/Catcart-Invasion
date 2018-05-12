using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// The script for all the player weapon events.
/// </summary>
public class PlayerCatilizerControl : MonoBehaviour
{
    public float switchMeleeDelay; // The time needed for the trigger to be pressed down to change the pistol to the melee weapon
                                   // If the trigger is being hold longer than this value, then the weapon will change to melee form, else it will shoot when the trigger is released
    public float swordEnergyPerSec; // How much energy the sword will comsume each second
    public float energyRegenPerSec; // How much energy will be regenerated per second if the player is not using the sword
    public float pistolEnergyConsump; // How much energy will be used for each pistol shotpublic float minimumStartMeleeReq; // The minimum energy required to start using melee
    public float minimumStartMeleeReq; // The minimum energy required to start using melee
    public float lowEnergyPercent; // If the current energy is lower than this percentage of total energy, it is considered to be low energy
    public PlayerCatilizerControl otherController; // The other controller
    public Vector3 superChargeArea; // The rectangular area below a certain height relative to the cat basket that allows the player to super charge
    public float superChargeEnergyPerSec; // How much more energy super charge will give each second
    public GameObject weaponModel; // The model of the weapon
    public bool gripMelee; // Do we use grip button for melee or not

    public bool hasWeapon; // Does this controller has the weapon
    public PlayerShieldRelatedReferences shieldRelatedReferences; // The references object that relates to shield functions
    public PlayerMeleeRelatedReferences meleeRelatedReferences; // The references object that relates to melee functions
    public PlayerPistolRelatedReferences pistolRelatedReferences; // The references object that relates to pistol functions
    public ShootPistol playerPistol; // The player's pistol
    public float triggerDownTime; // The last time the trigger is pressed down (Time.time - triggerDownTime = how long the trigger is being pressed add the way down)
    public bool isTriggerClickDown;
    //public bool isTriggerReleased; // If the trigger is released or not (THIS IS only used for the open and close sword conditions, not for general use!!!!!!!!
    public VRTK_ControllerActions controllerActions; // The controller actions (haptic pulse etc.)
    public bool isSuperCharging; // If the weapon is super-charging. The weapon cannot be used when it is super-charging
    public Coroutine weaponShieldAnimation; // The animation of the weapon transfer to shield or the reverse
    public Coroutine weaponMeleeAnimation; // The animation of the weapon transfer to melee or the reverse
    public int weaponForm; // Which form the weapon is now? (0: gun, 1: melee, 2: shield, 3: animation, 4: super-charging)
    public int nextForm; // What's the upcoming form after all the animation finished
    public bool isGripClickDown;
    public bool justPicked; // Did the player just picked up the weapon (prevent shoot pistol when the player release trigger when grab weapon)
    public PlayerUI playerUI; // The player's HUD UI
    public Transform catBasket; // The transform of the cat basket

    // Use this for initialization
    void Start()
    {
        hasWeapon = false;
        isTriggerClickDown = false;
        //isTriggerReleased = true;
        controllerActions = GetComponentInChildren<VRTK_ControllerActions>();
        weaponForm = 0;
        isGripClickDown = false;
        playerUI = FindObjectOfType<GameManager>().playerUI;

        // Set the switch melee delay for multiplayer or single player mode
        //if (GameManager.gameManager.allowMultiplayer)
        //{
        //    switchMeleeDelay = 0.2f;
        //}
        //else
        //{
        //    switchMeleeDelay = PlayerPrefs.GetFloat("TriggerClickTime");
        //}
    }

    //public IEnumerator LateStart()
    //{
    //    print("late start");
    //    yield return new WaitForSeconds(0.1f);
    //    print("end of frame");

    //    print("late start finish");
    //}

    // Update is called once per frame
    void Update()
    {
        if (pistolRelatedReferences == null)
        {
            shieldRelatedReferences = GetComponentInParent<PlayerShieldRelatedReferences>();
            meleeRelatedReferences = GetComponentInParent<PlayerMeleeRelatedReferences>();
            pistolRelatedReferences = GetComponentInParent<PlayerPistolRelatedReferences>();
            playerPistol = transform.parent.GetComponentInChildren<ShootPistol>();
        }

        if (!hasWeapon)
        {
            return;
        }

        CheckWeaponDistanceToSuperCharge();

        CheckIfAnimationFinished();

        //CheckWeaponRelativeAngle();
        if (nextForm == 2)
        {
            shieldRelatedReferences.shield.SetActive(true);
            weaponForm = nextForm;
            nextForm = 0;
        }

        CheckIfStartMelee();

        //CheckIfStopMeleeWhenEnergyEmpty();
    }

    /// <summary>
    /// Call when the grip is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DoGripClicked(object sender, ControllerInteractionEventArgs e)
    {
        isGripClickDown = true;
    }

    /// <summary>
    /// Call when the grip is unclicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DoGripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        isGripClickDown = false;

        if (gripMelee)
        {
            //If the grip is released and the sword is still opened (the sword is not closed because of no energy)
            if (weaponForm == 1 || (weaponForm == 3 && nextForm == 1))
            {
                ChangeToPistol();
            }
        }
    }

    /// <summary>
    /// Call when the trigger is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        // Don't run if the weapon is in the other controller
        if (otherController.hasWeapon)
        {
            return;
        }

        // Disable the weapon if it is super charging
        if (isSuperCharging)
        {
            return;
        }

        isTriggerClickDown = true;
        //isTriggerReleased = false;

        //if (bow.usingBow) // If the pistol is currently mounted on the bow then he cannot use pistol or melee
        //{
        //    return;
        //}

        triggerDownTime = Time.time;
    }

    /// <summary>
    /// Call when the trigger is unclicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DoTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        // Don't run if the weapon is in the other controller
        if (otherController.hasWeapon)
        {
            return;
        }

        // Disable the weapon if it is super charging
        if (isSuperCharging)
        {
            return;
        }

        // If the player just picked up the weapon
        if (justPicked)
        {
            justPicked = false;
            return;
        }

        isTriggerClickDown = false;
        //isTriggerReleased = true;

        if (gripMelee)
        {
            if (weaponForm == 0 && GameManager.currentEnergy >= 100)
            {
                controllerActions.TriggerHapticPulse(1f, 0.1f, 0.2f);
                GameManager.currentEnergy -= pistolEnergyConsump;
                //print(GameManager.currentEnergy);
                playerPistol.shoot();
            }
        }
        else
        {
            // If the trigger is clicked and the weapon is in the gun form and there is enough energy for one shot
            if (weaponForm == 0 &&
                GameManager.currentEnergy >= 100)
            {
                if (GameManager.gameManager.allowMultiplayer && Time.time - triggerDownTime <= 0.2f)
                {
                    controllerActions.TriggerHapticPulse(1f, 0.1f, 0.2f);
                    GameManager.currentEnergy -= pistolEnergyConsump;
                    playerPistol.shoot();
                }
                else if (!GameManager.gameManager.allowMultiplayer &&
                         //Time.time - triggerDownTime <= PlayerPrefs.GetFloat("TriggerClickTime"))
                         Time.time - triggerDownTime <= 0.2f) // For debug
                {
                    controllerActions.TriggerHapticPulse(1f, 0.1f, 0.2f);
                    GameManager.currentEnergy -= pistolEnergyConsump;
                    playerPistol.shoot();
                }
            }
        }

        //If the trigger is released and the sword is still opened (the sword is not closed because of no energy)
        if (!gripMelee &&
            (weaponForm == 1 || (weaponForm == 3 && nextForm == 1)))
        {
            if (GameManager.gameManager.allowMultiplayer && Time.time - triggerDownTime > 0.2f)
            {
                ChangeToPistol();
            }
            else if (!GameManager.gameManager.allowMultiplayer &&
                     //Time.time - triggerDownTime > PlayerPrefs.GetFloat("TriggerClickTime"))
                     Time.time - triggerDownTime > 0.2f) // For debug
            {
                ChangeToPistol();
            }
        }
    }

    void FixedUpdate()
    {
        UpdateEnergy();
    }

    /// <summary>
    /// Update the weapon energy when it's recharging and when melee is opened
    /// Because if the weapon energy reaches 0 or less than 0, it will start recharge, which will clamp the energy above 0,
    /// so we don't need to make sure the energy don't go below 0
    /// </summary>
    public void UpdateEnergy()
    {
        // Show low energy alert when the player is on low energy
        if (GameManager.currentEnergy <= lowEnergyPercent * GameManager.sPlayerTotalWeaponEnergy)
        {
            if (GameManager.gameManager.playerUI.gameObject.activeInHierarchy)
            {
                GameManager.gameManager.playerUI.startLowEnergyAni();
            }
        }
        // Hide low energy alert when the player is not on low energy
        if (GameManager.currentEnergy > lowEnergyPercent * GameManager.sPlayerTotalWeaponEnergy)
        {
            if (GameManager.gameManager.playerUI.gameObject.activeInHierarchy)
            {
                GameManager.gameManager.playerUI.stopLowEnergyAni();
            }
        }

        // If the weapon is in melee form
        if (hasWeapon && weaponForm == 1)
        {
            // Consume energy
            GameManager.currentEnergy -= swordEnergyPerSec * Time.fixedUnscaledDeltaTime / Time.timeScale;
        }

        // If the weapon cannot recharge energy
        if (!GameManager.canRechargeEnergy)
        {
            return;
        }

        // If the weapon is not in either controller
        if (!hasWeapon && !otherController.hasWeapon)
        {
            // Give energy (since this will be running on both controllers so each controller should recharge half of the recharge rate
            GameManager.currentEnergy = Mathf.Clamp(GameManager.currentEnergy +
                                                    energyRegenPerSec * Time.fixedUnscaledDeltaTime * 0.5f, 0, 1000);
        }

        // If the controller has weapon and the player is not using sword
        if (hasWeapon && weaponForm != 1)
        {
            // Give energy
            GameManager.currentEnergy = Mathf.Clamp(GameManager.currentEnergy +
                                                    energyRegenPerSec * Time.fixedUnscaledDeltaTime, 0, 1000);
        }

        // If the weapon is super charging
        if (hasWeapon && weaponForm == 4)
        {
            // Give more energy when super charging
            GameManager.currentEnergy = Mathf.Clamp(GameManager.currentEnergy +
                                                    superChargeEnergyPerSec * Time.fixedUnscaledDeltaTime, 0, 1000);

            // Play the super charge particle effect
            if (!catBasket.GetComponent<PlayerKartBasket>().superChargeParticle.isPlaying)
            {
                catBasket.GetComponent<PlayerKartBasket>().superChargeParticle.Play();
                catBasket.GetComponent<PlayerKartBasket>().superChargeParticle.
                    GetComponentInChildren<particleAttractorLinear>().target = transform;
            }
        }
    }

    /// <summary>
    /// When the weapon is grabbed
    /// </summary>
    public void GrabbedWeapon()
    {
        weaponModel.SetActive(true);

        if (weaponForm != 0) // If the weapon is not pistol then change to pistol
        {
            ChangeToPistol();
            isTriggerClickDown = false;
        }
        pistolRelatedReferences.pistolLaserSight.SetActive(true);
    }

    /// <summary>
    /// When the weapon is ungrabbed
    /// </summary>
    public void UngrabbedWeapon()
    {
        weaponModel.SetActive(false);

        if (weaponForm != 0) // If the weapon is not pistol then change to pistol
        {
            ChangeToPistol();
            isTriggerClickDown = false;
        }

        pistolRelatedReferences.pistolLaserSight.SetActive(false);
    }

    /// <summary>
    /// Check if the energy is 0, if is then stop melee
    /// </summary>
    public void CheckIfStopMeleeWhenEnergyEmpty()
    {
        if (GameManager.currentEnergy < 0)
        {
            if (!gripMelee) // If the melee is controlled by the trigger
            {
                isTriggerClickDown = false; // If the energy reaches 0, the player has to click the trigger again to
                                            // use the melee. It also allows the weapon change to shield if the melee
                                            // was holding in the shield posture
            }

            ChangeToPistol();
        }
    }

    /// <summary>
    /// Allow using pistol or active melee weapon when animation are finished
    /// </summary>
    public void CheckIfAnimationFinished()
    {
        if (weaponForm == 3 && weaponMeleeAnimation == null && weaponShieldAnimation == null) // If every animation is finished
        {
            weaponForm = nextForm;

            if (nextForm == 0) // If the weapon is going to change to the gun
            {
                if (hasWeapon) // Prevent run if the player ungrabbed the weapon
                {
                    pistolRelatedReferences.pistolLaserSight.SetActive(true); // show the laser sight
                }
            }
            else if (nextForm == 1) // If the weapon is going to change to the cat paw
            {
                //print("show melee");
                meleeRelatedReferences.sword.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Check if the trigger is holding down long enough to start using melee
    /// </summary>
    public void CheckIfStartMelee()
    {
        // If we use grip button to change melee
        if (gripMelee)
        {
            if (!isTriggerClickDown // If the trigger is not clicked
                && isGripClickDown // If the grip button is clicked
                && (weaponForm == 0 || weaponForm == 2 // If the weapon is pistol or shield
                || (weaponForm == 3 && nextForm != 1)) // If the weapon is transition and the target form is not melee
                && GameManager.currentEnergy >= minimumStartMeleeReq) // If the current energy is enough to start using sword
            {
                ChangeToMelee();
            }
        }
        else
        {
            if (//Time.time - triggerDownTime > PlayerPrefs.GetFloat("TriggerClickTime") // If the player is holding the trigger
                 isTriggerClickDown // If the trigger is down
                && (weaponForm == 0 || weaponForm == 2 || // If the weapon is pistol or shield
                (weaponForm == 3 && nextForm != 1)) // If the weapon is transition and the target form is not melee
                && GameManager.currentEnergy >= minimumStartMeleeReq) // If the current energy is enough to start using sword
            {

                if (GameManager.gameManager.allowMultiplayer && Time.time - triggerDownTime > 0.2f)
                {
                    ChangeToMelee();
                }
                else if (!GameManager.gameManager.allowMultiplayer &&
                         //Time.time - triggerDownTime > PlayerPrefs.GetFloat("TriggerClickTime"))
                         Time.time - triggerDownTime > 0.2f) // For debug
                {
                    ChangeToMelee();
                }
            }
        }
    }

    /// <summary>
    /// Change to melee transition
    /// </summary>
    public void ChangeToMelee()
    {
        nextForm = 1;
        weaponForm = 3;

        shieldRelatedReferences.shield.SetActive(false); // Close the shield

        if (weaponShieldAnimation != null) // Stop any shield animation
        {
            StopCoroutine(weaponShieldAnimation);
        }
        weaponShieldAnimation = StartCoroutine(CloseShieldWithWeaponTransformation(shieldRelatedReferences.animationTime));

        pistolRelatedReferences.pistolLaserSight.SetActive(false); // Hide the laser sight

        if (weaponMeleeAnimation != null) // Stop any melee animation
        {
            StopCoroutine(weaponMeleeAnimation);
        }
        weaponMeleeAnimation = StartCoroutine(OpenSwordWithWeaponTransformation(meleeRelatedReferences.switchTime));

        // Show melee UI icon
        playerUI.leftHandMelee.SetActive(true);
        playerUI.rightHandMelee.SetActive(true);
        // Hide pistol UI icon
        playerUI.leftHandPistol.SetActive(false);
        playerUI.rightHandPistol.SetActive(false);
    }

    /// <summary>
    /// Check the distance between the weapon and the super charge location
    /// </summary>
    public void CheckWeaponDistanceToSuperCharge()
    {
        // If super charging is not start and the weapon is within range
        if (catBasket.transform.InverseTransformPoint(transform.position).y <= superChargeArea.y &&
            Mathf.Abs(catBasket.transform.InverseTransformPoint(transform.position).x) <= superChargeArea.x &&
            Mathf.Abs(catBasket.transform.InverseTransformPoint(transform.position).z) <= superChargeArea.z &&
            weaponForm != 4)
        {
            //print("start super charge, form = " + weaponForm);

            pistolRelatedReferences.pistolLaserSight.SetActive(false);

            if (weaponForm != 0 || nextForm != 0) // If the weapon is not gun or not changing to gun
            {
                //print("is not gun");

                shieldRelatedReferences.shield.SetActive(false); // Close the shield

                if (weaponShieldAnimation != null) // Stop any shield animation
                {
                    StopCoroutine(weaponShieldAnimation);
                }
                weaponShieldAnimation = StartCoroutine(CloseShieldWithWeaponTransformation(shieldRelatedReferences.animationTime));

                //print("hide melee super charge");
                meleeRelatedReferences.sword.SetActive(false); // Close the cat paw

                if (weaponMeleeAnimation != null) // Stop any melee animation
                {
                    StopCoroutine(weaponMeleeAnimation);
                }
                weaponMeleeAnimation = StartCoroutine(CloseSwordWithWeaponTransformation(meleeRelatedReferences.switchTime));
            }

            weaponForm = 4;
        }

        // If is super charging but the weapon leaves range
        if (weaponForm == 4 &&
            (catBasket.transform.InverseTransformPoint(transform.position).y > superChargeArea.y ||
             Mathf.Abs(catBasket.transform.InverseTransformPoint(transform.position).x) > superChargeArea.x ||
             Mathf.Abs(catBasket.transform.InverseTransformPoint(transform.position).z) > superChargeArea.z))
        {
            ChangeToPistol();

            // Stop the super charge particle effect
            if (!catBasket.GetComponent<PlayerKartBasket>().superChargeParticle.isStopped)
            {
                catBasket.GetComponent<PlayerKartBasket>().superChargeParticle.Stop();
            }
        }

        // Enable weapon recharge when the weapon is super charging
        if (weaponForm == 4)
        {
            GameManager.canRechargeEnergy = true;
        }
    }

    /// <summary>
    /// Check the relative angle of the weapon (controller) and the player's head
    /// </summary>
    public void CheckWeaponRelativeAngle()
    {
        if (weaponForm == 4) // If the weapon is super charging
        {
            return;
        }

        shieldRelatedReferences.localEular = shieldRelatedReferences.controller.transform.localEulerAngles;

        // If it's the right controller
        if (transform.name == "RightController")
        {
            if (Vector3.Angle(shieldRelatedReferences.controller.transform.right, shieldRelatedReferences.head.transform.up) <=
                             (90f - shieldRelatedReferences.openShieldAngle) && // If the angle is within the switch to shield angle
                (weaponForm == 0 || (weaponForm == 3 && nextForm == 0)) && // If the weapon is pistol or changing to pistol
                !isTriggerClickDown && // If the trigger is not clicked down
                nextForm != 2) // If the weapon is not changing to shield
            {
                ChangeToShield();
            }

            if (Vector3.Angle(shieldRelatedReferences.controller.transform.right, shieldRelatedReferences.head.transform.up) >=
                             (90f - shieldRelatedReferences.closeShieldAngle) &&
                weaponForm == 2 &&
                !isTriggerClickDown) // Do not change to pistol is the trigger is down
            {
                //print(Vector3.Angle(controller.transform.right, head.transform.up));
                ChangeToPistol();
            }
        }
        // If it's the left controller
        else if (transform.name == "LeftController")
        {
            if (Vector3.Angle(shieldRelatedReferences.controller.transform.right, shieldRelatedReferences.head.transform.up) >=
                             (90f + shieldRelatedReferences.openShieldAngle) && // If the angle is within the switch to shield angle
                (weaponForm == 0 || (weaponForm == 3 && nextForm == 0)) && // If the weapon is pistol or changing to pistol
                !isTriggerClickDown && // If the trigger is not clicked down
                nextForm != 2) // If the weapon is not changing to shield
            {
                ChangeToShield();
            }

            if (Vector3.Angle(shieldRelatedReferences.controller.transform.right, shieldRelatedReferences.head.transform.up) <=
                             (90f + shieldRelatedReferences.closeShieldAngle) &&
                weaponForm == 2 &&
                !isTriggerClickDown) // Do not change to pistol is the trigger is down
            {
                //print(Vector3.Angle(controller.transform.right, head.transform.up));
                ChangeToPistol();
            }
        }
    }

    /// <summary>
    /// Change to shield transition
    /// </summary>
    public void ChangeToShield()
    {
        nextForm = 2;

        pistolRelatedReferences.pistolLaserSight.SetActive(false); // Hide the laser sight

        if (weaponShieldAnimation != null) // Stop any shield animation
        {
            StopCoroutine(weaponShieldAnimation);
        }
        weaponShieldAnimation = StartCoroutine(OpenShieldWithWeaponTransformation(shieldRelatedReferences.animationTime));

        //print("hide melee shield");
        meleeRelatedReferences.sword.SetActive(false);

        if (weaponMeleeAnimation != null) // Stop any melee animation
        {
            StopCoroutine(weaponMeleeAnimation);
        }
        weaponMeleeAnimation = StartCoroutine(CloseSwordWithWeaponTransformation(meleeRelatedReferences.switchTime));
    }

    /// <summary>
    /// Change to pistol transition
    /// </summary>
    public void ChangeToPistol()
    {
        nextForm = 0;
        weaponForm = 3;

        shieldRelatedReferences.shield.SetActive(false); // Close the shield

        if (weaponShieldAnimation != null) // Stop any shield animation
        {
            StopCoroutine(weaponShieldAnimation);
        }
        weaponShieldAnimation = StartCoroutine(CloseShieldWithWeaponTransformation(shieldRelatedReferences.animationTime));

        //print("hide melee pistol");
        meleeRelatedReferences.sword.SetActive(false); // Close the cat paw

        if (weaponMeleeAnimation != null) // Stop any melee animation
        {
            StopCoroutine(weaponMeleeAnimation);
        }
        weaponMeleeAnimation = StartCoroutine(CloseSwordWithWeaponTransformation(meleeRelatedReferences.switchTime));

        // Show pistol UI icon
        playerUI.leftHandPistol.SetActive(true);
        playerUI.rightHandPistol.SetActive(true);
        // Hide melee UI icon
        playerUI.leftHandMelee.SetActive(false);
        playerUI.rightHandMelee.SetActive(false);
    }

    /// <summary>
    /// Coroutine for the animation with gun model transformation
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator OpenSwordWithWeaponTransformation(float duration)
    {
        Quaternion startRotation = Quaternion.identity;
        Quaternion targetRotation = Quaternion.identity;
        startRotation.eulerAngles = meleeRelatedReferences.gunBarrel.transform.localEulerAngles;
        targetRotation.eulerAngles = new Vector3(-90, 0, 0);

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            meleeRelatedReferences.gunBarrel.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        meleeRelatedReferences.gunBarrel.transform.localEulerAngles = targetRotation.eulerAngles;

        weaponMeleeAnimation = null;
    }

    /// <summary>
    /// Coroutine for the animation with gun model transformation
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator CloseSwordWithWeaponTransformation(float duration)
    {
        Quaternion startRotation = Quaternion.identity;
        Quaternion targetRotation = Quaternion.identity;
        startRotation.eulerAngles = meleeRelatedReferences.gunBarrel.transform.localEulerAngles;
        targetRotation.eulerAngles = new Vector3(0, 0, 0);

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            meleeRelatedReferences.gunBarrel.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        meleeRelatedReferences.gunBarrel.transform.localEulerAngles = targetRotation.eulerAngles;

        weaponMeleeAnimation = null;
    }

    /// <summary>
    /// New open coroutine for the animation with gun model transformation
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator OpenShieldWithWeaponTransformation(float duration)
    {
        Vector3 startPosition = Vector3.zero;
        Vector3 targetPosition = Vector3.zero;
        targetPosition.z = -0.00434f;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            foreach (GameObject ring in shieldRelatedReferences.gunBarrelRings)
            {
                ring.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            }

            yield return null;
        }
        foreach (GameObject ring in shieldRelatedReferences.gunBarrelRings)
        {
            ring.transform.localPosition = targetPosition;
        }

        weaponShieldAnimation = null;
    }

    /// <summary>
    /// New close coroutine for the animation with gun model transformation
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator CloseShieldWithWeaponTransformation(float duration)
    {
        Vector3 startPosition = Vector3.zero;
        Vector3 targetPosition = Vector3.zero;
        startPosition.z = -0.00434f;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            foreach (GameObject ring in shieldRelatedReferences.gunBarrelRings)
            {
                ring.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            }

            yield return null;
        }
        foreach (GameObject ring in shieldRelatedReferences.gunBarrelRings)
        {
            ring.transform.localPosition = targetPosition;
        }

        weaponShieldAnimation = null;
    }

    /// <summary>
    /// Indication that the weapon is super charging
    /// </summary>
    public void SuperChargeIndication()
    {

    }
}
