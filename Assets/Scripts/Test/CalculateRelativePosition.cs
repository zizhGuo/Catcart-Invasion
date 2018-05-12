using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateRelativePosition : MonoBehaviour
{
    public GameObject child;

    public Vector3 childLocalPosi;
    public float child2ForwardAngle;
    public float childDistMagnitude;
    public Vector3 calculatedChildPosi;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        childLocalPosi = child.transform.localPosition;
        child2ForwardAngle = Vector3.Angle(Vector3.forward, -childLocalPosi);
        if(Vector3.Angle(Vector3.right, -childLocalPosi) >= 90)
        {
            child2ForwardAngle = 360 - child2ForwardAngle;
        }
        calculatedChildPosi.x = -childLocalPosi.magnitude * Mathf.Sin(child2ForwardAngle * Mathf.Deg2Rad);
        calculatedChildPosi.z = -childLocalPosi.magnitude * Mathf.Cos(child2ForwardAngle * Mathf.Deg2Rad);
    }
}
