using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToMelee : MonoBehaviour
{
    public GameObject gun; // Player's default pistol weapon
    public GameObject shield; // Player's default shield
    public GameObject sword; // Player's default melee weapon
    public float slowTimeFactor; // How slow the time will be compare to the normal time (0.5 means the time will goes half as slow)
    public float switchTime; // The time duration for the open or close sword animation
    public bool slowMo; // Does the melee trigger slow-mo or not
    public GameObject gunModel; // Player's gun model used for transform animation
    public Transform gunBarrel; // Player's gun barrel model used for transform animation

    public bool wasPistol; // Was the player using pistol or shield before he switch to the sword?
                           // When the player stops using sword, it will change back to the last item (shield or pistol)
    public float swordLength; // The length of the sword
    public Coroutine openSwordCoroutine; // The coroutine for opening the sword
    public Coroutine closeSwordCoroutine; // The coroutine for closing the sword
    public bool swordOpened; // Is the sword actually opened?

    // Use this for initialization
    void Start()
    {
        wasPistol = true;
        swordLength = sword.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startUsingMelee() // Start changing pistol to melee
    {
        //print(gun.activeInHierarchy);

        if (gun.activeInHierarchy)
        {
            wasPistol = true;
        }
        else
        {
            wasPistol = false;
        }

        gun.SetActive(false);
        shield.SetActive(false);

        if (openSwordCoroutine != null)
        {
            StopCoroutine(openSwordCoroutine);
        }
        if (closeSwordCoroutine != null)
        {
            StopCoroutine(closeSwordCoroutine);
        }

        openSwordCoroutine = StartCoroutine(OpenSwordWithWeaponTransformation(switchTime));

        //Time.timeScale = slowTimeFactor;
    }

    public void stopUsingMelee()
    {
        if (openSwordCoroutine != null)
        {
            StopCoroutine(openSwordCoroutine);
        }
        if (closeSwordCoroutine != null)
        {
            StopCoroutine(closeSwordCoroutine);
        }

        closeSwordCoroutine = StartCoroutine(CloseSwordWithWeaponTransformation(switchTime));
    }

    /// <summary>
    /// New open coroutine for the animation with gun model transformation
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator OpenSwordWithWeaponTransformation(float duration)
    {
        gunModel.SetActive(true);

        Quaternion startRotation = Quaternion.identity;
        Quaternion targetRotation = Quaternion.identity;
        startRotation.eulerAngles = gunBarrel.transform.localEulerAngles;
        targetRotation.eulerAngles = new Vector3(-90, 0, 0);

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            gunBarrel.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        gunBarrel.transform.localEulerAngles = targetRotation.eulerAngles;

        sword.SetActive(true);
        sword.GetComponentInChildren<Collider>().enabled = true;
        swordOpened = true;
        openSwordCoroutine = null;
    }

    /// <summary>
    /// New close coroutine for the animation with gun model transformation
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator CloseSwordWithWeaponTransformation(float duration)
    {
        sword.SetActive(false);
        sword.GetComponentInChildren<Collider>().enabled = false;

        Quaternion startRotation = Quaternion.identity;
        Quaternion targetRotation = Quaternion.identity;
        startRotation.eulerAngles = gunBarrel.transform.localEulerAngles;
        targetRotation.eulerAngles = new Vector3(0, 0, 0);

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            gunBarrel.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        gunBarrel.transform.localEulerAngles = targetRotation.eulerAngles;

        // Active the actual gun
        if (wasPistol && !GetComponent<OpenCloseShield>().isShield)
        {
            gun.SetActive(true);
            gunModel.SetActive(false);
        }
        else if (!wasPistol)
        {
            shield.SetActive(true);
        }

        swordOpened = false;
        closeSwordCoroutine = null;
    }

    public IEnumerator openSword(float duration)
    {
        sword.SetActive(true);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            sword.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(-0.054f, 0.5f - 0.054f, t));
            sword.transform.localScale = new Vector3(Mathf.Lerp(0, 1.193156f, t), sword.transform.localScale.y, sword.transform.localScale.z);

            if (slowMo)
            {
                Time.timeScale = Mathf.Lerp(1, slowTimeFactor, t);
            }

            yield return null;
        }

        sword.GetComponentInChildren<Collider>().enabled = true;
        openSwordCoroutine = null;
    }

    public IEnumerator closeSword(float duration)
    {
        sword.GetComponentInChildren<Collider>().enabled = false;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            sword.transform.localPosition = new Vector3(0, 0, Mathf.Lerp(0.5f - 0.054f, -0.054f, t));
            sword.transform.localScale = new Vector3(Mathf.Lerp(1.193156f, 0, t), sword.transform.localScale.y, sword.transform.localScale.z);

            if (slowMo)
            {
                Time.timeScale = Mathf.Lerp(slowTimeFactor, 1, t);
            }

            yield return null;
        }

        sword.SetActive(false);
        closeSwordCoroutine = null;
    }

}
