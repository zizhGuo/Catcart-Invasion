using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A general waypoint class.
public class Waypoint : MonoBehaviour {

	[SerializeField]
	private int valueOfPercentage;
	[SerializeField]
	private WaypointManager waypointManager;

	// If Gameobject is the temp Object.
	void OnTriggerEnter( Collider other)
	{
		if (other.gameObject.tag == "TempCatkartPosition") 
		{
			// Sets last active waypoint.
			waypointManager.setLastVisitedWaypoint (this.gameObject.GetComponent<Waypoint> ());
		}	
	}

	// Returns the percentage of a waypoint.
	public int getValueOfPercentage()
	{
		return valueOfPercentage;
	}

}
