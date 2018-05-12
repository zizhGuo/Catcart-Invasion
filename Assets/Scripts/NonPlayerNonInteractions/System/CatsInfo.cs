using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatsInfo : MonoBehaviour
{
    public CatInfo[] catsInfo; // The information about each cat

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

/// <summary>
/// Stores information about a cat, its color, name, etc
/// </summary>
[Serializable]
public class CatInfo
{
    public Vector3 catLocalPosition; // The cat's local position relative to the cart
    public string catName; // The name of the cat
    public Material catMat; // The material used for the cat
    public bool isCaught; // Has the cat been caught by the catcher
    public bool isSelected; // Has the cat been selected by the catcher (so another catcher won't select the same cat
}