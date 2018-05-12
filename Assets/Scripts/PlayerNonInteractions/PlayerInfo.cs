using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PlayerInfo : MonoBehaviour
{
    public int continuousShotToDisarm; // How many shot the player need to take continuously to drop the weapon
    public float shotEffectTime; // How much time each shot will last
    public float invulnerableTimeAfterDisarm; // How much time the player will be invulnerable to shots after he drops the weapon
    public ScreenOverlay flashBlind; // The screen overlay component for the flash blind
    public float flashWearOutSpeed; // How fast the flash blind fade away

    public List<float> effectiveShots; // Currently effective shot
    public float lastDisarmTime; // When is the last time the player drops the weapon
    public bool isInvulnerable; // If the player is invulnerable

    // Use this for initialization
    void Start()
    {
        flashWearOutSpeed *= 5;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (float thisShotTime in effectiveShots) // Remove the shot that past its effective time
        {
            if (Time.time - thisShotTime >= shotEffectTime)
            {
                effectiveShots.Remove(thisShotTime);
            }
        }

        if (isInvulnerable && Time.time - lastDisarmTime >= invulnerableTimeAfterDisarm)
        {
            isInvulnerable = false;
        }

        if (flashBlind.intensity > 5) // Prevent the blind exceed 100%
        {
            flashBlind.intensity = 5;
        }
        else if (flashBlind.intensity > 0) // Continuously fade out blind
        {
            flashBlind.intensity -= flashWearOutSpeed * Time.deltaTime;
        }
        else if (flashBlind.intensity < 0)
        {
            flashBlind.intensity = 0;
        }
    }
}
