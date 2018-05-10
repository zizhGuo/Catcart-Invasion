using UnityEngine;
using System;

public class GetColor_Particle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ParticleSystem.MainModule ps = this.gameObject.GetComponent<ParticleSystem>().main;
        ps.startColor = this.transform.root.gameObject.GetComponent<BeamParam>().BeamColor;
        ParticleSystem.MinMaxCurve startSize = ps.startSize;
        startSize.constant *= this.transform.root.gameObject.GetComponent<BeamParam>().Scale;
        ps.startSize = startSize;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
