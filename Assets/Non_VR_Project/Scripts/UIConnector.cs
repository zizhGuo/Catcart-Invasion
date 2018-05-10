using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConnector : MonoBehaviour
{

    // Use this for initialization
    [SerializeField] private List<float> Distances;

    [SerializeField] private List<Transform> newKitties;
    [SerializeField] private Transform VRplayer;
    [SerializeField] private RectTransform vrPlayerRectPos;
    [SerializeField] private Transform Tasershooter;
    [SerializeField] private Canvas currentCanvas;
    [SerializeField] private Camera currentCamera;
    [SerializeField] RectTransform CanvasRect;
    [SerializeField] private LineRenderer line1;
    [SerializeField] private RectTransform line2;
    [SerializeField] private RectTransform Connector1;


    void Start()
    {
        Distances = new List<float>();
        CanvasRect = currentCanvas.GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("VR find? " + VRplayer);
        //Debug.Log("Canvas Rect1: " + CanvasRect);
        //line2.sizeDelta = new Vector2(800, 1f);
        if (VRplayer != null)
        {
            Vector2 ViewportPosition = currentCamera.WorldToViewportPoint(VRplayer.transform.position);
            Vector2 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
            vrPlayerRectPos.anchoredPosition = new Vector3 (WorldObject_ScreenPosition.x, WorldObject_ScreenPosition.y, 0);

            //line1.SetPosition(vrPlayerRectPos.anchoredPosition, Tasershooter.anchoredPosition);


           
            Vector2 differenceVector = Connector1.anchoredPosition - vrPlayerRectPos.anchoredPosition;
            
            line2.sizeDelta = new Vector2(differenceVector.magnitude, 5.0f);
            line2.pivot = new Vector2(0, 0.5f);
            line2.position = vrPlayerRectPos.anchoredPosition;
            float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
            line2.rotation = Quaternion.Euler(0, 0, angle);


            //Debug.Log("VR player RectTransform Position: " + vrPlayerRectPos.anchoredPosition.x + " "+ vrPlayerRectPos.anchoredPosition.y);

            //for (int i = 0; i < 9; i++)
            //{
            //    Distances[i] = Vector3.Distance(newKitties[i].GetComponent<RectTransform>().position, VRplayer.position);
            //}
            //for (int i = 0; i < 9; i++)
            //{
            //    Debug.Log("The distances between kitties and VRplayer is " + Distances[i]);
            //}
        }
        

    }

    public void SetVRplayerTransform(Transform VRplayer)
    {
        this.VRplayer = VRplayer;
    }

    public void SetHealthBarKittiesTransform(List<Transform> newKitties)
    {
        this.newKitties = newKitties;
    }
}
