using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TutorialEvent6 : TutorialEventModel
{
    public GameObject[] cats; // The cats to be dropped
    public GameObject playerDetector; // The player detector in the cart
    public GameObject containerBullseye; // The bullseye icon on the container
    //public GameObject objectDetector; // The detector that detects if objects is in the CatCart
    public TutorialLine tutorialLine; // 15

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (GetComponent<LinearObjectMovement>().animationFinished && playerWeapon.transform.parent != null)
        //{
        //    playerWeapon.transform.parent = null;
        //}
        // If the cart finish rotating
        //if(GetComponent<RotateObject>().animationFinished && GameManager.gameManager.playerKart.transform.position.y != 0)
        //{
        //    GameManager.gameManager.playerKart.transform.position =
        //        new Vector3(GameManager.gameManager.playerKart.transform.position.x,
        //                    0,
        //                    GameManager.gameManager.playerKart.transform.position.z); // Place the cart at 0 along Y coord
        //    //StartCoroutine(GetComponent<LinearObjectMovement>().Animate()); // Move cart
        //}

        //if (GetComponent<LinearObjectMovement>().animationFinished)
        //{
        //    TutorialManager.tutorialText.text = "Click menu button to drop item";
        //    GameManager.sLeftController.GetComponent<VRTK_ControllerActions>().ToggleHighlightButtonTwo(true, Color.yellow, 0.5f);
        //    GameManager.sRightController.GetComponent<VRTK_ControllerActions>().ToggleHighlightButtonTwo(true, Color.yellow, 0.5f);
        //    gameObject.SetActive(false);
        //}

        // When the last cat is dropped
        if (cats[cats.Length - 1].activeInHierarchy)
        {
            TutorialManager.tutorialText.text = "Click menu button to drop item";
            GameManager.sLeftController.GetComponent<VRTK_ControllerActions>().ToggleHighlightButtonTwo(true, Color.yellow, 0.5f);
            GameManager.sRightController.GetComponent<VRTK_ControllerActions>().ToggleHighlightButtonTwo(true, Color.yellow, 0.5f);
            gameObject.SetActive(false);
        }

        // If the tutorial line 14 finished
        if (GameManager.gameManager.catCartVoiceOver.clip != tutorialLine.line &&
            !GameManager.gameManager.catCartVoiceOver.isPlaying &&
            !tutorialLine.played && TutorialManager.tutorialProgress == 6)
        {
            tutorialLine.played = true;
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLine.waitTime, tutorialLine.line);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DropCats());
        //StartCoroutine(GetComponent<RotateObject>().Animate()); // Rotate cart
        playerDetector.transform.localScale *= 2; // Increase the size of player detector
        containerBullseye.SetActive(false);
        //objectDetector.SetActive(false);
        TutorialManager.tutorialText.text = "";
    }

    /// <summary>
    /// Drops cats one by one
    /// </summary>
    /// <returns></returns>
    public IEnumerator DropCats()
    {
        for (int i = 0; i < cats.Length; i++)
        {
            // Adjust each cat's drop position a little so they won't stack up
            cats[i].transform.position = cats[i].transform.position + cats[i].transform.right * BetterRandom.betterRandom(-100, 100) / 500f
                                                                    + cats[i].transform.forward * BetterRandom.betterRandom(-100, 100) / 500f;
            cats[i].SetActive(true);
            cats[i].GetComponent<Rigidbody>().AddForce(cats[i].transform.forward * BetterRandom.betterRandom(50, 100) / 50f, ForceMode.VelocityChange);
            cats[i].GetComponent<MoveCatBackToContainer>().catDropTime = Time.time;

            yield return new WaitForSeconds(0.2f);
        }
    }
}
