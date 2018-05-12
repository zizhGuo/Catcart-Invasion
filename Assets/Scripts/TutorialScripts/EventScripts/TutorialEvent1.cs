using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent1 : TutorialEventModel
{
    public GameObject catCart; // The CatCart that will be moved to the player when the player press the button with the cat-paw stick
    public float moveSpeed; // The speed the CatCart move towards the player
    public GameObject controlPanel; // The control panel with the cat-paw button on it
    public GameObject buttonFollowee; // The rigidbody collider the control panel button follows
    public TutorialLine tutorialLine;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<LinearObjectMovement>().pause)
        {
            if (!tutorialLine.played)
            {
                tutorialLine.played = true;
                TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
                TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLine.waitTime, tutorialLine.line);
            }

            // If the tutorial line finished
            if (GameManager.gameManager.catCartVoiceOver.clip == tutorialLine.line &&
                !GameManager.gameManager.catCartVoiceOver.isPlaying)
            {
                GameManager.gameManager.catCartVoiceOver.clip = null;
                StartCoroutine(GetComponent<RotateObject>().Animate());
            }
        }

        if (GetComponent<RotateObject>().animationFinished)
        {
            GetComponent<LinearObjectMovement>().pause = false;
        }

        if (GetComponent<LinearObjectMovement>().animationFinished)
        {
            gameObject.SetActive(false);
        }

        if (controlPanel.GetComponent<LinearObjectMovement>().animationFinished)
        {
            Destroy(controlPanel.GetComponentInChildren<SpringJoint>());
            foreach (Collider c in controlPanel.GetComponentsInChildren<Collider>())
            {
                Destroy(c);
            }
        }
    }

    private void OnEnable()
    {
        //print("enable");
        //StartCoroutine(MoveCatCart());

        StartCoroutine(controlPanel.GetComponent<LinearObjectMovement>().Animate());

        //buttonFollowee.SetActive(false);

        StartCoroutine(GetComponent<LinearObjectMovement>().Animate()); // Move out cart
    }

    /// <summary>
    /// The coroutine that moves the CatCart towards the player
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveCatCart()
    {
        Vector3 newPosition; // The position where the CatCart will be for the next frame

        while (catCart.transform.position.y < 0) // Move the CatCart up from the floor
        {
            newPosition = catCart.transform.position;
            newPosition.y += moveSpeed * Time.deltaTime;
            catCart.transform.position = newPosition;
            yield return null;
        }

        newPosition = catCart.transform.position;
        newPosition.y = 0;
        catCart.transform.position = newPosition;

        yield return new WaitForSeconds(1);
        moveSpeed *= 1.5f;

        while (catCart.transform.position.z < -76.67 + 0.35f) // Move the CatCart towards the player
        {
            newPosition = catCart.transform.position;
            newPosition.z += moveSpeed * Time.deltaTime;
            catCart.transform.position = newPosition;
            yield return null;
        }

        this.enabled = false;
    }

    /// <summary>
    /// The coroutine that moves away the control panel to the side
    /// </summary>
    /// <returns></returns>
    //public IEnumerator MoveControlPanel()
    //{
    //    Vector3 newPosition; // The position where the CatCart will be for the next frame

    //    while (controlPanel.transform.position.x > -127.5f) // Move the CatCart up from the floor
    //    {
    //        newPosition = controlPanel.transform.position;
    //        newPosition.x -= moveSpeed * Time.deltaTime * 0.3f;
    //        controlPanel.transform.position = newPosition;
    //        yield return null;
    //    }
    //}
}
