using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfButtonPressed : MonoBehaviour
{
    public bool isToggle; // If the button is used for toggle
    public GameObject testingIndicator; // A gameobject that will be set active if the button is pressed down (just for testing in VR)
    public bool testing; // If the button is being tested

    public bool buttonDown; // If the button is pressed down, then this will be true
    public bool toggleOn; // If the button is toggled on

    // Use this for initialization
    void Start()
    {
        buttonDown = false;
        toggleOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (testing)
        {
            if (buttonDown)
            {
                testingIndicator.SetActive(true);
            }
            else
            {
                testingIndicator.SetActive(false);

            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Button")
        {
            buttonDown = true;

            if (isToggle) // If the button is used for toggle
            {
                if (toggleOn) // If the toggle is currently on
                {
                    toggleOn = false;
                }
                else
                {
                    toggleOn = true;
                }
            }

            GetComponent<AudioSource>().Play(); // Play the button click sound effect
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.name == "Button")
        {
            buttonDown = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.name == "Button")
        {
            buttonDown = false;
        }
    }
}
