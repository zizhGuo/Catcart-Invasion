using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpeedUI : MonoBehaviour
{
    public Image speedImage; //The imaging that represent the kart's current speed

    public KartFollowLaserSpot kartInfo; //To get the current speed of the kart

    // Use this for initialization
    void Start()
    {
        kartInfo = FindObjectOfType<KartFollowLaserSpot>();
    }

    // Update is called once per frame
    void Update()
    {
        speedImage.fillAmount = kartInfo.currentSpeed / kartInfo.maximumSpeed;
    }
}
