using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger5 : TutorialTriggerModel
{


    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialManager.tutorialProgress != 4)
        {
            return;
        }

        // If the player picked up the weapon with either left or right hand
        if (GameManager.sLeftController.GetComponent<HandPickItems>().currentItemName == "PlayerWeapon" ||
            GameManager.sRightController.GetComponent<HandPickItems>().currentItemName == "PlayerWeapon")
        {
            gameObject.SetActive(false);
        }

        //print(GameManager.gameManager.playerKart.transform.TransformDirection(new Vector3(0, -90, 0)));
        //print("inverse: " + GameManager.gameManager.playerKart.transform.InverseTransformDirection(new Vector3(0, -90, 0)));
    }
}
