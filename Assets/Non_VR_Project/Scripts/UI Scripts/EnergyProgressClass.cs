using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyProgressClass : MonoBehaviour
{

    [SerializeField] private float currentAmount = 1000;

    public void SetCurrentEnergy(float currentAmount) {
        this.currentAmount = currentAmount;
    }

    void Start()
    {

    }


    void Update()
    {
        //Debug.Log("EnergyBar Value track: " + currentAmount);
        gameObject.GetComponent<Image>().fillAmount = currentAmount / 1000;
    }
}
