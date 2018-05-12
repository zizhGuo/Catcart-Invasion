using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseShield : MonoBehaviour
{
    /// <summary>
    /// The opening and closing of the shield will be determined by the controller's Z orientation. 
    /// Close to 0 means the player is holding the controller like a pistol, so the shield should close.
    /// Close to 90 means the player is holding the controller like a shield, so the shield should open.
    /// </summary>
    public GameObject controller; // The controller which is the weapon
    public GameObject head; // Player's head
    public GameObject shield; // Player's shield
    public float openShieldAngle; // If the angle between the controller and head is greater than this value then the shield will open
    public float closeShieldAngle; // If the angle between the controller and head is greater than this value then the shield will close
    public Vector3 shieldSize; // The size for the shield
    public GameObject gun; // Player's default pistol weapon
    public GameObject sword; // Player's default melee weapon
    public GameObject gunModel; // Player's gun model used for transform animation
    public GameObject[] gunBarrelRings; // The coil rings on the gun barrel
    public float animationTime; // The time duration for the open or close shield animation

    public bool isShield; // Is the shield currently opened or not
    public Vector3 localEular; // The controller's local orientation
    public Coroutine openShieldCoroutine; // The coroutine for opening the sword
    public Coroutine closeShieldCoroutine; // The coroutine for closing the sword

    // Use this for initialization
    void Start()
    {
        isShield = false;
    }

    // Update is called once per frame
    void Update()
    {
        localEular = controller.transform.localEulerAngles;

        if (Vector3.Angle(controller.transform.right, head.transform.up) >= 90f - closeShieldAngle && isShield)
        {
            //print(Vector3.Angle(controller.transform.right, head.transform.up));
            CloseShield();
        }

        // When the player is using sword or super charging, he cannot use shield at the same time
        if (GetComponent<SwitchToMelee>().swordOpened ||
            GetComponent<SwitchToMelee>().openSwordCoroutine != null ||
            GameManager.sLeftController.GetComponent<CustomControllerEvents>().isSuperCharging ||
            GameManager.sRightController.GetComponent<CustomControllerEvents>().isSuperCharging)
        {
            if (isShield) // If the shield is opened, close the shield first
            {
                CloseShield();
            }

            return;
        }

        if (Vector3.Angle(controller.transform.right, head.transform.up) <= 90f - openShieldAngle && !isShield)
        {
            OpenShield();
        }

        //if(controller.transform.localEulerAngles.z >= openShieldAngle && controller.transform.localEulerAngles.z <= 180 && !isShield)
        //{
        //    OpenShield();
        //}
        //
        //if(controller.transform.localEulerAngles.z <= closeShieldAngle && isShield)
        //{
        //    CloseShield();
        //}
    }

    public void OpenShield()
    {
        isShield = true;
        shield.SetActive(true);
        gun.SetActive(false);

        if (openShieldCoroutine != null)
        {
            StopCoroutine(openShieldCoroutine);
        }
        if (closeShieldCoroutine != null)
        {
            StopCoroutine(closeShieldCoroutine);
        }

        openShieldCoroutine = StartCoroutine(OpenShieldWithWeaponTransformation(animationTime));
    }

    public void CloseShield()
    {
        isShield = false;
        shield.SetActive(false);

        if (openShieldCoroutine != null)
        {
            StopCoroutine(openShieldCoroutine);
        }
        if (closeShieldCoroutine != null)
        {
            StopCoroutine(closeShieldCoroutine);
        }

        closeShieldCoroutine = StartCoroutine(CloseShieldWithWeaponTransformation(animationTime));
    }

    /// <summary>
    /// New open coroutine for the animation with gun model transformation
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator OpenShieldWithWeaponTransformation(float duration)
    {
        gunModel.SetActive(true);

        Vector3 startPosition = Vector3.zero;
        Vector3 targetPosition = Vector3.zero;
        targetPosition.z = -0.00434f;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            foreach (GameObject ring in gunBarrelRings)
            {
                ring.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            }

            yield return null;
        }

        foreach (GameObject ring in gunBarrelRings)
        {
            ring.transform.localPosition = targetPosition;
        }

        openShieldCoroutine = null;
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
            foreach (GameObject ring in gunBarrelRings)
            {
                ring.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            }

            yield return null;
        }

        foreach (GameObject ring in gunBarrelRings)
        {
            ring.transform.localPosition = targetPosition;
        }

        // Switch to gun if the melee is not opened
        if (!GetComponent<SwitchToMelee>().swordOpened &&
            GetComponent<SwitchToMelee>().openSwordCoroutine == null)
        {
            gunModel.SetActive(false);
            gun.SetActive(true);
        }

        openShieldCoroutine = null;
    }
}
