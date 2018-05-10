using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Used to update the score
public class ScoreUpdate : MonoBehaviour {

	[SerializeField] private Text ScoreValue;
	private int Score;

	void Start()
	{
		ScoreValue.text = "0";
	}

	void Update()
	{
		ScoreValue.text = Score.ToString();
	}

	public void setScore(int scoreValue)
	{
		this.Score = scoreValue;
	}


}
