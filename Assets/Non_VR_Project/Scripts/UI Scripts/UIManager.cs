using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RadialProgressClass TaserShooter;
    [SerializeField] private RadialProgressClass Catcher;
    [SerializeField] private RadialProgressClass FallingDriller;
    [SerializeField] private RadialProgressClass DrillerTrap;
    [SerializeField] private HoroProgressClass Spawn_Points;
    [SerializeField] private ScoreUpdate ScoreClass;
    [SerializeField] private HealthBarClass kittiesBar;
    [SerializeField] private EnergyProgressClass energyBar;
    [SerializeField] public SpeedsProgressClass speedBar;
    // Waypoint manger refence: (type of GameObject)
    [SerializeField] private GameObject waypointManager;
    [SerializeField] private WaypointManager waypointManagerScript;
    [SerializeField] private UIConnector uiConnector;
    [SerializeField] private Text catsCount;
    

    public ParticleSystem ExplosionParticleSystem;
    private SpawnObjects spawnobject;
    private SpawnObjects spawnobjectNVR;
    public GameObject vrPlayer = null;
    public GameObject ClientSide = null;
    public bool find = false;
    [SerializeField] private int iconDistanceinPercentage;
    private bool isParticleAllowed = true;
    private float particle_Counter;

    private int catsCount_LoseCats = 9;
    public ParticleSystem ExplosionCatsLost;

    void Start()
    {
        if (waypointManagerScript == null) {
            waypointManagerScript = waypointManager.GetComponent<WaypointManager>();           
        }
        //print("Script start!");
    }

    void EnemeyLockerRoom()
    {
        //Enable the types of enemies if distance satisfy the requirement.
        iconDistanceinPercentage = waypointManagerScript.getDistancePercentage();
        if (iconDistanceinPercentage >= 0 && !spawnobject.lock_TaserShooter) spawnobject.lock_TaserShooter = true;
        if (iconDistanceinPercentage >= 15 && !spawnobject.lock_Catcher) spawnobject.lock_Catcher = true;
        if (iconDistanceinPercentage >= 30 && !spawnobject.lock_FallingDriller) spawnobject.lock_FallingDriller = true;
        if (iconDistanceinPercentage >= 50 && !spawnobject.lock_DrillerTrap) spawnobject.lock_DrillerTrap = true;
        //Debug.Log("iconDistanceinPercentage : " + iconDistanceinPercentage);
        //Debug.Log("spawnobject.lock_TaserShooter : " + spawnobject.lock_TaserShooter);
        //Debug.Log("spawnobject.lock_Catcher : " + spawnobject.lock_Catcher);
        //Debug.Log("spawnobject.lock_FallingDriller : " + spawnobject.lock_FallingDriller);
        //Debug.Log("spawnobject.lock_DrillerTrap : " + spawnobject.lock_DrillerTrap);
    }
    void FindKart()
    {
        //print("Finding!");
        if (GameObject.Find("Client side") != null && GameObject.Find("Cube(Clone)") != null && !find)
        {
            GameObject obj = GameObject.Find("Cube(Clone)");
            obj.name = "VR_Player";
            vrPlayer = obj;
            find = true;
            ClientSide = GameObject.Find("Client side");
            spawnobject = ClientSide.GetComponent<SpawnObjects>();
            spawnobjectNVR = vrPlayer.GetComponent<SpawnObjects>();
            setThresholdValuesForRadialBar(); // Set's time threshod to each enemy icon
            setThresholdValuesForHoroBar();
            //print("Found it");
        }
    }
    void Update()
    {
        if (vrPlayer == null)
        {
            //ClientSide = GameObject.Find("Client side");
            //if (ClientSide != null)
            //{
                FindKart();
                //spawnobject = ClientSide.GetComponent<SpawnObjects>();
                //setThresholdValuesForRadialBar(); // Set's time threshod to each enemy icon
                //setThresholdValuesForHoroBar();
            //}

        }
        if (vrPlayer != null)
        {

            // Debut to check the value throught network.
            //Debug.Log("Catscount: (from UIManager using activeCatcount)" + spawnobjectNVR.catsCount);
            //Debug.Log("Client, VR side Cats Number: " + spawnobjectNVR.catsCount);
            //Debug.Log("(BeamCollision.multiIsShot)Client, VR side isShoot?: " + spawnobjectNVR.isShoot);
            //Debug.Log("Client, VR side isShoot?: " + spawnobjectNVR.isShoot);
            //Debug.Log("Client, VR side Current Energy: " + spawnobjectNVR.currentEnergy);
            //Debug.Log("VR player's position" + spawnobjectNVR.transform.position);
            if (spawnobjectNVR.isShoot) {

                if (isParticleAllowed)
                {
                    particle_Counter = Time.time;

                    ParticleSystem explosionEffect = Instantiate(ExplosionParticleSystem) as ParticleSystem;
                    //explosionEffect.transform.localScale = new Vector3(3, 3, 3);
                    explosionEffect.transform.position = spawnobjectNVR.transform.position;
                    short pieces = (short)Mathf.Max(8, (50.0f * spawnobjectNVR.transform.localScale.x * spawnobjectNVR.transform.localScale.x));
                    explosionEffect.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, pieces) }, 1);
                    explosionEffect.Play();
                    //spawnobjectNVR.isShoot = false;
                    //BeamCollision.multiIsShot = false;
                    Debug.Log("VR player is been hit!!");
                    isParticleAllowed = false;
                }
                if (Time.time - particle_Counter > 49 * Time.deltaTime)
                {
                    isParticleAllowed = true;
                }


            }
            //
            if (spawnobjectNVR.catsCount < catsCount_LoseCats)
            {
                if (isParticleAllowed)
                {
                    particle_Counter = Time.time;
                    Debug.Log("Cats Lost Puff!!!!!!!");
                    ParticleSystem explosionEffect = Instantiate(ExplosionCatsLost) as ParticleSystem;
                    explosionEffect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    explosionEffect.transform.position = spawnobjectNVR.transform.position;
                    explosionEffect.Play();

                    isParticleAllowed = false;
                }
                if (Time.time - particle_Counter > 49 * Time.deltaTime)
                {
                    isParticleAllowed = true;
                }
                catsCount_LoseCats = spawnobjectNVR.catsCount;
            }
            //
            //if (spawnobjectNVR.isPlayerShoot) {

            //    // Shooting behavior


            //}

            catsCount.text = "CATS: " + spawnobjectNVR.catsCount.ToString();
            //Debug.Log("Cats number: " + spawnobjectNVR.catsCount);



            uiConnector.SetVRplayerTransform(vrPlayer.transform);
            SetCatsCountForHealthBar();
            // Locking system for unlocking types of enemies.
            EnemeyLockerRoom();
            setValuesForRadialBar();
            SetValueForEergyBar();
            SetValueForSpeedBar();
            //setValuesForHoroBar();
        }
    }


    void SetValueForEergyBar()
    {
        //Debug.Log("UI Manager energy track: " + spawnobjectNVR.currentEnergy);
        energyBar.SetCurrentEnergy(spawnobjectNVR.currentEnergy);
    }
    void SetValueForSpeedBar()
    {
        Debug.Log("Nion VR sode current Speed! " + spawnobjectNVR.currentSpeed);
        speedBar.SetCurrentSpeed(spawnobjectNVR.currentSpeed);
    }
    //void setValuesForHoroBar()
    //{
    //    if ()
    //}

    // Sets calues for each radial bar.
    void setValuesForRadialBar()
    {
        // Updates the score.
        int s = ClientSide.GetComponent<SpawnObjects>().getScore();
        ScoreClass.setScore(s);

        // Check which type of Enemy is active.Enable that type and disable all other types.
        // ---------------------------------------------------------------------------------------------------- 1: TaserShooter
        if (spawnobject.isSpawnIndexTaserShooter() && spawnobject.lock_TaserShooter)
        {
           // Debug.Log("Taser Shooter has been selected!");
            TaserShooter.SetCoolDownTimer(spawnobject.getTimerTaserShooter());
            Catcher.disactivateRadialBar();
            FallingDriller.disactivateRadialBar();
            DrillerTrap.disactivateRadialBar();
        }
        if (spawnobject.lock_TaserShooter)
        {
            TaserShooter.disableIconLock();
        }
        else TaserShooter.enableIconLock();
        
        // ---------------------------------------------------------------------------------------------------- 2: Catcher
        if (spawnobject.isSpawnIndexCatcher() && spawnobject.lock_Catcher)
        {
           // Debug.Log("Catcher has been selected!");
            Catcher.SetCoolDownTimer(spawnobject.getTimerCatcher());
            TaserShooter.disactivateRadialBar();
            FallingDriller.disactivateRadialBar();
            DrillerTrap.disactivateRadialBar();
        }
        if (spawnobject.lock_Catcher) Catcher.disableIconLock();
        else Catcher.enableIconLock();
        
        // ---------------------------------------------------------------------------------------------------- 3: Falling Driller
        if (spawnobject.isSpawnIndexFallingDriller() && spawnobject.lock_FallingDriller)
        {
            //Debug.Log("Falling Driller has been selected!");
            FallingDriller.SetCoolDownTimer(spawnobject.getTimerFallingDriller());
            TaserShooter.disactivateRadialBar();
            Catcher.disactivateRadialBar();
            DrillerTrap.disactivateRadialBar();
        }
        if (spawnobject.lock_FallingDriller) FallingDriller.disableIconLock();
        else FallingDriller.enableIconLock();
        
        // ---------------------------------------------------------------------------------------------------- 4. Driller Trap
        if (spawnobject.isSpawnIndexDrillerTrap() && spawnobject.lock_DrillerTrap)
        {
            //Debug.Log("Driller Trap has been selected!");
            DrillerTrap.SetCoolDownTimer(spawnobject.getTimerDrillerTrap());
            TaserShooter.disactivateRadialBar();
            Catcher.disactivateRadialBar();
            FallingDriller.disactivateRadialBar();
        }
        if (spawnobject.lock_DrillerTrap) DrillerTrap.disableIconLock();
        else DrillerTrap.enableIconLock();
    }

    // Sets all threshold variables of radial bar.
    void setThresholdValuesForRadialBar()
    {
        Catcher.SetCoolDownThreshold(spawnobject.getTimerThresholdCatcher());
        TaserShooter.SetCoolDownThreshold(spawnobject.getTimerThresholdTaserShooter());
        FallingDriller.SetCoolDownThreshold(spawnobject.getTimerThresholdFallingDriller());
        DrillerTrap.SetCoolDownThreshold(spawnobject.getTimerThresholdDrillerTrap());
    }
    void setValuesForHoroBar()
    {
        //Spawn_Points.SetHoroValue(spawnobject.getSpawnPoints());
    }
    void setThresholdValuesForHoroBar()
    {
        //Spawn_Points.SetHoroThreshold(spawnobject.getSpawnPointsThreshold());
    }

    // Get posiiton of kart to update temporary game object.
    public Vector3 getPosition()
    {
        if (vrPlayer != null)
            return vrPlayer.transform.position;
        else
            return Vector3.zero;
    }

    void SetCatsCountForHealthBar()
    {
        kittiesBar.setKittiesCount(spawnobjectNVR.catsCount);
    }

}
