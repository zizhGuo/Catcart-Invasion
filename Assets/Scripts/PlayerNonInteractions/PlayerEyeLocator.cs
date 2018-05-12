using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyeLocator : MonoBehaviour
{
    /// <summary>
    /// This script is only used for other game object to locate the object that this script is attached to,
    /// which is player's eyes actual position.
    /// </summary>

    public static GameObject thisGameObject;

    // Use this for initialization
    void Start()
    {
        thisGameObject = gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
