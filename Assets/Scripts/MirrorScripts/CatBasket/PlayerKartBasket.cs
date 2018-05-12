using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class PlayerKartBasket : MonoBehaviour
{
    /// <summary>
    /// This is in charge of the basket logic
    /// </summary>

    //public Text catCountText; // The text shows the count of cats
    public ParticleSystem superChargeParticle; // The particle effect for super charging the weapon

    public int catCount; // How many cats are currently in the basket
    public List<GameObject> cats; // The array contain's all the cat gameobject currently in the basket
    public NonPlayerKartBasket nonPlayerBasket; // The non-player mirror of the basket
    public Transform superChargingLocation; // The location where the player can "super charge" the weapon
    public EndRoomTrackStopCart suckerController; // The controller that controls the animation of the cat sucker at the end room

    // Use this for initialization
    void Start()
    {
        nonPlayerBasket = FindObjectOfType<NonPlayerKartBasket>();
        suckerController = FindObjectOfType<EndRoomTrackStopCart>();
    }

    // Update is called once per frame
    void Update()
    {
        catCount = cats.Count;
        //catCountText.text = "X " + cats.Count;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Cat") // If it is a cat in the basket
        {
            if (!cats.Contains(other.gameObject)) // If the basket is not already containing the cat
            {
                cats.Add(other.gameObject); // Add this cat to the basket

                // Active the PlayerCatStayInBasket component if the cat is put in the basket for the first time
                if (other.GetComponent<PlayerCatStayInBasket>().enabled == false)
                {
                    other.GetComponent<PlayerCatStayInBasket>().enabled = true;
                }

                // Delay setting cat.isInBasket a bit to wait for the cat basket cap to be effective for the cat
                other.GetComponent<PlayerCatStayInBasket>().getInCoroutine = StartCoroutine(keepPlayerCat(other.GetComponent<PlayerCatStayInBasket>())); // keep the cat in the basket
            }

            if (other.GetComponent<PlayerCatStayInBasket>().isInBasket)
            {
                if (other.GetComponent<VRTK_InteractableObject>().IsGrabbed()) // If the cat is being grabbed by the player
                {
                    other.GetComponent<PlayerCatStayInBasket>().isInBasket = false;
                    other.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerCatStayInBasket>().isInBasket = false;
                    cats.Remove(other.gameObject);
                    nonPlayerBasket.cats.Remove(other.GetComponent<PlayerSideMirror>().nonPlayerSideCopy);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Cat")
        {
            // Decrementing cat count in the ending room when player suck up each cat
            if (suckerController.suckerHandle.activeInHierarchy)
            {
                other.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<Collider>().enabled = false;
                nonPlayerBasket.cats.Remove(other.GetComponent<PlayerSideMirror>().nonPlayerSideCopy);
                cats.Remove(other.gameObject);
                return;
            }

            if (cats.Contains(other.gameObject))
            {
                StopCoroutine(other.GetComponent<PlayerCatStayInBasket>().getInCoroutine);
                other.GetComponent<PlayerCatStayInBasket>().isInBasket = false;

                cats.Remove(other.gameObject);
            }
        }
    }

    public IEnumerator keepPlayerCat(PlayerCatStayInBasket cat)
    {
        yield return new WaitForSeconds(1.5f);
        //yield return null;
        cat.isInBasket = true;
    }
}
