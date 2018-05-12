using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoOnUI : MonoBehaviour
{
    /// <summary>
    /// This script contains some info of enemy that will be displayed on player's HUD UI
    /// It will also control the UI of this enemy's info that is actually showing on player's HUD UI
    /// </summary>

    public LayerMask playerHUDLayer; // The layerMask which let the UI raycast only collide with the player's HUD.
    public string enemyName; // The name of the enemy to show on the HUD UI for enemy detial
    public float arrowSizeMinDist; // What is the distance from the enemy to the player that the arrow width should cap at maximum
    public float arrowSizeMaxDist; // What is the distance from the enemy to the player that the arrow width should cap at minimum
    public float HUDelementsAlpha; // The alpha value for the color for the HUD elements when they appears

    public GameObject playerHead;
    public GameObject playerEye; // The position of the player's eyes (it will be a bit in front of the head's position)
    public PlayerUI playerHUD; // The actual UI canvas on the HUD;
    public GameObject playerHUDdisplay; // The glass display game object on the HUD model;
    public GameObject infoOnHUD; // The UI game object which contains this enemy's info that is showed on the player's HUD UI
    public GameObject detailOnHUD; // The UI game object with enemy's detailed info if the enemy is within the HUD display area
    public GameObject arrowOnHUD; // The UI game object which is an arrow showing the enemy's relative position if the enemy is out of the HUD display area
    public RaycastHit hitInfo;
    public bool isAimedByPlayerGun; // If it is aimed by the player's gun's laser sight
    public bool inHUDarea; // If the enemy is in the HUD view area
    public Animator uiAnimator; // The animator that controlls UI animation
    public bool playAppearing; // Do we play the appearing animation
    public bool appearingFinished; // Has the appearing animation finished
    public bool playAiming; // Do we play the aiming animation
    public bool aimingFinished; // Has the animing animation finished
    public float aimingAnimationSpeed; // Controls the reverse of the aiming animation
    public bool shouldShow; // Should this enemy show HUD element or not

    /// <summary>
    /// Below is to calculate where the enemy arrow should be placed on the screen if the enemy is out of the player's HUD display region
    /// </summary>
    public Vector3 relativePositionToPlayer; // Enemy's "local" position relate to player's head
    public Vector3 vectorToCalculateAngle; // This vector will be the "local" position vector project onto the player head's local x-y plane
    public float relativeAngleToPlayerHeadUp; // This float will store the angle between vectorToCalculateAngle and player head's local up vector
    public Vector3 finalPositionOnHUD; // The calculated result of the local position for the arrow to be placed on the HUD
    public Vector3 vectorToCalculateBack; // Used to calculate the angle between player eye's "up" and enemy's yz vector
    public float relativeAngleToPlayerHeadUpBack; // This float will store the angle between vectorToCalculateBack and player head's local up vector

    // Test
    public float nomalizedAimingAnimation;
    public float startTime;
    public float fixedStartTime;

    // Use this for initialization
    void Start()
    {
        playerHead = GameManager.gameManager.playerHead;
        playerEye = PlayerEyeLocator.thisGameObject;
        playerHUD = GameManager.gameManager.playerUI;
        //playerHUDdisplay = playerHUD.transform.parent.parent.gameObject;
        playerHUDdisplay = GameManager.gameManager.playerHUDviewArea.transform.parent.Find("HUD_Glass_Temp").gameObject;

        infoOnHUD = Instantiate(playerHUD.enemyInfo, playerHUD.transform);
        detailOnHUD = infoOnHUD.GetComponent<EnemyInfoUIelements>().detail;
        detailOnHUD.GetComponentInChildren<Text>().text = enemyName;
        arrowOnHUD = infoOnHUD.GetComponent<EnemyInfoUIelements>().arrow;
        uiAnimator = infoOnHUD.GetComponent<EnemyInfoUIelements>().uiAnimator;
        inHUDarea = false;

        //detailOnHUD.SetActive(true);
        //playAppearing = true;
        //uiAnimator.SetBool("playAppearing", playAppearing);

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy is not in the HUD view area
        if (!inHUDarea)
        {
            playAppearing = true; // Reset the appearing animation to be played next time the enemy enters the HUD area
            infoOnHUD.GetComponent<EnemyInfoUIelements>().detailImage.sprite = infoOnHUD.GetComponent<EnemyInfoUIelements>().defaultDetailUIimage;
        }

        if (inHUDarea)
        {
            ControlAnimation(); // Control the UI animation
        }
    }

    /// <summary>
    /// Controls the UI animations
    /// </summary>
    public void ControlAnimation()
    {
        if (!detailOnHUD.activeInHierarchy)
        {
            return;
        }

        // If the enemy just enters the HUD view area
        if (playAppearing)
        {
            uiAnimator.SetBool("playAppearing", playAppearing);

            // Prevent the appearing animation from playing again
            //playAppearing = false;
        }

        // If the appearing animation finished
        if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("AimerAppearAnimation") &&
            uiAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 &&
            !uiAnimator.IsInTransition(0))
        {
            appearingFinished = true;
            uiAnimator.SetBool("appearingFinished", playAppearing);

            // Prevent the appearing animation from playing again
            playAppearing = false;
            uiAnimator.SetBool("playAppearing", playAppearing);
        }

        if (appearingFinished &&
            uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            appearingFinished = false;
            uiAnimator.SetBool("appearingFinished", playAppearing);

            // Prevent the appearing animation from playing again
            //playAppearing = false;
            //uiAnimator.SetBool("playAppearing", playAppearing);
        }

        // Don't do anything until the appearing animation finished
        if (playAppearing)
        {
            return;
        }

        // If the enemy is aimed by the player's gun
        if (isAimedByPlayerGun)
        {
            // If the enemy's current UI is at the normal state
            if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                playAiming = true;
                uiAnimator.SetBool("playAiming", playAiming);
                aimingAnimationSpeed = 1;
                uiAnimator.SetFloat("aimingAnimationSpeed", aimingAnimationSpeed);
                aimingFinished = false;
                uiAnimator.SetBool("aimingFinished", aimingFinished);
            }

            // If the aiming animation is rewinding (when the player's gun leaves the enemy)
            if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("AimerAimingAnimation") &&
                aimingAnimationSpeed == -1)
            {
                aimingAnimationSpeed = 1;
                uiAnimator.SetFloat("aimingAnimationSpeed", aimingAnimationSpeed);
            }

            // IF the aiming animation finished
            if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("AimerAimingAnimation") &&
                uiAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 &&
                !uiAnimator.IsInTransition(0))
            {
                playAiming = false;
                uiAnimator.SetBool("playAiming", playAiming);
                aimingAnimationSpeed = 0;
                uiAnimator.SetFloat("aimingAnimationSpeed", aimingAnimationSpeed);
            }
        }

        // If the enemy is not aimed by the player's gun
        if (!isAimedByPlayerGun)
        {
            // If the enemy's current UI is at the aiming state
            if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("AimerAimingAnimation") &&
                !playAiming)
            {
                aimingAnimationSpeed = -1;
                uiAnimator.SetFloat("aimingAnimationSpeed", aimingAnimationSpeed);
                //uiAnimator.Play("AimerAimingAnimation");
            }

            // If the aiming animation finished
            if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("AimerAimingAnimation") &&
                uiAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0 &&
                !uiAnimator.IsInTransition(0))
            {
                playAiming = false;
                uiAnimator.SetBool("playAiming", playAiming);
                aimingAnimationSpeed = 0;
                uiAnimator.SetFloat("aimingAnimationSpeed", aimingAnimationSpeed);
                aimingFinished = true;
                uiAnimator.SetBool("aimingFinished", aimingFinished);
            }

            // If the aiming animation is playing (when the player's gun just aimed at the enemy)
            if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("AimerAimingAnimation") &&
                aimingAnimationSpeed == 1 &&
                uiAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                aimingAnimationSpeed = -1;
                uiAnimator.SetFloat("aimingAnimationSpeed", aimingAnimationSpeed);
            }

            // IF the state already goes back to normal
            if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !uiAnimator.IsInTransition(0))
            {
                aimingFinished = false;
                uiAnimator.SetBool("aimingFinished", aimingFinished);
            }
        }

        //// If no animation is played then reset the sprite to normal sprite
        //if (uiAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
        //   !uiAnimator.IsInTransition(0))
        //{
        //    print("change");
        //    uiAnimator.
        //    infoOnHUD.GetComponent<EnemyInfoUIelements>().detailImage.sprite = infoOnHUD.GetComponent<EnemyInfoUIelements>().defaultDetailUIimage;
        //}

        //nomalizedAimingAnimation = uiAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    /// <summary>
    /// Reset the parameters for the HUD aimer animation
    /// </summary>
    public void ResetAnimationParameters()
    {
        playAppearing = false;
        appearingFinished = false;
        playAiming = false;
        aimingFinished = false;
    }

    void FixedUpdate()
    {
        //Debug.DrawRay(transform.position, Vector3.Normalize(playerHead.transform.position - transform.position) * 100, Color.red);
        //if (Physics.Raycast(transform.position, Vector3.Normalize(playerHead.transform.position - transform.position), out hitInfo, Mathf.Infinity))
        //{
        //    print("general: " + hitInfo.collider.name);
        //}
        if (shouldShow)
        {
            if (GetComponentInChildren<Image>() && GetComponentInChildren<Image>().color.a == 0)
            {
                foreach (Text t in GetComponentsInChildren<Text>())
                {
                    t.color = new Color(t.color.r, t.color.g, t.color.b, HUDelementsAlpha);
                }
                foreach (Image i in GetComponentsInChildren<Image>())
                {
                    i.color = new Color(i.color.r, i.color.g, i.color.b, HUDelementsAlpha);
                }
            }
        }
        else
        {
            if (GetComponentInChildren<Image>() && GetComponentInChildren<Image>().color.a != 0)
            {
                foreach (Text t in GetComponentsInChildren<Text>())
                {
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 0);
                }
                foreach (Image i in GetComponentsInChildren<Image>())
                {
                    i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
                }
            }

            return;
        }

        if (Physics.Raycast(transform.position, Vector3.Normalize(playerHead.transform.position - transform.position), out hitInfo, Mathf.Infinity, playerHUDLayer)
            && playerHUDdisplay.transform.InverseTransformPoint(transform.position).z > 0) // If the enemy is within the HUD display area and is in front of the display (not behind the player)
        {

            if (fixedStartTime == 0)
            {
                fixedStartTime = Time.time;
            }
            //print(hitInfo.collider.name);

            if (!detailOnHUD.activeInHierarchy) // If the enemy detail is not active
            {
                infoOnHUD.transform.localRotation = Quaternion.identity;
                detailOnHUD.SetActive(true);
                arrowOnHUD.SetActive(false);
            }

            inHUDarea = true;
            infoOnHUD.transform.LookAt(playerEye.transform);
            infoOnHUD.transform.position = hitInfo.point + (transform.position - playerHead.transform.position) * 1.5f;
        }

        else // If the enemy is out of the HUD display area
        {
            // Reset the parameters for the HUD aimer animation when the enemy move out of the HUD view area
            if (inHUDarea)
            {
                ResetAnimationParameters();
            }

            inHUDarea = false;

            //print(Vector3.Distance(transform.position, playerHUDdisplay.transform.position));
            relativePositionToPlayer = playerEye.transform.InverseTransformPoint(transform.position);
            relativePositionToPlayer.y += Vector3.Distance(transform.position, playerEye.transform.position) * 0.16f;
            //print(relativePositionToPlayer); //This is correct
            vectorToCalculateAngle = relativePositionToPlayer - playerEye.transform.InverseTransformDirection(playerEye.transform.forward) * relativePositionToPlayer.z;
            //print(vectorToCalculateAngle); //This is correct
            relativeAngleToPlayerHeadUp =
                Vector3.Angle(playerEye.transform.InverseTransformDirection(playerEye.transform.up), vectorToCalculateAngle) *
                Mathf.Sign(Vector3.Cross(playerEye.transform.InverseTransformDirection(playerEye.transform.up), vectorToCalculateAngle).z);
            //relativeAngleToPlayerHeadUp 
            //print("up: " + relativeAngleToPlayerHeadUp); //This is correct

            vectorToCalculateBack = relativePositionToPlayer - playerEye.transform.InverseTransformDirection(playerEye.transform.right) * relativePositionToPlayer.x;
            //print(vectorToCalculateBack);
            relativeAngleToPlayerHeadUpBack =
                Vector3.Angle(playerEye.transform.InverseTransformDirection(playerEye.transform.up), vectorToCalculateBack);
            //print("back: " + relativeAngleToPlayerHeadUpBack);
            //  (Sign of (relativeAngleToPlayerHeadUp)) + |sin(relativeAngleToPlayerHeadUpBack + 90)| * 
            //  |relativeAngleToPlayerHeadUp - (Sign of (relativeAngleToPlayerHeadUp)) * 90| * (Sign of ((Sign of (relativeAngleToPlayerHeadUp)) * 90))

            //print(playerHUDdisplay.transform.InverseTransformPoint(transform.position).z);
            // Only weight the arrow position if the enemy is behind the player
            if (playerHUDdisplay.transform.InverseTransformPoint(transform.position).z < 0)
            {
                relativeAngleToPlayerHeadUp =
                    (Mathf.Sign(relativeAngleToPlayerHeadUp) * 90) +
                    Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * (relativeAngleToPlayerHeadUpBack + 90))) *
                    Mathf.Abs(relativeAngleToPlayerHeadUp - Mathf.Sign(relativeAngleToPlayerHeadUp) * 90) *
                    Mathf.Sign(relativeAngleToPlayerHeadUp - Mathf.Sign(relativeAngleToPlayerHeadUp) * 90);
            }

            //print(Mathf.Sign(relativeAngleToPlayerHeadUp) + " * 90) + " + "||)
            //print("final: " + relativeAngleToPlayerHeadUp);

            //Debug.DrawRay(playerEye.transform.position, (playerEye.transform.TransformPoint(relativePositionToPlayer) - playerEye.transform.position), Color.yellow);
            //Debug.DrawRay(playerEye.transform.position, playerEye.transform.TransformDirection(vectorToCalculateAngle), Color.green);

            //If the angle is between the top left and right corner of the display (Around -55 ~ 55 degree)
            if (relativeAngleToPlayerHeadUp >= -Mathf.Atan(487f / 342f) * Mathf.Rad2Deg && relativeAngleToPlayerHeadUp <= Mathf.Atan(487f / 342f) * Mathf.Rad2Deg)
            {
                finalPositionOnHUD.y = 342;
                finalPositionOnHUD.x = 342f * -Mathf.Tan(relativeAngleToPlayerHeadUp * Mathf.Deg2Rad);
            }
            //If the angle is between the bottom left and right corner of the display (-180 ~ -125 || 125 ~ 180)
            else if (relativeAngleToPlayerHeadUp <= -180f + Mathf.Atan(487f / 342f) * Mathf.Rad2Deg || relativeAngleToPlayerHeadUp >= 180f - Mathf.Atan(487f / 342f) * Mathf.Rad2Deg)
            {
                finalPositionOnHUD.y = -342;
                finalPositionOnHUD.x = -342f * -Mathf.Tan(relativeAngleToPlayerHeadUp * Mathf.Deg2Rad);
            }
            //If the angle is between the top left and bottom left corner of the display (55 ~ 125)
            else if (relativeAngleToPlayerHeadUp > Mathf.Atan(487f / 342f) * Mathf.Rad2Deg && relativeAngleToPlayerHeadUp < 180f - Mathf.Atan(487f / 342f) * Mathf.Rad2Deg)
            {
                finalPositionOnHUD.x = -487;
                finalPositionOnHUD.y = -487f * Mathf.Tan((relativeAngleToPlayerHeadUp + 90f) * Mathf.Deg2Rad);
            }
            //If the angle is between the top right and bottom right corner of the display (-55 ~ -125)
            else if (relativeAngleToPlayerHeadUp < -Mathf.Atan(487f / 342f) * Mathf.Rad2Deg && relativeAngleToPlayerHeadUp > -180f + Mathf.Atan(487f / 342f) * Mathf.Rad2Deg)
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

            arrowOnHUD.GetComponent<RectTransform>().sizeDelta =
                new Vector2(175 - 105 *
                    Mathf.Clamp01(
                        Mathf.Pow((
                            Mathf.Clamp(
                                Vector3.Distance(playerEye.transform.position, transform.position),
                            arrowSizeMinDist, arrowSizeMaxDist) -
                        arrowSizeMinDist), 1.6f) /
                    Mathf.Pow(arrowSizeMaxDist, 1.6f)),
                arrowOnHUD.GetComponent<RectTransform>().rect.height);
        }
    }

    void OnDestroy()
    {
        Destroy(infoOnHUD);
    }
}
