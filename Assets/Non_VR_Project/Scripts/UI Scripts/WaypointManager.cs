using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages waypoints to show the distance.
public class WaypointManager : MonoBehaviour {

	//TODO set public into private

	[SerializeField] private List<Waypoint> WaypointList = new List<Waypoint> ();
	private Waypoint LastVisitedWaypoint;
	private Waypoint NextVisitedWaypoint;
	private int distance;
    private float iconDistance;
	[SerializeField] private Text DistanceText;
	[SerializeField] private Slider DistanceSlider;
    [SerializeField] private Image startCheck;
    [SerializeField] private Image endingCheck;
    [SerializeField] private Image catkart;

    // Use this for initialization
    void Start () {
		LastVisitedWaypoint = WaypointList.ElementAt (0);
		NextVisitedWaypoint = WaypointList.ElementAt (1);
		distance = LastVisitedWaypoint.getValueOfPercentage ();
        //if (startCheck != null && endingCheck != null) iconDistance = SetCheckpointIconDistance(endingCheck, startCheck);
    }
	
	// Update is called once per frame
	void Update () {
        //Update the Catkart Icon's position through each frame.
        iconDistance = SetCheckpointIconDistance(endingCheck, startCheck);
        //Debug.Log("The icon's distance is : " + iconDistance);
        Transform catkartTrans = catkart.GetComponent<RectTransform>().transform;
        catkartTrans.position = new Vector3(startCheck.GetComponent<RectTransform>().transform.position.x + (float)distance/100* iconDistance, catkartTrans.position.y, catkartTrans.position.z);
        
        
        // Check distance and sets UI.
        checkDistance ();
		setUI ();
	}

	// Sets last visited waypoint & sets the next waypoint as well. see if statement.
	public void setLastVisitedWaypoint(Waypoint lastWaypoint)
	{
		this.LastVisitedWaypoint = lastWaypoint;
		int index = WaypointList.IndexOf (lastWaypoint);
		if (index < WaypointList.Count)
			this.NextVisitedWaypoint = WaypointList.ElementAt (index + 1);
	}

	// Sets Next visited waypoint.
	//public void setNextVisitedWaypoint(Waypoint nextWaypoint)
	//{
	//	this.NextVisitedWaypoint = nextWaypoint;
	//}

	// checks distance.
	void checkDistance()
	{
		// If both are equal. Happens when we are at last checkpoint.
		if (LastVisitedWaypoint == NextVisitedWaypoint) {
			distance = LastVisitedWaypoint.getValueOfPercentage ();
			return;
		}

		// check distance and interpolate it between the teo waypoints.
		float totalDistance = Vector3.Distance (LastVisitedWaypoint.transform.position, NextVisitedWaypoint.transform.position);
		float coveredDistance = Vector3.Distance (LastVisitedWaypoint.transform.position, GameObject.FindGameObjectWithTag ("TempCatkartPosition").transform.position);
		distance =this.LastVisitedWaypoint.getValueOfPercentage () + 
			(int)((this.NextVisitedWaypoint.getValueOfPercentage () - this.LastVisitedWaypoint.getValueOfPercentage ()) * (coveredDistance / totalDistance));
		

	}

	// Sets UI and interpolate between two colors.
	void setUI()
	{
		DistanceText.text = distance.ToString () + " %";
		DistanceSlider.value =  (float)(distance) / WaypointList.LastOrDefault ().getValueOfPercentage ();
		DistanceSlider.GetComponentsInChildren<Image> ().FirstOrDefault (t => t.name == "Fill").color = Color.Lerp (Color.yellow, Color.red, DistanceSlider.value);
	}
    // Get current Distance Percentage. 
    public int getDistancePercentage()
    {
        return distance;
    }

    float SetCheckpointIconDistance(Image A, Image B)
    {
        return Mathf.Abs(Mathf.Abs(A.GetComponent<RectTransform>().position.x) - Mathf.Abs(B.GetComponent<RectTransform>().position.x));
    }
    void setCatkartPos() {

    }

}
