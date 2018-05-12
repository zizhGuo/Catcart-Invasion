using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls what is displayed on the debug text
/// </summary>
public class DebugTextController : MonoBehaviour
{


    public Text debugText;

    // Use this for initialization
    void Start()
    {
        debugText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = PlayerPrefs.GetFloat("TriggerClickTime").ToString();
    }
}
