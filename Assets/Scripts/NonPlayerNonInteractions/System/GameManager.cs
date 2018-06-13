using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;
using VRTK.GrabAttachMechanics;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public PlayerUI playerUI;
    public GameObject playerHUDviewArea; // The view area of the HUD UI, set this active to make the HUD visible
    public GameObject playerKart; // The player side kart (the one that player actually sees)
    public GameObject nonPlayerKart; // The non-player side kart (the mirrored one)
    public GameObject playerHead;
    public GameObject catBasket;
    public GameObject nonPlayerCatBasket; // The non-player side basket
    //public int minRocks;
    //public int maxRocks;
    //public int maxRockInc;
    //public float playerHealth; // The total hit point for the player
    public float playerTotalWeaponEnergy; // The total energy of player's weapon
    public GameObject leftController; // The left controller for [VRTK]
    public GameObject rightController; // The right controller for [VRTK]
    public float speedMultiplier; // All the (player kart, enemy move speed, pistol bullet speed, etc.) things that have speed (max speed, acceleration, spawn distance) will scale with this speed (some of them may have individual scale multiplied with this scale)
    //public bool pistolBulletIsLaser; // Do way use the old laser shot for pistol or the new homing shot
    public float slapForceMagnifier; // How much do we increase the slap force
    public float maxCollisionForce; // Used to down-scale the calculated collision force with a range for the TriggerHapticPulse() function
    public GameObject goal; // The mark for the position of the goal
    public float distanceAwayFromGoalBeforeAlert; // How far the player has to go away from the goal before there is indication that the player is going the wrong way
    public GameObject networkManager; // The network manager that allows crossplay
    public bool allowMultiplayer; // Do we enable multiplayer for this VR player
    public PlayerCatStayInBasket[] cats; // Player's nine cats
    public bool skipTutorial; // Do we skip tutorial
    public bool isNotMenu; // Is the current scene a menu or not
    public bool alwaysSyncPlayAreaWithCart; // Do we turn off PlayerDetector and always sync up the play area with the cart?
    public AudioSource catCartVoiceOver; // The audio source that plays CatCart lines
    public Transform catCartSFX; // The audio source that plays CatCart sound effects

    public static float sSpeedMultiplier; // All the things that have speed (player kart, enemy move speed, pistol bullet speed, etc.) will scale with this speed (some of them may have individual scale multiplied with this scale)
    //public static int sminRocks;
    //public static int smaxRocks;
    //public static int smaxRockInc;
    public static int score;
    public static bool gameOver;
    public static GameManager gameManager;
    public static KartFollowLaserSpot kartMovementInfo; //The movement information (speed, acceleration, etc.) about the kart
    public static bool gameStart;
    //public WallMove wallOfDeath; //The wall that will chasing player
    public GameObject enemySpawner; //The enemy spawner
    //public CustomControllerEvents playerWeapon; //The weapon the player is using
    public static float gameStartTime;
    public static float currentEnergy; // The amount of energy the player currently has
    public static bool canRechargeEnergy; // Can player weapon recharge energy?
    public static float sPlayerTotalWeaponEnergy;
    public static float currentSpeed; // The current speed of the kart
    public static Vector3 kartLastPosition; // The kart's position in the last fixed update
    public static Vector3 kartDeltaPosition; // The vector for how much the kart moved in last fixed update
    public static Vector3 kartDeltaAcceleration; // The vector for how much the kart's acceleration changed in last fixed update, it will be applied to physics simulations on the non-player side kart
    public static Vector3 kartLastVelocity; // The kart's velocity in the last fixed update
    public static Vector3 kartCurrentVelocity; // The kart's velocity in the last fixed update
    public static Vector3 kartLastAcceleration; // The vector for the kart's acceleration in last fixed update
    //public static bool sPistolBulletIsLaser;
    public static float sSlapForceMagnifier; // The static reference of the slap force
    public static float sMaxCollisionForce;
    public float closestDistanceToGoal; // How close the player has been to the goal (If the current distance between the player and the goal is larger than this then the player is going reverse)
    public float totalDistanceToGoal; // The total distance from the begin to the goal
    public float currentDistance; // The current distance from the player to the goal
    public float lastFurthestDistanceGoingBackward; // The longest distance from the player to the goal the current time the player is going in wrong direction
    public static GameObject sLeftController; // The static reference to the left controller
    public static GameObject sRightController; // The static reference to the right controller
    public int activeCatCount; // The count of active cat gameobject to determine if the player loses all the cats
    public static PlayerInfo playerInfo; // Info that relates to the actual player (i.e. cat count is not player info)
    public static bool gameFinished; // Is the game finished;
    public static CatsInfo catsInfo; // The information of the cats
    public static List<CatInfo> missingCats; // A list for missing cats
    public static float lastTimeNoEnergyVoicePlayed; // The last time that the voice over alert the player's energy stopped recharging is played

    // Test
    public float currentEnergyDebug;
    public Vector3 kartDeltaAccelerationDebug; //
    public Vector3 kartLastAccelerationDebug;
    public KartFollowLaserSpot testKartMovementInfo; // Get some debug info

    // Use this for initialization
    void Start()
    {
        // Skip main gameplay initialization
        if (!isNotMenu)
        {
            return;
        }
        score = 0;
        gameOver = false;
        gameStart = true;        // Copy from workable project.
        allowMultiplayer = true; // add this one for testing.
        //sminRocks = minRocks;
        //smaxRocks = maxRocks;
        //smaxRockInc = maxRockInc;
        gameManager = this;
        //kartMovementInfo = FindObjectOfType<KartFollowLaserSpot>();
        //wallOfDeath = FindObjectOfType<WallMove>();
        //enemySpawner = FindObjectOfType<SimpleSpawnEnemy>().gameObject;
        //playerWeapon = FindObjectOfType<CustomControllerEvents>();
        gameStartTime = 0;
        catBasket = FindObjectOfType<PlayerKartBasket>().gameObject;
        nonPlayerCatBasket = FindObjectOfType<NonPlayerKartBasket>().gameObject;
        sPlayerTotalWeaponEnergy = playerTotalWeaponEnergy;
        sSpeedMultiplier = speedMultiplier;
        currentEnergy = playerTotalWeaponEnergy;
        //sPistolBulletIsLaser = pistolBulletIsLaser;
        sSlapForceMagnifier = slapForceMagnifier;
        sMaxCollisionForce = maxCollisionForce;
        closestDistanceToGoal = Mathf.Infinity;
        totalDistanceToGoal = Vector3.Distance(goal.transform.position, playerKart.transform.position);
        sLeftController = leftController;
        sRightController = rightController;
        playerInfo = FindObjectOfType<PlayerInfo>();
        //print(FindObjectOfType<PlayerInfo>());
        canRechargeEnergy = true;
        gameFinished = false;
        missingCats = new List<CatInfo>();

        ///Setup controller "PlayerCatilizerControl"
        sLeftController.GetComponent<PlayerCatilizerControl>().catBasket = catBasket.transform;
        sRightController.GetComponent<PlayerCatilizerControl>().catBasket = catBasket.transform;

        /// Setup DetectIfPlayerIsInKart
        //print(FindObjectOfType<DetectIfPlayerIsInKart>().transform.parent.name);
        DetectIfPlayerIsInKart.playerPlayArea = FindObjectOfType<PlayAreaFollowKart>();
        DetectIfPlayerIsInKart.playerHead = FindObjectOfType<SteamVR_Camera>();

        //sLeftController.GetComponent<VRTK_ControllerActions>().ToggleControllerModel(false, null);
        //sRightController.GetComponent<VRTK_ControllerActions>().ToggleControllerModel(false, null);
        // Setup color and name for cats
        //catsInfo = FindObjectOfType<CatsInfo>();
        //for (int i = 0; i < cats.Length; i++)                                                                  // delete has no difference. 
        //{
        //    //cats[i].GetComponent<InteractWithCat>().catColorRenderer.material = catsInfo.catsInfo[i].catMat;
        //    cats[i].GetComponent<InteractWithCat>().catName = catsInfo.catsInfo[i].catName;
        //}

        if (alwaysSyncPlayAreaWithCart)
        {
            //FindObjectOfType<DetectIfPlayerIsInKart>().getInKart();
        }

        if (FindObjectOfType<NetworkManager>())
        {
            networkManager = FindObjectOfType<NetworkManager>().gameObject;

            if (!allowMultiplayer)
            {
                networkManager.SetActive(false);
            }
        }
        
        //networkManager = NetworkManager.singleton.gameObject;
        //GameManager.gameStartProc(); // Start Game

    }

    private void LateUpdate()
    {
        // Wait for the level to be loaded
        if (Time.timeSinceLevelLoad <= 1)
        {
            return;
        }

        // Count avtive cats, if it reaches 0 then game over
        activeCatCount = cats.Length;
        //print(cats.Length);
        foreach (PlayerCatStayInBasket cat in cats)
        {
            if (!cat.isActiveAndEnabled)
            {
                activeCatCount--;
            }
        }
        if (gameStart && !gameFinished && activeCatCount <= 0)  //////////////////////////////////////////////////// game has started.
        {
            //print("no cat " + activeCatCount);
            gameLostProc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (sSpeedMultiplier != speedMultiplier)
        //{
        //    sSpeedMultiplier = speedMultiplier;
        //}

        //print(sSpeedMultiplier);

        currentDistance = Vector3.Distance(goal.transform.position, playerKart.transform.position);

        if (currentDistance < closestDistanceToGoal) // Update the closest distance between the player and the goal
        {
            closestDistanceToGoal = Vector3.Distance(goal.transform.position, playerKart.transform.position);
        }

        if (currentDistance >= closestDistanceToGoal + distanceAwayFromGoalBeforeAlert) // If the player is moving away from the goal
        {
            playerUI.startWrongDirAni();

            //if (lastFurthestDistanceGoingBackward >= closestDistanceToGoal) // Update player's furthest distance since this time going reverse
            //{
            //    lastFurthestDistanceGoingBackward = currentDistance;
            //}
            if (currentDistance >= lastFurthestDistanceGoingBackward) // Update player's furthest distance since this time going reverse
            {
                lastFurthestDistanceGoingBackward = currentDistance;
            }

            if (currentDistance <= lastFurthestDistanceGoingBackward - distanceAwayFromGoalBeforeAlert) // Update the closest distance from the last time the player going in reverse
            {
                closestDistanceToGoal = currentDistance;
            }
        }
        else
        {
            lastFurthestDistanceGoingBackward = 0;
            playerUI.stopWrongDirAni();
        }

        // If the player reaches the goal, game over
        if (currentDistance <= 50)
        {
            //gameOverProc();
        }

        if (!gameOver)
        {
            //playerUI.score.text = "Kills : " + score + "\r\n" +
            //                      "Distance : " + playerKart.transform.position.z.ToString("F") + "m" + "\r\n" +
            //                      "Time : " + Time.time.ToString("F") + "s";

            if (playerUI.gameObject.activeInHierarchy)
            {
                playerUI.score.text = "SCORE" + "\r\n" + score.ToString("0000");
                //playerUI.speed.text = "SPEED" + "\r\n" + kartMovementInfo.currentSpeed.ToString("0000");
                playerUI.progression.text = "PROGRESSION" + "\r\n" + Mathf.Round(Mathf.Clamp((totalDistanceToGoal - currentDistance), 0, totalDistanceToGoal) / totalDistanceToGoal * 100) + "%";
                //playerUI.energy.text = "ENERGY" + "\r\n" + currentEnergy.ToString("0000");
                if (GameManager.gameStartTime != 0)
                {
                    playerUI.time.text = "TIME" + "\r\n" + (Time.time - GameManager.gameStartTime).ToString("0000");
                }
                else
                {
                    playerUI.time.text = "TIME" + "\r\n" + "0000";
                }
                playerUI.speedBar.fillAmount = Mathf.Clamp01(kartMovementInfo.currentSpeed / kartMovementInfo.maximumSpeed); // Fill the speed bar
                playerUI.leftHandEnergyBar.fillAmount = Mathf.Clamp01(currentEnergy / playerTotalWeaponEnergy); // Fill the left hand energy bar
                playerUI.rightHandEnergyBar.fillAmount = Mathf.Clamp01(currentEnergy / playerTotalWeaponEnergy); // Fill the right hand energy bar
            }
        }

        // Testing
        testKartMovementInfo = kartMovementInfo;
        currentEnergyDebug = currentEnergy;
        //print(currentEnergy);
    }

    void FixedUpdate()
    {
        //print(currentSpeed);
        currentSpeed = (playerKart.transform.position - kartLastPosition).magnitude / Time.fixedUnscaledDeltaTime;

        kartDeltaAcceleration = (kartCurrentVelocity - ((playerKart.transform.position - kartLastPosition) / Time.fixedUnscaledDeltaTime)) - kartLastAcceleration; // Notice that in here, the kartCurrentVelocity is 
                                                                                                                                                                   //actually the velocity calculated in the last frame, 
                                                                                                                                                                   //since it has not been calculated for this frame yet
        kartDeltaAccelerationDebug = kartDeltaAcceleration * 1000;

        kartCurrentVelocity = (playerKart.transform.position - kartLastPosition) / Time.fixedUnscaledDeltaTime; // The current velocity = (current position - last position) / time spent this frame
        kartLastAcceleration = kartCurrentVelocity - kartLastVelocity; // The velocity for last frame will be calculated in the current frame with the current velocity and last velocity and being used in the next frame

        kartLastAccelerationDebug = kartLastAcceleration;

        kartDeltaPosition = playerKart.transform.position - kartLastPosition;
        kartLastPosition = playerKart.transform.position;

        kartLastVelocity = kartCurrentVelocity;

    }

    public static void gameLostProc()
    {
        // Testing
        //print("game over " + gameOver);
        //

        if (!gameOver)
        {
            gameManager.playerUI.score.text = "GAMEOVER" + "\r\n" + score.ToString("0000");
            gameManager.playerUI.speed.text = "SPEED" + "\r\n" + kartMovementInfo.currentSpeed.ToString("0000");
            //gameManager.playerUI.time.text = "PROGRESSION" + "\r\n" + Mathf.Round(Mathf.Clamp((totalDistanceToGoal - currentDistance), 0, totalDistanceToGoal) / totalDistanceToGoal * 100) + "%";
            gameManager.playerUI.energy.text = "ENERGY" + "\r\n" + "0000";
        }

        gameOver = true;
        //Time.timeScale = 0.1f;
#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPaused = true;
#endif
        //UnityEditor.EditorApplication.isPlaying = false;
        SceneManager.LoadSceneAsync("GameLostScene", LoadSceneMode.Single);
    }

    public static void gameStartProc()
    {
        if (!gameStart)
        {
            //gameManager.enemySpawner.SetActive(true);
            gameStartTime = Time.time;
            //if (gameManager.wallOfDeath != null)
            //{
            //    gameManager.wallOfDeath.enabled = true;
            //}
            sLeftController.GetComponent<VRTK_ControllerActions>().ToggleControllerModel(false, null);
            sRightController.GetComponent<VRTK_ControllerActions>().ToggleControllerModel(false, null);

            gameStart = true;
        }
    }
    public void addScoreProc(float waitTime, int scoreReward)
    {
        StartCoroutine(addScore(waitTime, scoreReward));
    }

    public IEnumerator addScore(float waitTime, int scoreReward)
    {
        yield return new WaitForSeconds(waitTime);

        score += scoreReward;
    }
}
