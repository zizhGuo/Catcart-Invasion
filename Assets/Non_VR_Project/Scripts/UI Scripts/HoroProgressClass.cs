using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoroProgressClass : MonoBehaviour
{

    // AUTHOR: Zizhun Guo
    // UI fileds of a horo loading bar
    [SerializeField] private Transform LoadingBar;
    [SerializeField] private Transform TextIndicator;
    [SerializeField] private float currentAmount;
    [SerializeField] private float speed;
    [SerializeField] private Color FillingColor;

    [SerializeField] private Text DebugText;

    private int threshhold;
    private int value;
    // Update is called once per frame
    void Update()
    {
        SetProgressBar();
    }
    public void SetHoroThreshold(int threshhold)
    {
        this.threshhold = threshhold;
    }
    public void SetHoroValue(int value)
    {
        this.value = value;
    }

    private void SetProgressBar()
    {
        DebugText.text = "Threshold = " + threshhold + " Value = " + value;
        if (threshhold == null && value == null) return;
        //Center.GetComponent<Image>().color = FillingColor;
        TextIndicator.GetComponent<Text>().text = value.ToString();
        float ratio = (float)value / threshhold;
        LoadingBar.GetComponent<Image>().fillAmount = ratio;
        print("Current spawn points: " + value);
        print("Current ratio: " + ratio);
    }

}
