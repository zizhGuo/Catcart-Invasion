using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TestHapticPulse : MonoBehaviour
{
    /// <summary>
    /// Strength beyond 0.7 is pretty indifferenciable
    /// Interval can be only as short as 0.01, smaller value will extend the pulse duration
    /// </summary>

    public float pulseStrength;
    public float pulseDuration;
    public float pulseInterval;
    public bool playOneShot;
    public bool keepPlay;

    public VRTK_ControllerActions controllerAction; // The controller actions (haptic pulse etc.)

    // Use this for initialization
    void Start()
    {
        controllerAction = GetComponentInChildren<VRTK_ControllerActions>();
    }

    // Update is called once per frame
    void Update()
    {
        if(keepPlay)
        {
            controllerAction.TriggerHapticPulse(pulseStrength, pulseDuration, pulseInterval);
            return;
        }

        if(playOneShot)
        {
            playOneShot = false;
            controllerAction.TriggerHapticPulse(pulseStrength, pulseDuration, pulseInterval);
        }
    }
}
