/**
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
**/

using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

[RequireComponent(typeof(Camera))] //remove this line if you dont want to be attached to a camera i.e. in the case of Cardbaord
public class FOVLimiter : MonoBehaviour
{

    public float maxCartSpeed;

    public float maxBlockedFov = .8f;
    public float CRate = .1f;

    public float RateCutOff = .5f;

    private VignetteAndChromaticAberration fovLimiter;

    //private Vector3 currVelocity;
    //private Vector3 oldPos;

    // Test
    public float maxAcc;
    public float maxTurn;

    // Use this for initialization
    void Start()
    {
        //oldPos = transform.position;

        fovLimiter = GetComponent<VignetteAndChromaticAberration>();
        if (fovLimiter == null)
            fovLimiter = gameObject.AddComponent<VignetteAndChromaticAberration>(); //create if we dont have it

        //MaxSpeed = GameManager.sSpeedMultiplier * GameManager.kartMovementInfo.maxAcceleration;
    }

    // Update is called once per frame
    void Update()
    {
        //currVelocity = (transform.position - oldPos) / Time.deltaTime; // velocity = position / time
        //oldPos = transform.position;

        float expectedLimit = maxBlockedFov;
        if (GameManager.kartMovementInfo.currentSpeed < maxCartSpeed) //only update the MaxFOV if we are slower than the MaxSpeed
        {
            expectedLimit = Mathf.Clamp01(GameManager.kartMovementInfo.acceleration / 20f);
        }
        
        expectedLimit = 
            Mathf.Clamp01(expectedLimit + Mathf.Clamp01(GameManager.kartMovementInfo.rotatingSpeed / 20f)) * maxBlockedFov;

        // Test
        if (GameManager.kartMovementInfo.acceleration > maxAcc)
        {
            maxAcc = GameManager.kartMovementInfo.acceleration;
        }
        if (GameManager.kartMovementInfo.rotatingSpeed > maxTurn)
        {
            maxTurn = GameManager.kartMovementInfo.rotatingSpeed;
        }
        // End test

        //print(GameManager.kartLastAcceleration.magnitude + " / " + MaxSpeed + " = " +
        //      GameManager.kartLastAcceleration.magnitude / MaxSpeed + " * " + MaxFOV + " = " +
        //      (GameManager.kartLastAcceleration.magnitude / MaxSpeed) * MaxFOV);

        float currLimit = fovLimiter.intensity;
        float rate = CRate;

        if (currLimit < RateCutOff)
        {
            rate *= 3; //fast rate since the field of view is large and fast changes are less noticeable
        }
        else
        {
            rate *= .5f; //slower rate since the field of view changes are more noticable for larger values. 
        }

        fovLimiter.intensity = Mathf.Lerp(currLimit, expectedLimit, rate);
    }
}
