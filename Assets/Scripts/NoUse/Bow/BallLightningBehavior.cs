using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLightningBehavior : MonoBehaviour
{
    public float energyConsumePerSec; // How much energy the ball lightning is going to use per second
    public float increaseSizeToEnergyRatio; // How much the arrow size is going to increase with the given amount of used energy (size *= energy * ratio)
    public float increaseSpeedToEnergyRatio; // How much the arrow speed is going to increase with the given amount of used energy (size *= energy * ratio)

    public float energy; // How much energy this ball lightning have
    public float energyLeft; // How much energy is left in this ball lightning
    public Vector3 initialSize; // The initial size of the ball lightning

    // Use this for initialization
    void Start()
    {
        energyLeft = energy;
        initialSize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = initialSize * (1f + (energy - energyLeft) * increaseSizeToEnergyRatio);

        if(energyLeft <= 0)
        {
            ballLightningEndProc();
        }
    }

    void FixedUpdate()
    {
        energyLeft -= energyConsumePerSec * Time.fixedUnscaledDeltaTime;
        GetComponent<Rigidbody>().AddForce(transform.right * (energy - energyLeft) * increaseSpeedToEnergyRatio * Time.fixedUnscaledDeltaTime, ForceMode.Impulse);
    }

    public void ballLightningEndProc() // This happenes when the ball lightning used up all its energy and about to disappear
    {
        Destroy(gameObject);
    }
}
