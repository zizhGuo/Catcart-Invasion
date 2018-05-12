using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Move cat back to container if they move too far away from the player in the tutorial
/// </summary>
public class MoveCatBackToContainer : MonoBehaviour
{


    public Vector3 catReappearPosition; // Where the cat should re-appear
    public Quaternion catReappearRotation;
    public bool inCatContainer; // Is the cat in the cat container
    public Transform playerKartTransform; // Player's cart's transform
    public bool isOnTheFloor; // Is the cat on the floor
    public Coroutine putBackCatCoroutine; // The coroutine that plays the put cat back animation
    public float catDropTime; // The time when the cat is dropped from the dropper
    public static List<MoveCatBackToContainer> droppedCats = new List<MoveCatBackToContainer>(); // A list that tracks the cats on the tutorial floor

    // Use this for initialization
    void Start()
    {
        playerKartTransform = GameManager.gameManager.playerKart.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 6)
        {
            return;
        }

        CheckCatStatus();
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.name == "Bottom")
        {
            inCatContainer = true;
        }

        if (collision.collider.name == "Floor")
        {
            isOnTheFloor = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "Bottom")
        {
            inCatContainer = false;
        }

        if (collision.collider.name == "Floor")
        {
            isOnTheFloor = false;
        }
    }

    /// <summary>
    /// Checking where the cat is
    /// </summary>
    public void CheckCatStatus()
    {
        // If the player is grabbing the cat
        if (GetComponent<VRTK_InteractableObject>().IsGrabbed())
        {
            // Stop the put back process if the player re-grab the cat
            if (putBackCatCoroutine != null)
            {
                StopCoroutine(putBackCatCoroutine);
                putBackCatCoroutine = null;
            }

            return;
        }

        // If the cat is already being putting back
        if (putBackCatCoroutine != null)
        {
            return;
        }

        // If the cat is just dropped
        if (Time.time - catDropTime <= 5)
        {
            return;
        }

        // If there are cats dropped before it
        if (MoveCatBackToContainer.droppedCats.Count > 0)
        {
            return;
        }

        // If the cat is not above the ground or too high up
        if (transform.position.y < 0 || transform.position.y > 10)
        {
            MoveCatBackToContainer.droppedCats.Add(this);
            putBackCatCoroutine = StartCoroutine(PutCatBackInContainer(false));
        }

        // If the cat is on the floor
        if (isOnTheFloor)
        {
            MoveCatBackToContainer.droppedCats.Add(this);
            putBackCatCoroutine = StartCoroutine(PutCatBackInContainer(true));
        }

        // If the cat is too far away from the player
        if (Vector3.Distance(playerKartTransform.position, transform.position) > 30)
        {
            MoveCatBackToContainer.droppedCats.Add(this);
            putBackCatCoroutine = StartCoroutine(PutCatBackInContainer(false));
        }
    }

    /// <summary>
    /// Put cat back in the cat container if the cat is too far away from the player 
    /// </summary>
    /// <param name="showSucker"></param>
    public IEnumerator PutCatBackInContainer(bool showSucker)
    {
        // If the cat is rolling away slowly on the tutorial room floor
        if (showSucker)
        {
            yield return new WaitForSeconds(1); // Let the cat move for an extra second

            while (!isOnTheFloor) // Wait until the cat is touching the floor
            {
                yield return null;
            }

            // Stop the cat's movement
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // Move the cat sucker above the cat
            TutorialManager.tutorialManager.catSucker.transform.position =
                new Vector3(transform.position.x, TutorialManager.tutorialManager.catSucker.transform.position.y, transform.position.z);

            // Prevent the player from grab the cat after the animation start playing
            GetComponent<VRTK_InteractableObject>().isGrabbable = false; 
            // Start sucker tube animation
            StartCoroutine(TutorialManager.tutorialManager.catSucker.GetComponent<LinearObjectMovement>().Animate());

            // Wait for the animation
            while (!TutorialManager.tutorialManager.catSucker.GetComponent<LinearObjectMovement>().pause)
            {
                yield return null;
            }

            // Suck up cat
            if (TutorialManager.tutorialManager.catSucker.GetComponent<LinearObjectMovement>().pause)
            {
                for (float t = 0; t < 1; t += Time.deltaTime)
                {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    Vector3 newPosi = transform.position;
                    newPosi.y += Time.deltaTime * 1.5f;
                    transform.position = newPosi;

                    yield return null;
                }
            }

            transform.position = new Vector3(0, -1000, 0);

            // Resume animation
            TutorialManager.tutorialManager.catSucker.GetComponent<LinearObjectMovement>().pause = false;

            // Wait for sucker move up animation
            while (!TutorialManager.tutorialManager.catSucker.GetComponent<LinearObjectMovement>().animationFinished)
            {
                yield return null;
            }

            TutorialManager.tutorialManager.catSucker.GetComponent<LinearObjectMovement>().animationFinished = false;
            MoveCatBackToContainer.droppedCats.Remove(this);

            yield return new WaitForSeconds(2);

            // Stop the cat's movement
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // Move cat to drop position
            transform.position = catReappearPosition;
            transform.rotation = catReappearRotation;

            // Enable the player to grab the cat after the cat is returned
            GetComponent<VRTK_InteractableObject>().isGrabbable = true;
        }
        else
        {
            yield return new WaitForSeconds(1);

            // Stop the cat's movement
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            // Move cat to drop position
            transform.position = catReappearPosition;
            transform.rotation = catReappearRotation;
        }

        catDropTime = Time.time; // Reset cat drop time
        putBackCatCoroutine = null;
    }
}
