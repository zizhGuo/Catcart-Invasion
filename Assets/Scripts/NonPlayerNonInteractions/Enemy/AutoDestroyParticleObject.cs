using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class AutoDestroyParticleObject : MonoBehaviour
{
    public AudioSource explosionSound;
    public AudioClip[] explosionClips;

    public float effectDuration;

	// Use this for initialization
	void Start ()
    {
        //explosionSound.PlayOneShot(explosionClips[(betterRandom(0, explosionClips.Length - 1))]);
        for (int i = 0; i < explosionClips.Length; i++)
        {
            explosionSound.pitch = (betterRandom(900, 1150) / 1000f);
            explosionSound.PlayOneShot(explosionClips[i]);
        }

        effectDuration = GetComponent<ParticleSystem>().main.duration;
        Destroy(gameObject, effectDuration);
	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    #region Better random number generator 

    private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

    public static int betterRandom(int minimumValue, int maximumValue)
    {
        byte[] randomNumber = new byte[1];

        _generator.GetBytes(randomNumber);

        double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

        // We are using Math.Max, and substracting 0.00000000001,  
        // to ensure "multiplier" will always be between 0.0 and .99999999999 
        // Otherwise, it's possible for it to be "1", which causes problems in our rounding. 
        double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

        // We need to add one to the range, to allow for the rounding done with Math.Floor 
        int range = maximumValue - minimumValue + 1;

        double randomValueInRange = Math.Floor(multiplier * range);

        return (int)(minimumValue + randomValueInRange);
    }
    #endregion
}
