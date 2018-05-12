using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseBow : MonoBehaviour
{
    public GameObject bowFrame; // The frame of the bow

    // Use this for initialization
    void Start()
    {
        bowFrame = transform.parent.Find("BowFrameWrap").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "BowButton")
        {
            if (!bowFrame.activeInHierarchy)
            {
                bowFrame.SetActive(true);
            }

            else if (bowFrame.activeInHierarchy)
            {
                bowFrame.SetActive(false);
            }
        }
    }
}
