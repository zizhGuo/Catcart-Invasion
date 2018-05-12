using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerAreaHintAnimation : MonoBehaviour
{
    public float range; // How large the area hint will increase
    public float duration; // How long is each animation cycle

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += Vector3.one * range / duration * Time.deltaTime;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

        if (transform.localScale.x >= range)
        {
            transform.localScale = Vector3.zero;
        }
    }
}
