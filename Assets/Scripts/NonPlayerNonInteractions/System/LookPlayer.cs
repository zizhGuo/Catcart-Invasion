using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPlayer : MonoBehaviour
{
    public GameObject playerHead;

    // Use this for initialization
    void Start()
    {
        playerHead = FindObjectOfType<GameManager>().playerHead;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerHead.transform);
    }
}
