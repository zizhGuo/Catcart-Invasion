using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatStayInKart : MonoBehaviour
{
    public float moveBackSpeed; // How fast is the cat moving back to it's location in the basket

    public bool isInBasket; // If this cat is in the basket
    public Vector3 positionInBasket; // The initial position of the cat in the basket
    public KartBasket basket; // The basket for the cats
    public Coroutine getInCoroutine; // The coroutine for the cat to stay in the basket, this need to be stopped if the cat is out of the basket before runs.

    // Use this for initialization
    void Start()
    {
        isInBasket = false;
        basket = FindObjectOfType<KartBasket>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
        if(gameObject.layer != LayerMask.NameToLayer("CatInBasket"))
        {
            //GetComponent<Rigidbody>().useGravity = false;
            gameObject.layer = LayerMask.NameToLayer("CatInBasket");
        }
        //print(GameManager.currentSpeed);
        transform.position += (basket.transform.TransformPoint(positionInBasket) - transform.position) * Time.fixedUnscaledDeltaTime * Mathf.Clamp(Mathf.Pow(1.2f, Mathf.Clamp(GameManager.kartMovementInfo.currentSpeed, 1, 100)), 10, 1000) * moveBackSpeed;
        //GetComponent<Rigidbody>().MovePosition(basket.transform.TransformPoint(positionInBasket));
        transform.rotation = Quaternion.identity;
        //transform.position = basket.transform.TransformPoint(positionInBasket);
        //transform.position = basket.transform.TransformPoint(positionInBasket + Vector3.up * BetterRandom.betterRandom());
    }

    public void leave() // Make the cat leaves the basket
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        //GetComponent<Rigidbody>().useGravity = true;
    }
}
