using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger2 : TutorialTriggerModel
{
    public GameObject catCartStartKeyhole; // The trigger that detect if the cat-paw stick is in the CatCart's keyhole
    public LinearObjectMovement catCartMove; // Controls the cat cart moves towards the player
    public TutorialLine[] tutorialLines; // 4, 5, 6

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 1)
        {
            return;
        }

        // If the CatCart is in position
        if (catCartMove.animationFinished && !tutorialLines[0].played)
        {
            TutorialManager.tutorialManager.StopAllPlayLineCoroutines();
            tutorialLines[0].played = true;
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[0].waitTime, tutorialLines[0].line);
            tutorialLines[1].played = true;
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[1].waitTime, tutorialLines[1].line);
            tutorialLines[2].played = true;
            TutorialManager.tutorialManager.StartPlayLineCoroutine(tutorialLines[2].waitTime, tutorialLines[2].line);
        }

        if (!catCartStartKeyhole.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
}
