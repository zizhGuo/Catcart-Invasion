using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TutorialEvent7 : TutorialEventModel
{
    public GameObject bullsEye; // The button the player need to press to exit the tutorial
    public Collider bullsEyeCollider; // The collider of the bullseye

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (bullsEye.GetComponent<RotateObject>().pause)
        {
            bullsEyeCollider.enabled = true;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(bullsEye.GetComponent<RotateObject>().Animate());
        TutorialManager.tutorialText.text = "";
        GameManager.sLeftController.GetComponent<VRTK_ControllerActions>().ToggleHighlightButtonTwo(true, Color.clear, 0.5f);
        GameManager.sRightController.GetComponent<VRTK_ControllerActions>().ToggleHighlightButtonTwo(true, Color.clear, 0.5f);
        TutorialManager.tutorialWrap.SetActive(false);
    }
}
