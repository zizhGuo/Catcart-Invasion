using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallEnemyDie : MonoBehaviour
{
    public GameObject catToBe; // The cat the enemy will turnes into when it is killed
    public float maxCatSpeed; // The maximum speed that the cat will jump out
    public float minCatSpeed; // The minimum speed that the cat will jump out
    public float maxCatAngle; // The maximum angle between player's current left/right direction that the cat will jump out (The cat will jump to the side but with a bit of randomed angle)
    public GameObject explosionParticle;
    public int score; // Score rewarded to the player when the enemy die
    public Vector3 scoreOffset; // Where the score should be shown relate to the UI's position
    public float scoreLastingTime; // How long the score will last before disappear
    public float distanceToDestroyIfTooFar; // If the enemy is left behind too far away it should destroy itself

    public GameObject playerKart;
    public Vector3 newJumpDirection; // The angle the cat jumps out
    public PlayerUI playerUI; // The player's HUD UI
    public GameObject enemyUIOnHUD; // The UI this enemy has on player's HUD
    public bool destroyedByPlayer; // If the enemy is destroyed by player

    // Use this for initialization
    void Start()
    {
        playerKart = GameManager.gameManager.playerKart;
        playerUI = GameManager.gameManager.playerUI;

        if (!GetComponent<RandomLightningMovement>())
        {
            enemyUIOnHUD = GetComponent<EnemyInfoOnUI>().infoOnHUD;
        }

        //Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerKart.transform.position) >= distanceToDestroyIfTooFar)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        //if(!this.enabled)
        //{
        //    return;
        //}

        TestDestroyAfterTraversal.listManager.RemoveDestroyedDrone(gameObject);                 //Removing this enemy instance from the total list of alive enemies


        if (!destroyedByPlayer) // If the enemy is not destroyed by player
        {
            return;
        }

        //if (GetComponent<NASCatcherEnemyBehavior>())
        //{
        //    if (!GetComponent<NASCatcherEnemyBehavior>().hasCat && GetComponent<NASCatcherEnemyBehavior>().ca)
        //    {
        //        print("explode disabled");
        //        return;
        //    }
        //}

        //if (GetComponent<NASDrillerEnemyBehavior>())
        //{
        //    if (GetComponentInChildren<ShockBehavior>().exploded && !GetComponentInChildren<ShockBehavior>().isSlapped)
        //    {
        //        return;
        //    }
        //}

        if (GetComponent<DrillerMissileBehavior>())
        {
            Instantiate(GetComponent<DrillerMissileBehavior>().shockAttack.GetComponent<ShockBehavior>().shockParticle, transform.position, transform.rotation);
            return;
        }

        GameObject newCat = Instantiate(catToBe, transform.position, Quaternion.identity); // Create the cat the enemy is turned into
        Vector3 catJumpDirection = Mathf.Sign(BetterRandom.betterRandom(-100, 99)) * transform.InverseTransformDirection(playerKart.transform.right); // Get player kart's current left or right direction
        newJumpDirection.x = BetterRandom.betterRandom(-180, 180);
        newJumpDirection.y = BetterRandom.betterRandom(-180, 180);
        newJumpDirection.z = BetterRandom.betterRandom(-180, 180); // Get a random angle

        while (Vector3.Angle(catJumpDirection, newJumpDirection) >= maxCatAngle) // While the angle between the random angle and the initial angle is larger than the maximum allowed random angle
        {
            newJumpDirection.x = BetterRandom.betterRandom(-180, 180);
            newJumpDirection.y = BetterRandom.betterRandom(-180, 180);
            newJumpDirection.z = BetterRandom.betterRandom(-180, 180);
        }

        newCat.GetComponent<Rigidbody>().AddForce(newJumpDirection.normalized * BetterRandom.betterRandom(Mathf.RoundToInt(minCatSpeed * 1000), Mathf.RoundToInt(maxCatSpeed * 1000)) / 1000f, ForceMode.Impulse);
        Destroy(newCat, 5);

        GameObject newExplosion = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation); //Create explosion particle effect

        //print("create score");
        enemyUIOnHUD.transform.LookAt(GetComponent<EnemyInfoOnUI>().playerEye.transform); // Rotate the info so it looks at the player (if the info has never entered the HUD range before the enemy die, it won't look at the player, so the score's rotation will be wrong)
        GameObject scoreReward = Instantiate(enemyUIOnHUD.GetComponent<EnemyInfoUIelements>().score, enemyUIOnHUD.transform.position + scoreOffset, enemyUIOnHUD.transform.rotation); // Create floating score above died enemy
        scoreReward.transform.LookAt(scoreReward.transform.TransformPoint(-Vector3.forward)); // Adjusting score rotation
        scoreReward.GetComponentInChildren<Text>().text = "+" + score;
        GameManager.gameManager.addScoreProc(scoreLastingTime, score);
        Destroy(scoreReward, scoreLastingTime);
    }
}
