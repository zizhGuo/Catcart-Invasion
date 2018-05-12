using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newNASTaserShooterEnemyBehavior : MonoBehaviour
{
    public float moveSpeed; // How fast the drone is moving
    public int minDistanceFromPlayer;
    public int maxDistanceFromPlayer;
    public float waitTime;          //the time for which the shooter will stop before moving again to a new location
    public float taserShotMoveSpeed = 10f;         //Speed of the spawned projectile specified by the designer
    public float distanceToEngage; // How close the enemy has to be from the player to start engaging
    public float gazedBehaviorCooldown; // How long before the taser can do another gazed behavior before the last time
    public float minTempTargetDistance; // What's the minimum distance the drone's next moving target has to be with its current position
    public Color normalRingColor; // The color of the ring when the drone is not charging
    public Color chargedRingColor; // The color of the ring when the drone is fully charged
    public MeshRenderer ringMeshRenderer; // The mesh renderer of the ring
    public int chargeNeedForFlash; // How many charges is required to release a flash to the player
    public bool explodeAfterFlash; // Does the enemy self-explode after it flash
    public float energyDrainPerShot; // How much energy does one shot drains from player's weapon
    public float flashBlindPercentage; // How much percent one flash should blind the player

    public GameObject playerKart;
    public GameObject playerHead;
    public MultiShooter taserBeamShooter; // The script that control shooting taser beam
    public LayerMask shootLayer;
    private Vector3 tempTarget = new Vector3(0, 0, 0);       //a temporary target for the drone to move
    public float tempWaitTime;
    private bool isThereRandom;         //to check if the drone has found a new random location relative to the player
    public int movementStatus;         //this is to determine which kind of movement should the drone incorporate
    public NASTaserShooterEnemySpawner spawner;
    public MoveWithPlayerKart moveWithPlayerKart; // The script made it move with player's kart
    public float lastGazedBehaviorTime; // The time of the last time the catcher is doing the behavior when it is gazed by the player
    public float moveSpeedCase2; // The move speed of the taser drone during the switch case 2
    public int energyChargeCount; // How many times does this drone steal energy from the player already?
    public bool shotHit; // Does the shot hits the player
    public GameObject currentShot; // The current laser shot gameobject instantiated by taser enemy
    public AudioSource shootBeamSound; // Audio source that plays the shoot beam sound when shoot laser beam

    // Use this for initialization
    void Start()
    {
        playerKart = GameManager.gameManager.playerKart;        //finding the catKart and storing its gameObject, used to calculate swarm positions
        playerHead = GameManager.gameManager.playerHead;        //finding the player and storing its gameObject, used to shoot projectile
        shootBeamSound = GetComponents<AudioSource>()[1];

        isThereRandom = false;

        tempWaitTime = waitTime;

        //0 = movement after spawning, 1 = Stop for specified time, 2 = movement to a new random location within range
        movementStatus = 0;
        moveWithPlayerKart = GetComponent<MoveWithPlayerKart>();
        moveWithPlayerKart.enabled = false;
        moveSpeedCase2 = moveSpeed;

        flashBlindPercentage /= 20f;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        switch (movementStatus)
        {
            case 0: // When the shooter is not within engage distance
                transform.LookAt(playerKart.transform);

                if (!moveWithPlayerKart.enabled) // If the enemy is far away from player then make it fly faster
                {
                    GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed * 10;
                }
                else
                {
                    GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeed;
                }

                if (((playerKart.transform.position - transform.position).magnitude) <= distanceToEngage)
                {
                    moveWithPlayerKart.enabled = true;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    transform.LookAt(playerHead.transform);
                    movementStatus = 1;
                    lastGazedBehaviorTime = Time.time - gazedBehaviorCooldown * 0.5f;
                }
                break;

            case 1:
                tempWaitTime -= Time.deltaTime;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                moveSpeedCase2 = moveSpeed;

                if (!Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, shootLayer))
                {
                    taserBeamShooter.shoot();
                    ringMeshRenderer.material.color = normalRingColor; // Change the ring color back to normal
                    shootBeamSound.PlayOneShot(shootBeamSound.clip); // Play shoot beam sound

                    tempWaitTime = waitTime;
                    movementStatus = 3;
                }

                if (tempWaitTime <= 0.0f)
                {
                    transform.LookAt(playerHead.transform);
                    taserBeamShooter.shoot();
                    ringMeshRenderer.material.color = normalRingColor; // Change the ring color back to normal
                    shootBeamSound.PlayOneShot(shootBeamSound.clip); // Play shoot beam sound

                    tempWaitTime = waitTime;
                    movementStatus = 3;
                }
                break;

            case 2:
                // Select next moving target position
                if (isThereRandom == false)
                {
                    int whileStopper = 0;

                    do
                    {
                        if (whileStopper > 1000000)
                        {
                            break;
                        }

                        tempTarget.x = BetterRandom.betterRandom(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                        tempTarget.z = BetterRandom.betterRandom(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                        tempTarget.y = 0;

                        whileStopper++;
                    } while (tempTarget.magnitude < minDistanceFromPlayer ||
                             tempTarget.magnitude > maxDistanceFromPlayer ||
                             Vector3.Angle(playerKart.transform.forward, tempTarget) >= 45 ||
                             Vector3.Distance(playerKart.transform.InverseTransformPoint(transform.position), tempTarget) <= minTempTargetDistance);

                    tempTarget.x += playerKart.transform.position.x;
                    tempTarget.z += playerKart.transform.position.z;
                    tempTarget.y = ((BetterRandom.betterRandom(2000, 5000)) / 1000f);

                    tempTarget = playerKart.transform.InverseTransformPoint(tempTarget);
                    isThereRandom = true;
                }

                // Move the drone
                transform.LookAt(playerKart.transform.TransformPoint(tempTarget));
                GetComponent<Rigidbody>().velocity = transform.forward.normalized * moveSpeedCase2;

                transform.LookAt(playerHead.transform); // Look at the player

                if (Vector3.Distance(transform.position, playerKart.transform.TransformPoint(tempTarget)) <= 0.5f)
                {
                    movementStatus = 1;
                    StartCoroutine(ShooterAtkAni()); // Start the enemy attack animation
                    transform.LookAt(playerHead.transform);
                    isThereRandom = false;
                    shotHit = false;
                }

                // If the player is looking at the taser enemy and it can do another gaze behavior
                if (GetComponentInChildren<CheckIfGazed>().isBeingGazed && Time.time - lastGazedBehaviorTime >= gazedBehaviorCooldown)
                {
                    BehaviorWhenGazed();
                    lastGazedBehaviorTime = Time.time;
                }

                break;

            case 3:
                // Wait for the respond of its shot
                if (shotHit) // If the shot hit player
                {
                    if (energyChargeCount == chargeNeedForFlash)
                    {
                        movementStatus = 4;
                        tempWaitTime = waitTime / 2f;
                        energyChargeCount = 0;
                    }
                    else
                    {
                        movementStatus = 2;
                    }
                }
                if (currentShot == null)
                {
                    movementStatus = 2;
                }

                break;

            case 4:
                // Flash attack
                tempWaitTime -= Time.deltaTime;

                if (tempWaitTime <= 0.0f)
                {
                    FlashAttack();
                    tempWaitTime = waitTime;
                }
                break;

            default:
                break;
        }

    }

    /// <summary>
    /// Flash the player
    /// </summary>
    public void FlashAttack()
    {
        GameManager.playerInfo.flashBlind.intensity += flashBlindPercentage;

        if (explodeAfterFlash)
        {
            Destroy(gameObject);
        }
        else
        {
            movementStatus = 2;
        }
    }

    /// <summary>
    /// What the taser shooter enemy will do if it is gazed by the player
    /// </summary>
    public void BehaviorWhenGazed()
    {
        moveSpeedCase2 = moveSpeed * 2.0f; // Double the move speed if the player is looking at it
    }

    public IEnumerator ShooterAtkAni()
    {
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            ringMeshRenderer.material.color = Color.Lerp(normalRingColor, chargedRingColor, t);
            yield return null;
        }

        ringMeshRenderer.material.color = chargedRingColor;
    }

    void OnDestroy()
    {
        //NASTaserShooterEnemySpawner spawner = FindObjectOfType<NASTaserShooterEnemySpawner>();

        //if (spawner != null)
        //{
        //    spawner.totalCountOfEnemies--;
        //}
        //GameObject newExplosion = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation); //Create explosion particle effect.

        //GameManager.score += 1;
    }
}
