using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger6 : TutorialTriggerModel
{
    public GameObject catContainer; // The container where the cats will drop into
    public GameObject catContainerTrigger; // The trigger detects if the container has been shot by the player pistol

    public GameManager gameManager; // The game manager

    // Use this for initialization
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 5)
        {
            return;
        }

        // If the player has shot the container
        if (TutorialManager.tutorialProgress == 5 && catContainerTrigger.GetComponent<CatContainerTrigger>().isShot)
        {
            gameObject.SetActive(false);
        }
    }
}
