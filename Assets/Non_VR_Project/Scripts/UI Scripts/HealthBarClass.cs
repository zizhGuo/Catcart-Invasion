using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBarClass : MonoBehaviour
{
    [SerializeField] private Transform[] oldKitties;
    //[SerializeField] private List<Transform> oldKitties;
    [SerializeField] public List<Transform> newKitties;
    [SerializeField] private int kittyCount = 9;
    [SerializeField] private List<Vector2> kittyPos;
    [SerializeField] private int catchIndex = 9;
    [SerializeField] private UIConnector uiConnector;

    void Start()
    {
        //oldKitties = new List<Transform>();
        newKitties = new List<Transform>();
        kittyPos = new List<Vector2>();
        newKitties = RandomSort(oldKitties); // Randomlized the order of different kitties.
        SetRectAnchorPos(kittyPos);                     // Set kitty pos in a list         

        for (int i = 0; i < 9; i++)
        {
            newKitties[i].GetComponent<RectTransform>().anchoredPosition = kittyPos[i];
        }

        uiConnector.SetHealthBarKittiesTransform(newKitties);
    }
    void Update()
    {
        switch (kittyCount) {
            case 8:
                newKitties[8].gameObject.SetActive(false);
                break;
            case 7:
                newKitties[7].gameObject.SetActive(false);
                break;
            case 6:
                newKitties[6].gameObject.SetActive(false);
                break;
            case 5:
                newKitties[5].gameObject.SetActive(false);
                break;
            case 4:
                newKitties[4].gameObject.SetActive(false);
                break;
            case 3:
                newKitties[3].gameObject.SetActive(false);
                break;
            case 2:
                newKitties[2].gameObject.SetActive(false);
                break;
            case 1:
                newKitties[1].gameObject.SetActive(false);
                break;
            case 0:
                newKitties[0].gameObject.SetActive(false);
                break;

        }
    }

    // Assign RectTransform AnchoredPosition to the list.
    private void SetRectAnchorPos(List<Vector2> list) {
        for (int i = 0; i < 9; i++)
        {
            list.Add(new Vector2(i * 10 - 40, 0));
        }
    }
    private List<Transform> RandomSort(Transform[] list)
    {
        var random = new System.Random();
        var newList = new List<Transform>();
        foreach (var item in list)
        {
            newList.Insert(random.Next(newList.Count), item);
        }
        return newList;
    }

    // Update the kitties' number.
    public void setKittiesCount(int count)
    {
        kittyCount = count;
    }
    public List<Vector2> GetKittyPos() {
        return kittyPos;
    }


}
