using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MirrorGameManager : MonoBehaviour
{
    public PlayerUI playerUI; // The player side UI
    public GameObject playerHUDviewArea; // The view area of the HUD UI, set this active to make the HUD visible, player side
    public GameObject playerKart; // The player side kart
    public GameObject playerHead;
    public int minRocks;
    public int maxRocks;
    public int maxRockInc;
    public float playerHealth; //The total hit point for the player
    public float playerTotalWeaponEnergy; // The total energy of player's weapon
    public GameObject leftController; // The left controller for [VRTK]
    public GameObject rightController; // The right controller for [VRTK]
    public float speedMultiplier; // All the (player kart, enemy move speed, pistol bullet speed, etc.) things that have speed (max speed, acceleration, spawn distance) will scale with this speed (some of them may have individual scale multiplied with this scale)
    public GameObject nonPlayerKart; // The non-player side kart
    public GameObject nonPlayerCatBasket; // The non-player side basket

    public static float sSpeedMultiplier; // All the things that have speed (player kart, enemy move speed, pistol bullet speed, etc.) will scale with this speed (some of them may have individual scale multiplied with this scale)
    public static int sminRocks;
    public static int smaxRocks;
    public static int smaxRockInc;
    public static int score;
    public static float distanceTravelled;
    public static bool gameOver;
    public static MirrorGameManager gameManager;
    public static PlayerKartFollowLaserSpot kartMovementInfo; //The movement information (speed, acceleration, etc.) about the kart
    public static bool gameStart;
    public WallMove wallOfDeath; //The wall that will chasing player
    public GameObject enemySpawner; //The enemy spawner
    //public CustomControllerEvents playerWeapon; //The weapon the player is using
    public static float gameStartTime;
    public static float currentEnergy; // The amount of energy the player currently has
    public static float sPlayerTotalWeaponEnergy;
    public static float currentSpeed; // The current speed of the kart
    public static Vector3 kartLastPosition; // The kart's position in the last fixed update

    // Use this for initialization
    void Start()
    {
        score = 0;
        gameOver = false;
        gameStart = false;
        sminRocks = minRocks;
        smaxRocks = maxRocks;
        smaxRockInc = maxRockInc;
        gameManager = this;
        wallOfDeath = FindObjectOfType<WallMove>();
        gameStartTime = 0;
        nonPlayerCatBasket = FindObjectOfType<KartBasket>().gameObject;
        sPlayerTotalWeaponEnergy = playerTotalWeaponEnergy;
        sSpeedMultiplier = speedMultiplier;
        currentEnergy = playerTotalWeaponEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled = playerKart.transform.position.z - 0;

        if (!gameOver)
        {
            if (playerUI.gameObject.activeInHierarchy)
            {
                playerUI.score.text = "SCORE" + "\r\n" + score.ToString("0000");
                playerUI.speed.text = "SPEED" + "\r\n" + kartMovementInfo.currentSpeed.ToString("0000");
                if (gameStart)
                {
                    playerUI.progression.text = "TIME" + "\r\n" + (Time.time - GameManager.gameStartTime).ToString("0000");
                }
                playerUI.energy.text = "ENERGY" + "\r\n" + currentEnergy.ToString("0000");
            }
        }
    }

    void FixedUpdate()
    {
        currentSpeed = (playerKart.transform.position - kartLastPosition).magnitude / Time.fixedUnscaledDeltaTime;
        kartLastPosition = playerKart.transform.position;
    }

    public static void gameOverProc()
    {
        if (!gameOver)
        {
            gameManager.playerUI.score.text = "GAMEOVER" + "\r\n" + score.ToString("0000");
            gameManager.playerUI.speed.text = "SPEED" + "\r\n" + kartMovementInfo.currentSpeed.ToString("0000");
            gameManager.playerUI.progression.text = "TIME" + "\r\n" + (Time.time - GameManager.gameStartTime).ToString("0000");
            gameManager.playerUI.energy.text = "ENERGY" + "\r\n" + "0000";
        }

        gameOver = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    public static void gameStartProc()
    {
        if (!gameStart)
        {
            gameManager.enemySpawner.SetActive(true);
            gameStartTime = Time.time;
            if (gameManager.wallOfDeath != null)
            {
                gameManager.wallOfDeath.enabled = true;
            }
            gameStart = true;
        }
    }
}
