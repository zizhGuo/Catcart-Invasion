using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class Rock : MonoBehaviour
{
    public float minSize;
    public float maxSize;
    public float minGreyScale;
    public float maxGreyScale;
    
    public GameObject parentGround;
    public PlayAreaFollowKart playArea;

    // Use this for initialization
    void Start()
    {

        Vector3 newScale = new Vector3(betterRandom(Mathf.RoundToInt(minSize * 100f), Mathf.RoundToInt(maxSize * 100f)) / 100f,
                                       betterRandom(Mathf.RoundToInt(minSize * 100f), Mathf.RoundToInt(maxSize * 100f)) / 100f,
                                       betterRandom(Mathf.RoundToInt(minSize * 100f), Mathf.RoundToInt(maxSize * 100f)) / 100f);

        transform.localScale = newScale;
        //transform.parent = parentGround.transform;
        playArea = FindObjectOfType<PlayAreaFollowKart>();
    }

    // Update is called once per frame
    void Update()
    {
        if(parentGround == null)
        {
            Destroy(gameObject);
        }
    }

    //void OnCollisionEnter(Collision col)
    //{
    //    if (col.collider.tag == "Player")
    //    {
    //        playArea.hitObstacle();
    //    }
    //}

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
