using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class RandomLightningMovement : MonoBehaviour
{
    /// <summary>
    /// Make the top end of the lightning move randomly
    /// </summary>

    public float randomRange; // The range the end can randomly moves

    public GameObject lightningStart; // The top end of the lightning
    public float randInterval; // The interval between two random movement
    public Vector3 newEndPosition; // The new random position for the top end

    // Use this for initialization
    void Start()
    {
        lightningStart = GetComponent<LightningBoltScript>().StartObject;
        randInterval = GetComponent<LightningBoltScript>().Duration;
        newEndPosition = Vector3.zero;
        newEndPosition.y = lightningStart.transform.localPosition.y;

        StartCoroutine(moveLightning());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator moveLightning()
    {
        while (!GameManager.gameOver)
        {
            newEndPosition.x = BetterRandom.betterRandom(Mathf.RoundToInt(-randomRange * 1000), Mathf.RoundToInt(randomRange * 1000)) / 1000f;
            newEndPosition.z = BetterRandom.betterRandom(Mathf.RoundToInt(-randomRange * 1000), Mathf.RoundToInt(randomRange * 1000)) / 1000f;

            lightningStart.transform.localPosition = newEndPosition;
            //GetComponent<LightningBoltScript>().StartPosition = newEndPosition;

            yield return new WaitForSeconds(randInterval);
        }
    }
}
