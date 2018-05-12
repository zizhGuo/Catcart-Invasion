using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class HaltonTest : MonoBehaviour
{
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float xOffset;
    public float zOffset;

    HaltonSequence positionsequence = new HaltonSequence();

    void Start()
    {
        //float size = 20.0f * 2;
        //positionsequence.Reset();
        //Vector3 position = Vector3.zero;
        //int amount = 100;
        //for (int i = 0; i < amount; i++)
        //{
        //    positionsequence.Increment();
        //    position = positionsequence.m_CurrentPos;
        //    position.y = 0.0f;
        //    position *= size;
        //    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    sphere.transform.position = position;
        //    //print(position);
        //}

        createRocksOnNewGround(gameObject);
    }

    public void createRocksOnNewGround(GameObject newGround)
    {
        int rockCount = 50;

        HaltonSequence rockHalton = new HaltonSequence();
        float size = 100.0f;
        minX = 0;
        maxX = 0;
        minZ = 0;
        maxZ = 0;
        Vector3[] rockPosi = new Vector3[rockCount * 2];
        bool[] rockPosiOccupy = new bool[rockCount * 2];

        Vector3 position = new Vector3();
        for (int i = 0; i < rockCount * 2; i++)
        {
            rockHalton.Increment();
            position = rockHalton.m_CurrentPos;
            position.y = 0.0f;
            position *= size;
            rockPosi[i] = position;
            rockPosiOccupy[i] = false;

            if (position.x <= minX)
            {
                minX = position.x;
            }
            if (position.x >= maxX)
            {
                maxX = position.x;
            }
            if (position.z <= minZ)
            {
                minZ = position.z;
            }
            if (position.z >= maxZ)
            {
                maxZ = position.z;
            }
        }

        xOffset = maxX - ((maxX - minX) / 2f);
        zOffset = maxZ - ((maxZ - minZ) / 2f);

        for (int i = 0; i < rockCount; i++)
        {
            int rockInd = betterRandom(0, rockPosi.Length - 1);

            while (rockPosiOccupy[rockInd]
                || rockPosi[rockInd].x - xOffset < -49
                || rockPosi[rockInd].x - xOffset > 49
                || rockPosi[rockInd].z - zOffset < -49
                || rockPosi[rockInd].z - zOffset > 49)
            {
                rockInd = betterRandom(0, rockPosi.Length - 1);
            }

            rockPosiOccupy[rockInd] = true;
            Vector3 newRockPosi = rockPosi[rockInd] + newGround.transform.position;
            newRockPosi.x -= xOffset;
            newRockPosi.z -= zOffset;
            
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = newRockPosi;
        }
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
