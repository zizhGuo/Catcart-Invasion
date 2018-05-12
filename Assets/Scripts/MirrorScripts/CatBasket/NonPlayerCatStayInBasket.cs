using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCatStayInBasket : MonoBehaviour
{
    /// <summary>
    /// This is in charge of the cat in basket physics
    /// </summary>

    public bool isInBasket; // If this cat is in the basket
    public Coroutine getInCoroutine;
    public NonPlayerKartBasket nonPlayerKartBasket; // The basket on the non-player side

    // Use this for initialization
    void Start()
    {
        isInBasket = false;
        nonPlayerKartBasket = FindObjectOfType<NonPlayerKartBasket>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!nonPlayerKartBasket.cats.Contains(gameObject))
        {
            isInBasket = false;
        }

        if (isInBasket) // If the cat is in the basket
        {
            stay();
        }

        if (!isInBasket)
        {
            leave();
        }
    }

    public void stay() // Make the cat stay in the basket
    {
        if (gameObject.layer != LayerMask.NameToLayer("CatInBasket"))
        {
            gameObject.layer = LayerMask.NameToLayer("CatInBasket");

            GetComponent<NonPlayerSideMirror>().doMirror = false;
        }
    }

    public void leave() // Make the cat leaves the basket
    {
        if (gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }
}
