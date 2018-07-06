using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnObjects : NetworkBehaviour
{

    public GameObject[] dronesToSpawn;
    public GameObject obstacles;
    public GameObject[] obsEnemy;

    public Transform spawnPosition;
    public GameObject[] dronesPool;
    public GameObject drone;
    public InitiateObjects initiater;

    //Non-VR Game Manager
    public GameObject GM;
    public NonVRGameManager GMscript;


    public int objectPoolSize = 10;

    public float counter_TaserShooter;
    public float counter_Catcher;
    public float counter_FallingDriller;
    public float counter_DrillerTrap;
    public float counter_Rock;
    public int spawn_Points;

    // ------------------------------------------------ Beamshot counter
    public float beamCollision_Counter;
    public bool beamCollision_Lock;
    public float playerShoot_Couter;
    public bool playerShoot_Lock;

    //public int Max_Spawn_Points = 6;
    //public int spawn_Points_TaserShooter;
    //public int spawn_Points_Catcher;
    //public int spawn_Points_FallingDriller;
    //public int spawn_Points_DrillerTrap;
    //public float counter_Spawn_Points;

    public float cd_TaserShooter;
    public float cd_Catcher;
    public float cd_FallingDriller;
    public float cd_DrillerTrap;
    public float cd_Rock;

    public bool lock_TaserShooter;
    public bool lock_Catcher;
    public bool lock_FallingDriller;
    public bool lock_DrillerTrap;
    public bool lock_Rock;

    public int testNumber;

    public int index = 0;
    public int spawnIndex = 1;
    public int score; // Store the score here. 

    [SerializeField] Vector3 weaponPos = new Vector3(-780.66f, 0.983f, 0.287f);
    public GameObject weapon;

    // -------------------------------------------------- Test to update the number of cats
    public GameManager gameManager;
    [SyncVar] public int catsCount = 0;
    [SyncVar] public float currentEnergy;
    [SyncVar] public bool isShoot;
    [SyncVar] public bool isPlayerShoot;
    [SyncVar] public int abc = 0;
    [SyncVar] public float currentSpeed;

    [SyncVar] public Vector3 playerWeaponPos;
    [SyncVar] public Quaternion playerWeaponRot;
    [SyncVar] public Vector3 laserPointerPos;
    [SyncVar] public Quaternion laserPointerRot;
    //[SyncVar] public bool isShootClient;
    //[SyncVar] public bool isPlayerShootClient;

    public GameObject serverClient = null;
    public SpawnObjects nonVRPlayer;

    [SerializeField] private Transform playerWeapon;
    [SerializeField] private Transform laserPointer;
    //[SyncVar] public bool isShootLock = false;
    //[SyncVar] public bool isPlayerShootLock = false;

    // --------------------------------------------------------------------------- Testing for spawning the VR weapon.
    [Command]
    void CmdSpawnWeapon(GameObject ob, Vector3 position)
    {
        Debug.Log("Weapon spawned");
        GameObject obb = (GameObject)Instantiate<GameObject>(ob);
        NetworkServer.Spawn(obb);
        //AddObject(0, spawnPosition.position);
        //AddObject(0, transform.position);
        obb.transform.position = position;
        //index++;
    }

    void Start()
    {

        if (isServer)
        {

        }

        initiater = gameObject.GetComponent<InitiateObjects>();
        //Game Manager Script
        GMscript = GM.GetComponent<NonVRGameManager>();

        initiater.enabled = false;

        // Timer counter
        // Enemy type:  1. Taser shooter
        //              2. Catcher
        //              3. Falling Driller
        //              4. DrillerTrap
        //              5. Clinder(for Test)
        counter_TaserShooter = Time.time;
        counter_Catcher = Time.time;
        counter_FallingDriller = Time.time;
        counter_DrillerTrap = Time.time;
        counter_Rock = Time.time;
        beamCollision_Counter = Time.time;
        beamCollision_Lock = true;
        playerShoot_Couter = Time.time;
        playerShoot_Lock = true;

    //Cool-down Time
    // Enemy type:  1. Taser shooter
    //              2. Catcher
    //              3. Falling Driller
    //              4. DrillerTrap
    //              5. Clinder(for Test)
    cd_TaserShooter = 0.5f;
        cd_Catcher = 1.0f;
        cd_FallingDriller = 1.5f;
        cd_DrillerTrap = 3.0f;
        cd_Rock = 3.0f;

        // For test purpose, just change the boolean value.
        lock_TaserShooter = true;
        lock_Catcher = false;
        lock_FallingDriller = false;
        lock_DrillerTrap = false;
        lock_Rock = true;


        // Spawnpoints
        // Enemy type:  1. Taser shooter
        //              2. Catcher
        //              3. Falling rock
        //              4. DrillerTrap
        //              5. Driller
        //spawn_Points_TaserShooter = 1;
        //spawn_Points_Catcher = 2;
        //spawn_Points_FallingDriller = 2;
        //spawn_Points_DrillerTrap = 3;
        // Set up Timer for SpawnPoints
        //counter_Spawn_Points = Time.time;

        //SpawnPionts
        //spawn_Points = 6;a
        testNumber = 0;

        dronesPool = new GameObject[objectPoolSize];
        obsEnemy = new GameObject[100];
        spawnIndex = 1;

    }
    public int getTestNumber()
    {
        return testNumber;
    }

    [Command]
    void CmdSendCatsCounttoServer(int count)
    {
        catsCount = count;
    }
    [Command]
    void CmdSendEnegytoServer(float energy)
    {
        currentEnergy = energy;
    }
    [Command]
    void CmdSendShootingSignaltoServer(bool isShoot)
    {
        this.isShoot = isShoot;
    }
    [Command]
    void CmdSendPlayerShootingSignaltoServer(bool isPlayerShoot)
    {
        this.isPlayerShoot = isPlayerShoot;
    }
    [Command]
    void CmdUpdatePlayerSpeed(float speed)
    {
        this.currentSpeed = speed;
    }
    //[Command]
    //void CmdClientSendBackShootingSignal(bool isShoot)
    //{
    //    this.isShoot = isShoot;
    //}

    //[Command]
    //void CmdSendDataFromClient(int abc)
    //{
    //    this.abc = abc;
    //}

    [Command]
    void CmdSendWeaponLaserInfo(Vector3 A_pos, Quaternion A_rot, Vector3 B_pos, Quaternion B_rot)
    {
        this.playerWeaponPos = A_pos;
        this.playerWeaponRot = A_rot;
        this.laserPointerPos = B_pos;
        this.laserPointerRot = B_rot;
    }
    void FindServerClient()
    {
        if (GameObject.Find("Client side") != null && GameObject.Find("Server side") != null)
        {
            GameObject obj = GameObject.Find("Client side");
            obj.name = "Non-VR player";
            serverClient = obj;
            //ClientSide = GameObject.Find("Client side");
            //spawnobject = ClientSide.GetComponent<SpawnObjects>();
            nonVRPlayer = serverClient.GetComponent<SpawnObjects>();
        }
    }

    void FindServerServer()
    {
        if (FindObjectOfType<GameManager>()) //
        {
            gameManager = FindObjectOfType<GameManager>();    // Define this player as the VR player.
            catsCount = gameManager.activeCatCount;
            //CmdSpawnWeapon(weapon, weaponPos);
            Debug.Log("GM found!!");
        }
        //if (GameObject.Find("PlayerWeapon")) playerWeapon = GameObject.Find("PlayerWeapon").transform;
        if (GameObject.Find("PlayerWeapon")) {
            //playerWeapon = GameObject.Find("[CameraRig] Kart/Controller (left)/RayGun/BarrelEnd/Shooter").transform;
            playerWeapon = GameObject.Find("PlayerWeapon").transform;
            Debug.Log("Weapon's Shooter's been found!");
        }
        
        if (GameObject.Find("LaserPointer")) laserPointer = GameObject.Find("LaserPointer").transform;
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (isServer)
        {
            // ----------------------------------------------------------------------------------Get local Client Game Object
            if (gameManager == null) FindServerServer();
            if (serverClient == null) FindServerClient();
            //if (serverClient != null) Debug.Log("nonVRPlayerabc : "+ nonVRPlayer.abc);

            CmdSendPlayerShootingSignaltoServer(MultiShooter.isPlayerShoot);
            CmdSendShootingSignaltoServer(BeamCollision.multiIsShot);

            Debug.Log("BeamCollision value: " + BeamCollision.multiIsShot);

            if (BeamCollision.multiIsShot)
            {
                if (beamCollision_Lock) {
                    beamCollision_Counter = Time.time;
                    beamCollision_Lock = false;
                }
                if (Time.time - beamCollision_Counter < 20 * Time.deltaTime)
                {
                    Debug.Log("Status: shot!");                   
                }
                else {
                    BeamCollision.multiIsShot = false;
                    beamCollision_Lock = true;
                }
                Debug.Log("Reset BeamCollision to False: " + BeamCollision.multiIsShot);
            }
            if (MultiShooter.isPlayerShoot)
            {
                if (playerShoot_Lock) {
                    playerShoot_Couter = Time.time;
                    playerShoot_Lock = false;
                }
                if (Time.time - playerShoot_Couter < 20 * Time.deltaTime)
                {
                    Debug.Log("Status: player shot!");
                }
                else {
                    MultiShooter.isPlayerShoot = false;
                    playerShoot_Lock = true;
                }
                Debug.Log("Reset PlayerShoot to False: " + MultiShooter.isPlayerShoot);
            }


            CmdSendCatsCounttoServer(gameManager.activeCatCount);
            CmdSendEnegytoServer(GameManager.currentEnergy);
            CmdUpdatePlayerSpeed(GameManager.currentSpeed);
            
            if (playerWeapon != null && laserPointer != null) {
                if (playerWeapon.transform.parent != null)
                {
                    //Debug.Log("Weapon's name: " + playerWeapon.name);
                    //Debug.Log("Parent's name: " + playerWeapon.parent.name);
                    //Debug.Log("Player weapon's parent rotation : " + playerWeapon.rotation.eulerAngles);
                    CmdSendWeaponLaserInfo(playerWeapon.parent.transform.position, playerWeapon.parent.transform.rotation, laserPointer.position, laserPointer.rotation);
                }
                //CmdSendWeaponLaserInfo(playerWeapon.position, playerWeapon.rotation, laserPointer.position, laserPointer.rotation);

            }
            
           // Debug.Log("Value from Non-VR: " + abc);

            //Debug.Log("catCount: " + catsCount);
            //Debug.Log("Current Energy: " + gameManager);
            //Debug.Log("Is shot???: " + isShoot);
            //Debug.Log("Current Energy" + GameManager.currentEnergy);

        }
        if (!isServer)
        {

            //if (Input.GetKey(KeyCode.M)) CmdSendDataFromClient(123);
            //if (Input.GetKey(KeyCode.N)) BeamCollision.multiIsShot = true;
            //Debug.Log("VR side Cats Number (GM):" + gameManager.activeCatCount);
            // Eastern Egg to unlock all types of enemies.
            if (Input.GetKey(KeyCode.U))
            {
                lock_TaserShooter = true;
                lock_Catcher = true;
                lock_FallingDriller = true;
                lock_DrillerTrap = true;
            }
            if (Input.GetKey(KeyCode.Alpha1)) spawnIndex = 1;
            if (Input.GetKey(KeyCode.Alpha2)) spawnIndex = 2;
            if (Input.GetKey(KeyCode.Alpha3)) spawnIndex = 3;
            if (Input.GetKey(KeyCode.Alpha4)) spawnIndex = 4;
            if (Input.GetKey(KeyCode.Alpha5)) spawnIndex = 5;
            if (Input.GetKey(KeyCode.Alpha6)) spawnIndex = 6;
            //if (Input.GetKey(KeyCode.Alpha4)) spawnIndex = 4;

            /* Cancelation of Spawning Mana
            if (Time.time - counter_Spawn_Points > 2.0f && spawn_Points < Max_Spawn_Points)
            {
                spawn_Points++;
                counter_Spawn_Points = Time.time;
            }
            */
            if (Input.GetKey(KeyCode.M)) BeamCollision.multiIsShot = false;
            if (spawnIndex == 6 && Time.time - counter_TaserShooter > cd_TaserShooter && Input.GetKey(KeyCode.Mouse0) && lock_TaserShooter)
            {
                CmdSpawnTaserEnemy(spawnIndex, new Vector3(transform.position.x, 3, transform.position.z));
                counter_TaserShooter = Time.time;
                score++;
            }

            // ---------------------------------------------------------------------------------------------------- Press 1 choose TaserShooter
            // ---------------------------------------------------------------------------------------------------- Click Mouse.left to Spawn
            if (spawnIndex == 1 && Time.time - counter_TaserShooter > cd_TaserShooter && Input.GetKey(KeyCode.Mouse0) && lock_TaserShooter)
            {
                CmdSpawnTaserEnemy(spawnIndex, new Vector3(transform.position.x, 3, transform.position.z));
                counter_TaserShooter = Time.time;
                score++;
            }
            // ---------------------------------------------------------------------------------------------------- Press 2 choose Catcher
            // ---------------------------------------------------------------------------------------------------- Click Mouse.left to Spawn
            if (spawnIndex == 2 && Time.time - counter_Catcher > cd_Catcher && Input.GetKey(KeyCode.Mouse0) && lock_Catcher)
            {
                CmdSpawnTaserEnemy(spawnIndex, new Vector3(transform.position.x, 3, transform.position.z));
                counter_Catcher = Time.time;
                score++;
            }
            // ---------------------------------------------------------------------------------------------------- Press 3 choose Driller
            // ---------------------------------------------------------------------------------------------------- Click Mouse.left to Spawn
            if (spawnIndex == 3 && Time.time - counter_FallingDriller > cd_FallingDriller && Input.GetKey(KeyCode.Mouse0) && lock_FallingDriller)
            {
                CmdSpawnTaserEnemy(spawnIndex, new Vector3(transform.position.x, 8, transform.position.z));
                counter_FallingDriller = Time.time;
                score++;
            }
            // ---------------------------------------------------------------------------------------------------- Press 4 choose DrillerTrap
            // ---------------------------------------------------------------------------------------------------- Click Mouse.left to Spawn
            if (spawnIndex == 4 && Time.time - counter_DrillerTrap > cd_DrillerTrap && Input.GetKey(KeyCode.Mouse0) && lock_DrillerTrap)
            {
                CmdSpawnTaserEnemy(spawnIndex, transform.position);
                counter_DrillerTrap = Time.time;
                score++;
            }
            // ---------------------------------------------------------------------------------------------------- Test: Press 5 choose Cylinder
            // ---------------------------------------------------------------------------------------------------- Test: Click Mouse.left to Spawn
            if (spawnIndex == 5 && Time.time - counter_Rock > cd_Rock && Input.GetKey(KeyCode.Mouse0) && lock_Rock) // Left mouse key
            {
                CmdSpawnTaserEnemy(spawnIndex, new Vector3(transform.position.x, 30, transform.position.z));
                counter_Rock = Time.time;
                score++;
            }


            /*
			if (Input.GetKey(KeyCode.H) && Time.time - counter_TaserShooter > 1) // Spawn game objects on server
            {
                //CmdObjectPooler(0, objectPoolSize);
                initiater.enabled = true;
				counter_TaserShooter = Time.time;
            }*/

            //if (Input.GetKey(KeyCode.T))
            // GMscript.isFollowPlayer = !GMscript.isFollowPlayer;
            //print(GMscript.isFollowPlayer);
            //if (Time.time - counter > 1 && Input.GetKey(KeyCode.T))
            //{
            //    GMscript.isFollowPlayer = !GMscript.isFollowPlayer;
            //    print("Main: Switch the Camera Following item! " + GMscript.isFollowPlayer);
            //    counter = Time.time;
            //}

        }

        //if (Time.time - counter > 1 && Input.GetKey(KeyCode.Mouse0)) // 
        //{
        //    CmdSpawnDrone();
        //    counter = Time.time; 
        //}
        /* Detect actual Enemies' status
         * Date: 2/22/2018
         * Author: Zizhun Guo
         */
        if (initiater.desFlag0 && initiater.actFlag0) // Detection if the enemey's been destroyed or not
        {
            print("[NonVR] It's been destoryed!");
            print("[VR] actFlag0: " + initiater.actFlag0);
            print("[VR] desFlag0: " + initiater.desFlag0);
            dronesPool[0].SetActive(false);
            initiater.desFlag0 = false;
            initiater.actFlag0 = false;
        }

        //if (initiater.td1) dronesPool[0].SetActive(false);
    }
    [Command]
    void CmdSpawnTaserEnemy(int objIndex, Vector3 position)
    {
        Debug.Log("Spawn Enemy is Spawned");
        obsEnemy[index] = (GameObject)Instantiate<GameObject>(dronesToSpawn[objIndex]);
        NetworkServer.Spawn(obsEnemy[index]);
        //AddObject(0, spawnPosition.position);
        //AddObject(0, transform.position);
        obsEnemy[index].transform.position = position;
        index++;
    }

    [Command]
    void CmdSpawnObstacle(int objIndex, Vector3 position)
    {
        Debug.Log("Spawn Obstacle is Spawned");
        GameObject obj = (GameObject)Instantiate<GameObject>(obstacles, position, transform.rotation);
        NetworkServer.Spawn(obj);
        //AddObject(0, spawnPosition.position);
        //AddObject(0, transform.position);
        obj.transform.position = position;
        index++;
    }

    //[Command]
    //public void CmdObjectPooler(int objIndex, int size)  // Initialize the Object Pool
    //{
    //    for (int i = 0; i < size; i++)
    //    {
    //        dronesPool[i] = Instantiate<GameObject>(dronesToSpawn[objIndex]);
    //        dronesPool[i].name = "Taser: " + i;
    //        NetworkServer.Spawn(dronesPool[i]);
    //        dronesPool[i].SetActive(false);
    //    }
    //}
    //[Command]
    //void CmdSpawnDrone()
    //{
    //    //AddObject(0, spawnPosition.position);
    //    //AddObject(0, transform.position);
    //    AddObject(transform.position);
    //}

    //public void AddObject(Vector3 position)  // Active the objects from Pool
    //{
    //    foreach (var obj in dronesPool)
    //    {
    //        if (!obj.activeInHierarchy)
    //        {
    //            Debug.Log("Activating object " + obj.name + " at " + position);
    //            obj.transform.position = position;
    //            obj.SetActive(true);
    //            break;
    //        }
    //        if (obj == null) return;
    //    }
    //}
    // -------------------------------------------------------------------- 1. TaserShooter
    public float getTimerTaserShooter() // Get the filling timer
    {
        return Time.time - counter_TaserShooter;
    }

    public float getTimerThresholdTaserShooter()
    {
        //To be change if threshold value changes
        return cd_TaserShooter;
    }
    // -------------------------------------------------------------------- 2. Catcher
    //Change counter variable
    public float getTimerCatcher()
    {
        return Time.time - counter_Catcher;
    }

    public float getTimerThresholdCatcher()
    {
        //To be change if threshold value changes
        return cd_Catcher;
    }
    // -------------------------------------------------------------------- 3. Falling Driller
    public float getTimerFallingDriller()
    {
        return Time.time - counter_FallingDriller;
    }

    public float getTimerThresholdFallingDriller()
    {
        //To be change if threshold value changes
        return cd_FallingDriller;
    }
    // -------------------------------------------------------------------- 4. Driller Trap
    public float getTimerDrillerTrap()
    {
        return Time.time - counter_DrillerTrap;
    }

    public float getTimerThresholdDrillerTrap()
    {
        //To be change if threshold value changes
        return cd_DrillerTrap;
    }
    // -------------------------------------------------------------------- Condition of Index
    public bool isSpawnIndexTaserShooter()
    {
        return spawnIndex == 1 ? true : false;
    }

    public bool isSpawnIndexCatcher()
    {
        return spawnIndex == 2 ? true : false;
    }

    public bool isSpawnIndexFallingDriller()
    {
        return spawnIndex == 3 ? true : false;
    }
    // -------------------------------------------------------------------- Get Score
    public bool isSpawnIndexDrillerTrap()
    {
        return spawnIndex == 4 ? true : false;
    }
    // -------------------------------------------------------------------- Get Spawn Points
    /*
    public int getSpawnPoints()
    {
        return spawn_Points;
    }
    // -------------------------------------------------------------------- Get Max Spawn Points for Threshold
    public int getSpawnPointsThreshold()
    {
        return Max_Spawn_Points;
    }
    */
    public int getScore()
    {
        return score;
    }
}