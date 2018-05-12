using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The trigger that will activate the spawn triggers for a group of enemies
/// </summary>
public class NASGroupEnemySpawnTrigger : MonoBehaviour
{
    public float distanceToTrigger; // How close the player has to be to activate this trigger

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Activate all the related enemy triggers when this trigger is activated
        if (Vector3.Distance(GameManager.gameManager.playerKart.transform.position, transform.position) <= distanceToTrigger)
        {
            foreach (NASIndividualEnemySpawner enemySpawner in GetComponentsInChildren<NASIndividualEnemySpawner>(true))
            {
                enemySpawner.gameObject.SetActive(true);
            }
        }
    }
}
