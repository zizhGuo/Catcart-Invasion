using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerCatStayInBasket : MonoBehaviour
{
    /// <summary>
    /// This is in charge of the cat in basket logic
    /// </summary>

    public float distanceToJumpBack; // When the distance between cat and cat basket is within this value, the cat will jump back to the basket itself
    public float speedLimitToJumpBack; // The player's kart's speed cannot go above this number if he wants the cat to jump back itself, so if the player drives really fast the cat will not jump back to basket at all
    public float jumpDuration; // The duration of the jump animation
    public float jumpHeight; // How high the cat will jump
    public GameObject nonPlayerCat; // Use the generate its non player side copy

    public bool isInBasket; // If this cat is in the basket
    public Coroutine getInCoroutine; // The coroutine for the cat to stay in the basket, this need to be stopped if the cat is out of the basket before runs.
    public GameObject playerKartBasket; // Player's side basket
    //Jump back
    public Vector3 startJumpPosition; // The position the cat start jumping
    public bool isJumping; // If the cat is currently jumping then we don't want it to jump;
    public Vector3 jumpDestination; // Where the cat is going to land
    public Coroutine jumpCoroutine;

    // Use this for initialization
    void Start()
    {
        playerKartBasket = FindObjectOfType<GameManager>().catBasket;
        isInBasket = false;
        isJumping = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<PlayerSideMirror>().inKart && !isJumping)
        {
            isJumping = false;

            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }
        }

        if (playerKartBasket == null)
        {
            playerKartBasket = FindObjectOfType<GameManager>().catBasket;
        }

        if (isInBasket) // If the cat is in the basket
        {
            stay();
        }

        if (!isInBasket)
        {
            leave();
        }

        if (isJumping)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        /*
        //------------------------Cat Jump
        //Make sure that the cat is within the range, and player's kart is slow enough. Also, make sure the cat is in front of the basket
        if (!GetComponent<PlayerSideMirror>().inKart && !isJumping && Vector3.Distance(transform.position, playerKartBasket.transform.position) < distanceToJumpBack
                                                                   && Vector3.Distance(transform.position, playerKartBasket.transform.position) > 1 && GameManager.kartCurrentVelocity.magnitude <= speedLimitToJumpBack)
        {
            //if (transform.name == "PlayerCat")
            //{
            //    print(GetComponent<PlayerSideMirror>().inKart + " " + Vector3.Distance(transform.position, playerKartBasket.transform.position));
            //}

            isJumping = true;
            jumpCoroutine = StartCoroutine(jumpBack());
        }
        */

        //------------------------Basket levitation
        //Make sure that the cat is within the range. Also, make sure the cat is in front of the basket
        if (!GetComponent<PlayerSideMirror>().inKart && !isJumping && Vector3.Distance(transform.position, playerKartBasket.transform.position) < distanceToJumpBack
                                                                   && Vector3.Distance(transform.position, playerKartBasket.transform.position) > 1
                                                                   && GetComponent<Collider>().enabled
                                                                   && !GetComponent<VRTK_InteractableObject>().IsGrabbed())
        {
            isJumping = true;

            if (GetComponent<MoveWithBoss>()) // If the cat is moving with boss
            {
                GetComponent<MoveWithBoss>().enabled = false;
            }

            jumpCoroutine = StartCoroutine(levitatingBack());
        }
    }

    void LateUpdate()
    {
    }

    public void stay() // Make the cat stay in the basket
    {
        if (gameObject.layer != LayerMask.NameToLayer("CatInBasket"))
        {
            gameObject.layer = LayerMask.NameToLayer("CatInBasket");

            GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerCatStayInBasket>().isInBasket = true;
            GetComponent<PlayerSideMirror>().doMirror = true;
        }
    }

    public void leave() // Make the cat leaves the basket
    {
        if (gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerCatStayInBasket>().isInBasket = false;
            GetComponent<PlayerSideMirror>().doMirror = false;
        }
    }

    public IEnumerator jumpBack() // Cat will jump back to the basket if the player's kart is close enough
    {
        startJumpPosition = transform.position;
        jumpDestination = playerKartBasket.transform.position;// - playerKartBasket.transform.forward;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / jumpDuration)
        {
            //if (GetComponent<PlayerSideMirror>().inKart)
            //{
            //    isJumping = false;

            //    if(transform.name == "PlayerCat (8)")
            //    {
            //        //print(GetComponent<PlayerSideMirror>().inKart);
            //    }

            //    yield break;
            //}

            transform.position = startJumpPosition + (jumpDestination - startJumpPosition) * t + jumpHeight * (Mathf.Log10(3 * t + 0.1f) + 1f) / 3f * Vector3.up;
            //GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerSideMirror>().doMirror = true;

            yield return null;
        }

        isJumping = false;
    }

    public IEnumerator levitatingBack() // Cat will be levitate back to the basket if the player's kart is close enough
    {
        //print(Time.inFixedTimeStep);

        startJumpPosition = playerKartBasket.transform.InverseTransformPoint(transform.position);
        jumpDestination = Vector3.up * jumpHeight * 0.4f + Vector3.forward * GameManager.kartCurrentVelocity.magnitude / 250f;

        for (float t = 0.0f; t < 1.0f; t += Time.fixedUnscaledDeltaTime / jumpDuration / 2f)
        {
            transform.position = playerKartBasket.transform.TransformPoint(startJumpPosition) +
                                (playerKartBasket.transform.TransformPoint(jumpDestination) - playerKartBasket.transform.TransformPoint(startJumpPosition)) * (t);

            yield return null;
        }

        //transform.position = playerKartBasket.transform.TransformPoint(jumpDestination);

        GetComponent<PlayerSideMirror>().lastRelativeVelocity = Vector3.zero;

        //for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / jumpDuration * 4)
        //{
        //    transform.position = playerKartBasket.transform.TransformPoint(jumpDestination); ;

        //    yield return null;
        //}
        //print("jump" + GetComponent<PlayerSideMirror>().relativeVelocity);
        isJumping = false;

        // If the cat is rescued by player from boss and still don't have its non-player side copy
        //if (FindObjectOfType<BossBehavior>().started && GetComponent<PlayerSideMirror>().nonPlayerSideCopy == null)
        //{
        //    GetComponent<PlayerSideMirror>().nonPlayerSideCopy = Instantiate(nonPlayerCat, FindObjectOfType<GameManager>().nonPlayerKart.transform.TransformPoint(
        //                                                                                    FindObjectOfType<GameManager>().playerKart.transform.InverseTransformPoint(
        //                                                                                      transform.position)),
        //                                                                                 transform.rotation); // Create the non player side copy
        //    GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerSideMirror>().playerSideCopy = gameObject;

        //    FindObjectOfType<BossBehavior>().targetSpeed = FindObjectOfType<BossBehavior>().chargeSpeed;
        //    FindObjectOfType<BossMovement>().chargeBack();
        //}
    }
}
