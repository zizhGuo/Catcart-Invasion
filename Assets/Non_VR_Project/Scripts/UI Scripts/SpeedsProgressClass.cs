using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedsProgressClass : MonoBehaviour {


    [SerializeField] private float currentAmount = 0;

    public void SetCurrentSpeed(float currentAmount)
    {
        this.currentAmount = currentAmount;
        //Debug.Log
    }

    void Start()
    {

    }


    void Update()
    {
        //Debug.Log("EnergyBar Value track: " + currentAmount);
        gameObject.GetComponent<Image>().fillAmount = currentAmount / 60;
    }
}
