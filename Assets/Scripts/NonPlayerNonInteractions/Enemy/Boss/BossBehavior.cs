using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    /// <summary>
    /// Boss move at a speed slower than the player (3/4 of player speed? or a constant speed that is not relate to the player's speed (45?)
    /// When the boss is too close to the player (30?), it will dash back a bit, then resume to it's default speed (7/4 of player speed? or constant 90?)
    /// Player need to slap catcher drones to the Boss to disable its shield (the catcher drone will only temp disable the shield, not slowing the boss)
    /// Player need to slap driller missile to the Boss to slow the boss down to get back the cats that is previously been caught 
    /// (after the boss is slowed, its speed will graduly goes down to zero or close to zero
    ///  after the player get back a cat or after some time the boss will recover and return to normal speed)
    ///  
    /// Boss attack: spawn waves of catcher drones
    ///              shoot waves of driller missile (which will slow player's kart upon impact) (Boss will move at 5/4? (or remained 45?) of player speed when the player is slowed by driller missile
    ///              unleash shockwave-like attack to drop player's weapon
    ///              
    /// Catcher drone should have the same behavior
    /// Missiles and shockwave should move at a speed relative to the boss (so the player can change the cart's relative speed to boss's attacks)
    /// 
    /// Player's cats will be placed outside of the boss's shield, so if the player gets back a cat or drive close to the shield, the boss will suddenly recover from slow speed
    /// If the boss's shield is down, the boss will pause spawning missiles and catcher drone. (It won't pause when its only been slowed)
    /// </summary>

    public GameObject catcherDrone; // The catcher enemy
    public GameObject drillerMissile; // The driller missile
    public GameObject taserWave; // The shock waves that are like taser shot
    public float minDistanceToChargeBack; // How close the boss has to be with the player to start rapidly move back
    public float maxDistanceToStopCharging; // How far the boss has to be with the player to stop rapid move back
    public float minDistanceToSlowDown; // How far the boss has to be away from player to start slow down and let the player catch up
    public float slowForCatchUpVelocityRatio; // What's the ratio between boss and player's distance and boss's speed when it lets the player catch up,
                                              // the further it is from the player, the slower it should move.
    public float defaultSpeed; // The boss's default speed
    public float chargeSpeed; // The boss's speed when fast move back
    public float catPlacementDistance; // How far to put player's cat from it
    public float catPlacementAltitude; // How high to put player's cat from ground
    public float catPlacementSpacing; // How far each cat is away from another when they float in front of the boss
    public Vector3 moveDirection; // Which direction the boss will move towards
    public float shieldDownTime; // How long the shield will be turned off when the boss is hit by a catcher drone
    public GameObject shield; // The shield of the boss
    public float bossAcceleration; // How fast the boss accelerates
    public float playerDistanceToStartFight; // How close the player has to be with the boss to start the boss fight
    public float startAnimationDuration; // The duration of the start animation (where the player won't be able to attack the boss)
    public GameManager gameManager;
    public GameObject cat; // The cat to spawn at the beginning of the boss fight
    public int bossTotalHealth; // Boss's total health

    public float baseCatcherWaveInterval; // The base interval between each wave of catcher drones;
    public int baseCatcherEachWave; // The base number of catcher drones to be spawned each wave;
    public float baseCatcherSpawnInterval; // The base interval between the spawn of each catcher in a single wave;
    public float catchSpawnAltitude; // How high is the catcher spawned
    public int catchSpawnMaxHorizontal; // What's the max horizontal displacement the catcher can be spawned from the boss

    public float baseMissileWaveInterval;
    public int baseMissileEachWave;
    public float baseMissileSpawnInterval;
    public float missileSpawnAltitude; // How high is the missile spawned
    public int missileSpawnMaxHorizontal; // What's the max horizontal displacement the missile can be spawned from the boss
    public float missileForwardRange; // How far the missile should travel relative to its spawn location

    public float baseTaserAtkInterval; // The base interval between each taser attack (not spawn taser drone)
    public int baseTaserWaveEachWave; // How high is the taser spawned
    public int taserWaveSpawnMaxHorizontal; // What's the max horizontal displacement the taser wave can be spawned from the boss

    //public Vector3 bossDeltaPosition; // How much the boss move each frame
    //public bool beginMove; // Does the boss fight begins
    public bool isSlowed; // If the boss is slowed by its own missile
    public bool shieldOn; // If the boss's shield is on
    public bool isChargingBack; // If the boss is rapidly moving back
    public GameObject playerkart; // Player's cart
    public float distanceFromPlayer; // How far is the boss from the player
    public float lastShiledDownTime; // The last time the shield was down
    public float targetSpeed; // What is the boss's targeting speed
    public bool startAni; // If the start animation has began
    public bool started; // If the boss fight actually started (the starting animation finished)
    public int bossHealth; // The boss's health
    public float axisToLock; // Lock boss's position on one axis

    public float lastCatcherWave; // The time of the last wave of catcher drone attack
    public float nextCatcherWaveInterval; // The time wait for next wave of catcher
    public Coroutine catcherSpawnCoroutine; // The coroutine that spawns catcher drones;

    public float lastMissileWave;
    public float nextMissileWaveInterval; // The time wait for next wave of missile
    public Coroutine missileSpawnCoroutine; // The coroutine that spawns driller missiles;

    public float lastTaserWave;
    public float nextTaserWaveInterval; // The time wait for next wave of taser wave


    ///
    /// Temporary boss behavior
    ///
    int layer1;
    int layer2;

    // Use this for initialization
    void Start()
    {
        playerkart = FindObjectOfType<GameManager>().playerKart;
        gameManager = FindObjectOfType<GameManager>();
        isSlowed = false;
        shieldOn = true;
        isChargingBack = false;
        started = false;
        startAni = false;
        bossHealth = bossTotalHealth;
        axisToLock = transform.position.z;
        layer1 = LayerMask.NameToLayer("Boss");
        layer2 = LayerMask.NameToLayer("PlayerSword");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(playerkart.transform.position, transform.position) <= playerDistanceToStartFight && !startAni && !started)
        {
            startAni = true;
            GetComponent<MoveWithPlayerKart>().enabled = true;
            StartCoroutine(waitForStartAni());
            StartCoroutine(startProcess());
        }

        if (startAni)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, axisToLock);
        }

        if (started)
        {
            //moveBoss();

            if (shieldOn) // If the boss haven't been hit by the catcher drone
            {
                if (Time.time - lastCatcherWave >= nextCatcherWaveInterval && bossHealth > 0)
                {
                    catcherSpawnCoroutine = StartCoroutine(spawnCatcher());
                }

                if (Time.time - lastTaserWave >= nextTaserWaveInterval && bossHealth > 0)
                {
                    spawnTaserWave();
                }

                if (Time.time - lastMissileWave >= nextMissileWaveInterval)
                {
                    missileSpawnCoroutine = StartCoroutine(spawnMissile());
                }
            }

            if (!shieldOn && Time.time - lastShiledDownTime >= shieldDownTime)
            {
                turnOnShield();
            }
        }

        ///
        /// Temporary boss behavior
        ///
        if (bossHealth == 0)
        {
            shield.SetActive(false);
            //GetComponentInChildren<BossGetHit>().gameObject.layer = LayerMask.NameToLayer("CanCollidePlayerSword");
            Physics.IgnoreLayerCollision(layer1, layer2, false);

            if (isSlowed && targetSpeed == defaultSpeed)
            {
                if (GetComponent<Rigidbody>().velocity.magnitude > defaultSpeed)
                {
                    targetSpeed = defaultSpeed;
                }
            }
        }
    }

    public IEnumerator startProcess() // Preparing the boss fight (during the start animation)
    {
        Vector3 catSpawnPosi = Vector3.zero; // Where to spawn the cat relative to the boss
        catSpawnPosi.y = catPlacementAltitude;
        catSpawnPosi.x = -catPlacementDistance;

        if (gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().catCount > 5) // Increase the spacing between cats put 
                                                                                             // in front of the boss if there is less cats been caught
        {
            catPlacementSpacing *= 4;
        }
        else if (gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().catCount > 3)
        {
            catPlacementSpacing *= 2;
        }

        for (int i = 0; i < 9 - gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().catCount; i++)
        {

            if (i % 2 == 0)
            {
                catSpawnPosi.z = Mathf.CeilToInt(i / 2f) * catPlacementSpacing;
            }
            else
            {
                catSpawnPosi.z = -Mathf.CeilToInt(i / 2f) * catPlacementSpacing;
            }

            GameObject newCat = Instantiate(cat, transform.position + catSpawnPosi, Quaternion.identity);
            newCat.AddComponent<MoveWithBoss>();

            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator waitForStartAni() // wait for boss's starting animation
    {
        yield return new WaitForSeconds(startAnimationDuration);

        started = true;
        GetComponent<MoveWithPlayerKart>().enabled = false;
        GetComponent<Rigidbody>().velocity = transform.forward.normalized * GameManager.currentSpeed;
        startAni = false;
    }

    public void turnOffShield()
    {
        shieldOn = false;
        shield.SetActive(false);

        lastShiledDownTime = Time.time;
        StopCoroutine(catcherSpawnCoroutine);
        StopCoroutine(missileSpawnCoroutine);

        //yield return new WaitForSeconds(shieldDownTime);
        //shieldOn = true;
        //shield.SetActive(true);
    }

    public void turnOnShield()
    {
        shieldOn = true;
        shield.SetActive(true);
    }

    public IEnumerator spawnMissile()
    {
        lastMissileWave = Time.time;
        nextMissileWaveInterval = baseMissileWaveInterval + BetterRandom.betterRandom(-3, 2) - (bossTotalHealth - bossHealth) * 2;
        Vector3 missileSpawnPosi = Vector3.zero; // Need to change
        missileSpawnPosi.y = missileSpawnAltitude;

        for (int i = 0; i < BetterRandom.betterRandom(baseMissileEachWave, baseMissileEachWave * 2); i++)
        {
            missileSpawnPosi.z = BetterRandom.betterRandom(-missileSpawnMaxHorizontal, missileSpawnMaxHorizontal);
            Instantiate(drillerMissile, transform.position + missileSpawnPosi, Quaternion.identity);

            yield return new WaitForSeconds(BetterRandom.betterRandom(5, 10) / 10f * baseMissileSpawnInterval);

            lastMissileWave = Time.time;
        }
    }

    public IEnumerator spawnCatcher()
    {
        lastCatcherWave = Time.time;
        nextCatcherWaveInterval = baseCatcherWaveInterval + BetterRandom.betterRandom(-3, 2) - (bossTotalHealth - bossHealth) * 2;
        Vector3 catcherSpawnPosi = Vector3.zero;
        catcherSpawnPosi.y = catchSpawnAltitude;
        catcherSpawnPosi.x = -10;

        for (int i = 0; i < BetterRandom.betterRandom(baseCatcherEachWave, baseCatcherEachWave * 2); i++)
        {
            catcherSpawnPosi.z = BetterRandom.betterRandom(-catchSpawnMaxHorizontal, catchSpawnMaxHorizontal);
            Instantiate(catcherDrone, transform.position + catcherSpawnPosi, Quaternion.identity);

            yield return new WaitForSeconds(BetterRandom.betterRandom(5, 10) / 10f * baseCatcherSpawnInterval);

            lastCatcherWave = Time.time;
        }
    }

    public void spawnTaserWave()
    {
        lastTaserWave = Time.time;
        nextTaserWaveInterval = baseTaserAtkInterval + BetterRandom.betterRandom(-3, 2) - (bossTotalHealth - bossHealth) * 2;
        Vector3 taserSpawnPosi = Vector3.zero;
        taserSpawnPosi.y = 0;

        for (int i = 0; i < BetterRandom.betterRandom(baseTaserWaveEachWave, baseTaserWaveEachWave * 2); i++)
        {
            taserSpawnPosi.z = BetterRandom.betterRandom(-taserWaveSpawnMaxHorizontal, taserWaveSpawnMaxHorizontal);
            Instantiate(taserWave, transform.position + taserSpawnPosi, Quaternion.identity);

            lastTaserWave = Time.time;
        }
    }

    //public void moveBoss() // Make the boss move
    //{
    //    distanceFromPlayer = Vector3.Distance(transform.position, playerkart.transform.position);

    //    if (isSlowed) // If the boss is slowed by its own missile
    //    {
    //        if (distanceFromPlayer <= catPlacementDistance - 1) // If the player is too close
    //        {
    //            targetSpeed = chargeSpeed;
    //            chargeBack();

    //            ///
    //            /// Temporary boss behavior
    //            ///
    //            if(bossHealth == 0)
    //            {
    //                targetSpeed = defaultSpeed;
    //            }
    //        }
    //    }
    //    else if (isChargingBack) // If the boss is rapidly charging back
    //    {
    //        if (distanceFromPlayer >= maxDistanceToStopCharging)
    //        {
    //            isChargingBack = false;
    //            targetSpeed = defaultSpeed;

    //            ///
    //            /// Temporary boss behavior
    //            ///
    //            if (bossHealth == 0)
    //            {
    //                isSlowed = false;
    //            }
    //        }
    //    }
    //    else if (distanceFromPlayer <= minDistanceToChargeBack) // If the boss is close to the player
    //    {
    //        targetSpeed = 60.01f;
    //        chargeBack();
    //    }

    //    if (GetComponent<Rigidbody>().velocity.magnitude < targetSpeed)
    //    {
    //        GetComponent<Rigidbody>().AddForce(transform.forward.normalized * bossAcceleration, ForceMode.Acceleration);
    //        //print("should acc, " + GetComponent<Rigidbody>().velocity);
    //    }
    //    else if (GetComponent<Rigidbody>().velocity.magnitude > targetSpeed)
    //    {
    //        GetComponent<Rigidbody>().AddForce(-transform.forward.normalized * bossAcceleration, ForceMode.Acceleration);
    //    }
    //}
    ///
    //public void chargeBack() // let boss charge back
    //{
    //    isSlowed = false;
    //    isChargingBack = true;
    //    //targetSpeed = chargeSpeed;

    //    ///
    //    /// Temporary boss behavior
    //    ///
    //    if (bossHealth == 0 && targetSpeed == defaultSpeed)
    //    {
    //        isSlowed = true;
    //    }
    //}
    ///
    //public IEnumerator returnDefaultSpeed() // return the movement speed to default
    //{
    //    int whileStopper = 0;

    //    while (GetComponent<Rigidbody>().velocity.magnitude > defaultSpeed && whileStopper < 1000000)
    //    {
    //        GetComponent<Rigidbody>().AddForce(-(transform.forward.normalized) * bossAcceleration, ForceMode.Acceleration);
    //        whileStopper++;
    //        yield return null;
    //    }

    //    GetComponent<Rigidbody>().velocity = moveDirection * defaultSpeed;
    //}
}
