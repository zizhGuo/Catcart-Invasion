using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A general Class for making a radial progress bar.
public class RadialProgressClass : MonoBehaviour {

	// UI fields of a Radial Loading Bar
	[SerializeField] private Transform LoadingBar;
	[SerializeField] private Transform Center;
	[SerializeField] private Transform TextIndicator;
    [SerializeField] private Transform IconLock;
    [SerializeField] private Transform Outline;
	[SerializeField] private float currentAmount;
	[SerializeField] private float speed;
	[SerializeField] private Color ReadyColor ;
	[SerializeField] private Color FillingColor ;

	// The Timee related to enemies. Each type of enemy has different cool down time.
	private float CoolDownTimer;
	private float CoolDownThreshold;
	// Used to know if Radial Bar is active or not.
	private bool isActive = true;

	// This method is called from UIManager to set private variable Cool Down Time
	public void SetCoolDownTimer(float time)
	{
		this.CoolDownTimer = time;
		isActive = true;
	}

	// This method is called from UIManager to set private variable Cool Down Threshold.
	public void SetCoolDownThreshold(float time)
	{
		this.CoolDownThreshold = time;
	}

	// If this Radial Bar is active, make it loading else, deactivate the loading bar.
	void Update()
	{
		if (isActive) {
			setRadialProgressBar ();
		} else disactivateRadialBar ();
	}

	// Sets the radial Bar.
	private void setRadialProgressBar()
	{

		// If cool down time is not set, then early return.
		if (CoolDownTimer == null || CoolDownThreshold == null)
			return;

		// Check if time is less than threshold. We need to make it load.
		if (CoolDownTimer < CoolDownThreshold) {
			Center.GetComponent<Image> ().color = FillingColor; 
			currentAmount += speed * Time.deltaTime;
			// Set the Text in percentage accordingly.
			TextIndicator.GetComponent<Text> ().text = ((int)ConvertCoolDownTimerintoPercentage ()).ToString () + "%";
		} else {
			//TextIndicator.GetComponent<Text> ().text = "Ready";
            switch (gameObject.name) {
                case "TaserShooterRadialProgressBar":
                    TextIndicator.GetComponent<Text>().text = "TaserShooter";
                    break;
                case "CatcherRadialProgressBar":
                    TextIndicator.GetComponent<Text>().text = "Catcher";
                    break;
                case "FallingDrillerRadialProgressBar":
                    TextIndicator.GetComponent<Text>().text = "DrillerBomb";
                    break;
                case "DrillerTrapRadialProgressBar":
                    TextIndicator.GetComponent<Text>().text = "DrillerTrap";
                    break;
            }
            Center.GetComponent<Image> ().color = ReadyColor; 
		}
		// The value for filling the amount.
		LoadingBar.GetComponent<Image> ().fillAmount = ConvertCoolDownTimerintoPercentage () / 100;
        Outline.GetComponent<Image>().fillAmount = 1;
    }

	// Convert the cool down timer as a percentage.
	private float ConvertCoolDownTimerintoPercentage()
	{
		if (CoolDownTimer < 0)  return 0;
		else return ((CoolDownTimer / CoolDownThreshold)*100);
	}

	//Deactiavates the radial bar.
	public void disactivateRadialBar()
	{
		isActive = false;
		//TextIndicator.GetComponent<Text> ().text = "X";
        switch (gameObject.name)
        {
            case "TaserShooterRadialProgressBar":
                TextIndicator.GetComponent<Text>().text = "TaserShooter";
                break;
            case "CatcherRadialProgressBar":
                TextIndicator.GetComponent<Text>().text = "Catcher";
                break;
            case "FallingDrillerRadialProgressBar":
                TextIndicator.GetComponent<Text>().text = "DrillerBomb";
                break;
            case "DrillerTrapRadialProgressBar":
                TextIndicator.GetComponent<Text>().text = "DrillerTrap";
                break;
        }
        Center.GetComponent<Image> ().color = FillingColor; 
		LoadingBar.GetComponent<Image> ().fillAmount = 1;
        Outline.GetComponent<Image>().fillAmount = 0;

    }

    public void enableIconLock() {

        IconLock.gameObject.SetActive(true);
    }
    public void disableIconLock() {
        IconLock.gameObject.SetActive(false);
    }
}
