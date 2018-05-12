using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDestroyAfterTraversal : MonoBehaviour
{

    public List<GameObject> destroyList = new List<GameObject>();

    public static TestDestroyAfterTraversal listManager;

    // Use this for initialization
    void Start()
    {
        listManager = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroyLeftoverdrones()
    {
        foreach (GameObject drone in destroyList)        //Here, we only destoy the drones in the list. On destroy, each drone removes itself from the list, using RemoveDestroyedDrone
        {
            //destroyList.Remove(drone);
            Destroy(drone);
        }
    }

    public void RemoveDestroyedDrone(GameObject drone)
    {
        if (destroyList.Contains(drone))
        {
            destroyList.Remove(drone);
        }
    }
}