using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script copies position of catKart into a new Object. The new object has a collider which helps in tracking distance.
public class CopyCatkartPosition : MonoBehaviour {

	[SerializeField] private UIManager UIobject;

	// Update is called once per frame
	void Update () {
		this.gameObject.transform.position = UIobject.getPosition ();
	}
}





	