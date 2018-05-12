using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text score;
    public Text speed;
    public Text progression;
    public Text energy;
    public Text time;
    public GameObject glassDisplay; // The actual region that is going to display the HUD UI
    public GameObject enemyInfo; // The prefab to display enemy's information
    public GameObject lowEnergyWarning; // The warning sign for low energy
    public GameObject collisionWarning; // The warning for potential collision
    public GameObject incomingWarning; // The warning for incoming enemy attack
    public GameObject wrongDirectionWarning; // The warning to show when the player is going in the wrong direction
    public float showInterval; // The time in sec for the warning signs to turn on
    public float notShowInterval; // The time in sec for the warning signs to turn off
    public Text leftHandItem; // The item in the left hand
    public Text rightHandItem; // The item in the right hand
    public Image speedBar; // The filling meter that shows the catcart's speed
    public Image leftHandEnergyBar; // The filling meter that shows the energy on the left hand
    public Image rightHandEnergyBar; // The filling meter that shows the energy on the right hand
    public Text leftHandCatName; // The cat name text on the left hand
    public Text rightHandCatName; // The cat name text on the left hand
    public GameObject itemInfo; // The prefab to display item's information (as well as tell the player where it is)
    public GameObject leftHandCat; // The prefab that contains the cat icon image and cat name text to show the info of the cat that is currently in the left hand
    public GameObject leftHandWeapon; // The prefab that contains the weapon images and the energy bar if the weapon is in the left hand
    public GameObject leftHandPistol; // The image of pistol icon
    public GameObject leftHandMelee; // The image of the cat paw
    public GameObject leftHandLaserPointer; // The prefab that contains the laser pointer images if the laser pointer is in the left hand
    public GameObject rightHandCat; // The prefab that contains the cat icon image and cat name text to show the info of the cat that is currently in the right hand
    public GameObject rightHandWeapon; // The prefab that contains the weapon images and the energy bar if the weapon is in the right hand
    public GameObject rightHandPistol; // The image of pistol icon
    public GameObject rightHandMelee; // The image of the cat paw
    public GameObject rightHandLaserPointer; // The prefab that contains the laser pointer images if the laser pointer is in the right hand


    public Coroutine showLowEnergyAni; // The animation for display low energy warning
    public Coroutine showCollisionAni; // The animation for display collision warning
    public Coroutine showIncomingAni; // The animation for display incoming attack warning
    public Color lowEnergyColor; // The color for low energy warning
    public Color collisionColor; // The color for collision warning
    public Color incomingColor; // The color for incoming warning
    public Color wrongDirColor; // The color for wrong direction
    public Coroutine showWrongDirectionAni; // The animation for display incoming attack warning

    // Use this for initialization
    void Start()
    {
        lowEnergyColor = lowEnergyWarning.GetComponent<Text>().color;
        collisionColor = collisionWarning.GetComponent<Text>().color;
        incomingColor = incomingWarning.GetComponent<Text>().color;
        wrongDirColor = wrongDirectionWarning.GetComponent<Text>().color;

        lowEnergyWarning.GetComponent<Text>().color = Color.clear;
        collisionWarning.GetComponent<Text>().color = Color.clear;
        incomingWarning.GetComponent<Text>().color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Wrong Direction Animation
    public void startWrongDirAni()
    {
        if (!wrongDirectionWarning.activeInHierarchy)
        {
            wrongDirectionWarning.GetComponent<Text>().color = wrongDirColor;
            wrongDirectionWarning.SetActive(true);
            showWrongDirectionAni = StartCoroutine(wrongDirAni());
        }
    }

    public IEnumerator wrongDirAni()
    {
        while (wrongDirectionWarning.activeInHierarchy)
        {
            if (wrongDirectionWarning.GetComponent<Text>().color.a == 0) // If the sign is currently not showed
            {
                yield return new WaitForSeconds(notShowInterval);
                wrongDirectionWarning.GetComponent<Text>().color = wrongDirColor;
            }

            else // If the sign is currently showed
            {
                yield return new WaitForSeconds(showInterval);
                wrongDirectionWarning.GetComponent<Text>().color = Color.clear;
            }
        }
    }

    public void stopWrongDirAni()
    {
        if (wrongDirectionWarning.activeInHierarchy)
        {
            StopCoroutine(showWrongDirectionAni);
            wrongDirectionWarning.GetComponent<Text>().color = Color.clear;
            wrongDirectionWarning.SetActive(false);
        }
    }
    #endregion

    #region Low Energy Animation
    public void startLowEnergyAni()
    {
        if (!lowEnergyWarning.activeInHierarchy)
        {
            lowEnergyWarning.GetComponent<Text>().color = lowEnergyColor;
            lowEnergyWarning.SetActive(true);

            showLowEnergyAni = StartCoroutine(lowEnergyAni(leftHandWeapon.activeInHierarchy, rightHandWeapon.activeInHierarchy));
        }
    }

    public IEnumerator lowEnergyAni(bool leftWeapon, bool rightWeapon)
    {
        while (lowEnergyWarning.activeInHierarchy)
        {
            if (lowEnergyWarning.GetComponent<Text>().color.a == 0) // If the sign is currently not showed
            {
                yield return new WaitForSeconds(notShowInterval);
                lowEnergyWarning.GetComponent<Text>().color = lowEnergyColor;

                leftHandWeapon.SetActive(leftWeapon); // Blink left hand weapon if the weapon is in the left hand
                rightHandWeapon.SetActive(rightWeapon); // Blink right hand weapon if the weapon is in the right hand
            }

            else // If the sign is currently showed
            {
                yield return new WaitForSeconds(showInterval);
                lowEnergyWarning.GetComponent<Text>().color = Color.clear;

                leftHandWeapon.SetActive(false);
                rightHandWeapon.SetActive(false);
            }
        }
    }

    public void stopLowEnergyAni()
    {
        if (lowEnergyWarning.activeInHierarchy)
        {
            // Show back the weapon icon on the hand that has the weapon
            if (GameManager.sLeftController.GetComponent<HandPickItems>().currentItemName == "CATILIZER")
            {
                leftHandWeapon.SetActive(true);
            }
            if (GameManager.sRightController.GetComponent<HandPickItems>().currentItemName == "CATILIZER")
            {
                rightHandWeapon.SetActive(true);
            }

            StopCoroutine(showLowEnergyAni);
            lowEnergyWarning.GetComponent<Text>().color = Color.clear;
            lowEnergyWarning.SetActive(false);
        }
    }
    #endregion

    #region Collision Animation
    public void startCollisionAni()
    {
        if (!collisionWarning.activeInHierarchy)
        {
            collisionWarning.GetComponent<Text>().color = lowEnergyColor;
            collisionWarning.SetActive(true);
            showCollisionAni = StartCoroutine(collisionAni());
        }
    }

    public IEnumerator collisionAni()
    {
        while (collisionWarning.activeInHierarchy)
        {
            if (collisionWarning.GetComponent<Text>().color.a == 0) // If the sign is currently not showed
            {
                yield return new WaitForSeconds(notShowInterval);
                collisionWarning.GetComponent<Text>().color = lowEnergyColor;
            }

            else // If the sign is currently showed
            {
                yield return new WaitForSeconds(showInterval);
                collisionWarning.GetComponent<Text>().color = Color.clear;
            }
        }
    }

    public void stopCollisionAni()
    {
        if (collisionWarning.activeInHierarchy)
        {
            StopCoroutine(showCollisionAni);
            collisionWarning.GetComponent<Text>().color = Color.clear;
            collisionWarning.SetActive(false);
        }
    }
    #endregion

    #region Incoming Animation
    public void startIncomingAni()
    {
        if (!incomingWarning.activeInHierarchy)
        {
            incomingWarning.GetComponent<Text>().color = lowEnergyColor;
            incomingWarning.SetActive(true);
            showIncomingAni = StartCoroutine(incomingAni());
        }
    }

    public IEnumerator incomingAni()
    {
        while (incomingWarning.activeInHierarchy)
        {
            if (incomingWarning.GetComponent<Text>().color.a == 0) // If the sign is currently not showed
            {
                yield return new WaitForSeconds(notShowInterval);
                incomingWarning.GetComponent<Text>().color = lowEnergyColor;
            }

            else // If the sign is currently showed
            {
                yield return new WaitForSeconds(showInterval);
                incomingWarning.GetComponent<Text>().color = Color.clear;
            }
        }
    }

    public void stopIncomingAni()
    {
        if (incomingWarning.activeInHierarchy)
        {
            StopCoroutine(showIncomingAni);
            incomingWarning.GetComponent<Text>().color = Color.clear;
            incomingWarning.SetActive(false);
        }
    }
    #endregion
}
