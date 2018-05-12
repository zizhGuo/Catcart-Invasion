using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerDefaultEnemyBehavior : MonoBehaviour
{
    public GameObject playerSideEnemyPrefab; // The prefab for the enemy on the player side

    public GameObject playerSideEnemy; // The copy of this enemy on the player side

    // Use this for initialization
    void Start()
    {
        playerSideEnemy = Instantiate(playerSideEnemyPrefab);
        playerSideEnemy.GetComponent<PlayerSideMirror>().nonPlayerSideCopy = gameObject;
        playerSideEnemy.GetComponent<PlayerSideMirror>().doMirror = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        Destroy(playerSideEnemy);
        MirrorGameManager.score += 1;
    }
}
