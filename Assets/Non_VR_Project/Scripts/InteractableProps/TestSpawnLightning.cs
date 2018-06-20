using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;

public class TestSpawnLightning : MonoBehaviour {

    [SerializeField] float _timeDuration = 1f;
    [SerializeField] float _currentTime;
    public Transform StartPoint;
    public Transform EndPoint;

    [SerializeField] GameObject lighting_Prefab;
    // Use this for initialization
	void Start () {
        _currentTime = Time.time;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && Time.time - _currentTime > 1)
        {
            _currentTime = Time.time;
            var _lightning = Instantiate(lighting_Prefab);
            var controller = _lightning.GetComponent<LightningBoltPrefabScript>();
            controller.Source = StartPoint.gameObject;
            controller.Destination = EndPoint.gameObject;

        }
	}
}
