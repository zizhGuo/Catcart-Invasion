using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger7 : TutorialTriggerModel
{
    public TutorialLine[] tutorialLines; // 16. 17

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 6)
        {
            return;
        }

        if (GameManager.gameManager.catBasket.GetComponent<PlayerKartBasket>().catCount == 0)
        {
            // If the tutorial line 15 finished
            if (GameManager.gameManager.catCartVoiceOver.clip == TutorialManager.tutorialManager.tutorialEvents[5].GetComponent<TutorialEvent6>().tutorialLine.line &&
                !GameManager.gameManager.catCartVoiceOver.isPlaying &&
                !tutorialLines[0].played && TutorialManager.tutorialProgress == 6)
            {
                tutorialLines[0].played = true;
                TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
                TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[0].waitTime, tutorialLines[0].line);
            }
        }

        // If the player has put the first cats in the container
        if (GameManager.gameManager.catBasket.GetComponent<PlayerKartBasket>().catCount > 0)
        {
            TutorialManager.tutorialText.text = "";

            // If the tutorial line 16 finished
            if (GameManager.gameManager.catCartVoiceOver.clip == tutorialLines[0].line &&
                !GameManager.gameManager.catCartVoiceOver.isPlaying &&
                !tutorialLines[1].played && TutorialManager.tutorialProgress == 6)
            {
                tutorialLines[1].played = true;
                TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
                TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[1].waitTime, tutorialLines[1].line);
            }
        }

        // If the player has put all the cats in the container
        if (GameManager.gameManager.catBasket.GetComponent<PlayerKartBasket>().catCount == 9)
        {
            gameObject.SetActive(false);
        }
    }
}
