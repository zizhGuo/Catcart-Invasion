using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestDotween : MonoBehaviour {

    [SerializeField] GameObject streetlamp;
    [SerializeField] float currentTime;
    [SerializeField] GameObject pos1;
    [SerializeField] GameObject pos2;
    [SerializeField] GameObject pos3;
    public float rotationSpeed = 2f;
    public float positionSpeed = 5f;
    public float delayTime = 2;


    // Use this for initialization
    void Start () {
        currentTime = Time.time;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time - currentTime > 1) {
            currentTime = Time.time;
            //streetlamp.transform.DOMove(pos.transform.position, 0.5f).SetEase(Ease.InOutQuad);
            streetlamp.transform.DOMove(pos3.transform.position, positionSpeed).SetDelay(delayTime);
            streetlamp.transform.DORotate(new Vector3(pos3.transform.rotation.eulerAngles.x, pos3.transform.rotation.eulerAngles.y,
                pos3.transform.rotation.eulerAngles.z), rotationSpeed);
            //streetlamp.transform.DOMove(pos1.transform.position, 2f).SetDelay(2f);
           //streetlamp.transform.DORotate(new Vector3(pos1.transform.rotation.eulerAngles.x, pos1.transform.rotation.eulerAngles.y,
             //   pos1.transform.rotation.eulerAngles.z), 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            RePos(streetlamp, pos2);
        }
        
	}
    void RePos(GameObject pref, GameObject pos) {
        pref.transform.position = pos.transform.position;
        pref.transform.rotation = pos.transform.rotation;
    }
}
