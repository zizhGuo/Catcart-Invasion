using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GroupEnemyDestroyAfterTraversalTrigger : MonoBehaviour
{

    public float distanceToTrigger = 10;            //distance of the playerkart from this trigger

    public bool traversed;                         //Is this trigger already been traversed by the player once
    public Transform playerKart;                   //Reference to the transform of the player's cart
    public Scene currentScene;                     //To get the name of the current Scene

    // Use this for initialization
    void Start()
    {

        traversed = false;                          //setting the trigger to not have been used 

        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "ProgressVersion")
        {
            playerKart = FindObjectOfType<ObjectLocator>().objectToLocate.transform;
        }
        else
        {
            playerKart = GameManager.gameManager.playerKart.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(playerKart.position, transform.position) <= distanceToTrigger)
        {
            if (traversed == false)                                              //destroying the leftover enemies from the list in the static TestDestroyAfterTraversal
            {
                TestDestroyAfterTraversal.listManager.DestroyLeftoverdrones();
                traversed = true;
            }
            else
            {
                Destroy(gameObject);                                            //destroying this trigger when attempted to use again to destroy the drones alive
            }
        }


    }
}
