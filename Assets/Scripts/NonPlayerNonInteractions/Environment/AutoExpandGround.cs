using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoExpandGround : MonoBehaviour
{
    /// <summary>
    /// This now is actually only used for destroy unused ground.
    /// </summary>
    public GroundExpandTrigger[] expandTriggers;
    
    public int canDestroy;
    public float lastSteppedTime; //The last time that this ground is stepped on by player.
    public float destroyDelay; //If the ground has not been stepped for a period of time, it will be destroyed.
    
    // Use this for initialization
    void Start ()
    {
        canDestroy = 0;

        lastSteppedTime = Time.time;
        destroyDelay = 10f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //canDestroy = 0;

		for(int i = 0; i < expandTriggers.Length; i++)
        {
            if(expandTriggers[i].isStepped)
            {
                //canDestroy++;
                lastSteppedTime = Time.time;
            }
        }

        //if(canDestroy == 0)
        //{
        //}

        if(Time.time - lastSteppedTime > destroyDelay)
        {
            Destroy(gameObject);
        }
	}
}
