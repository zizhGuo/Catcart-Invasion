using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class InitiateObjects : MonoBehaviour
{
    //GameObject[] taserEnemy;
    public GameObject taserEnemyPreb;
    public GameObject[] taserEnemy;
    public GameObject[] spawnObject;
    public bool actFlag0 = false;
    public bool actFlag1 = false;
    public bool actFlag2;
    public bool actFlag3;
    public bool actFlag4;
    public bool actFlag5;
    public bool actFlag6;
    public bool actFlag7;
    public bool actFlag8;
    public bool actFlag9;

    public bool desFlag0 = false; // Temporary Flags variable to indicate the searching status
    public bool desFlag1 = false;
    public bool desFlag2 = false;
    public bool desFlag3 = false;
    public bool desFlag4 = false;
    public bool desFlag5 = false;
    public bool desFlag6 = false;
    public bool desFlag7 = false;
    public bool desFlag8 = false;
    public bool desFlag9 = false;


    void Start()
    {
        print("Start!");

        taserEnemy = new GameObject[10];
        spawnObject = new GameObject[10];

        //print("start!");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Taser: 0") != null && !actFlag0) // Determine if there are objects spawned by Non-VR player
        {
            GameObject obj = GameObject.Find("Taser: 0");
            taserEnemy[0] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
            taserEnemy[0].name = "TaserDrone: 0";
            taserEnemy[0].transform.position = obj.transform.position;
            actFlag0 = true; // Switch the state.
            print("Found it!");
        }
        else if (GameObject.Find("Taser: 0") != null && actFlag0)
        {
            //try
            //{
                GameObject.Find("Taser: 0").transform.position = taserEnemy[0].transform.position;
            //print("On tracking...");
            //}// Sychronize the position of spawned objects aliging with the position of actual drones.
            //catch
            //{
            //    print("lalalala!");
            //}
        }
        else if (GameObject.Find("Taser: 0") == null && actFlag0)
        {
            print("It's been destroyed! ");
        }
        

        if (GameObject.Find("Taser: 1") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag1) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 1");
                taserEnemy[1] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[1].name = "TaserDrone: 1";
                taserEnemy[1].transform.position = obj.transform.position;
                actFlag1 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 1").transform.position = taserEnemy[1].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 2") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag2) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 2");
                taserEnemy[2] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[2].name = "TaserDrone: 2";
                taserEnemy[2].transform.position = obj.transform.position;
                actFlag2 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 2").transform.position = taserEnemy[2].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 3") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag3) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 3");
                taserEnemy[3] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[3].name = "TaserDrone: 3";
                taserEnemy[3].transform.position = obj.transform.position;
                actFlag3 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 3").transform.position = taserEnemy[3].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 4") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag4) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 4");
                taserEnemy[4] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[4].name = "TaserDrone: 4";
                taserEnemy[4].transform.position = obj.transform.position;
                actFlag4 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 4").transform.position = taserEnemy[4].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 5") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag5) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 5");
                taserEnemy[5] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[5].name = "TaserDrone: 5";
                taserEnemy[5].transform.position = obj.transform.position;
                actFlag5 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 5").transform.position = taserEnemy[5].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 6") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag6) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 6");
                taserEnemy[6] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[6].name = "TaserDrone: 6";
                taserEnemy[6].transform.position = obj.transform.position;
                actFlag6 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 6").transform.position = taserEnemy[6].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 7") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag7) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 7");
                taserEnemy[7] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[7].name = "TaserDrone: 7";
                taserEnemy[7].transform.position = obj.transform.position;
                actFlag7 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 7").transform.position = taserEnemy[7].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 8") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag8) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 8");
                taserEnemy[8] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[8].name = "TaserDrone: 8";
                taserEnemy[8].transform.position = obj.transform.position;
                actFlag8 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 8").transform.position = taserEnemy[8].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }

        if (GameObject.Find("Taser: 9") != null) // Determine if there are objects spawned by Non-VR player
        {
            if (!actFlag9) // Flag to control the state
            {
                GameObject obj = GameObject.Find("Taser: 9");
                taserEnemy[9] = Instantiate(taserEnemyPreb); // Instantiate the actual drone in the VR scene. 
                taserEnemy[9].name = "TaserDrone: 9";
                taserEnemy[9].transform.position = obj.transform.position;
                actFlag9 = true; // Switch the state.
                print("Found it!");
            }
            else
            {
                GameObject.Find("Taser: 9").transform.position = taserEnemy[9].transform.position; // Sychronize the position of spawned objects aliging with the position of actual drones.
            }
        }
        /*
        Destroying actual Enemy called "TaserDrone: 0"
        Date: 2/22/2018
        Author: Zizhun Guo
        */
        if (GameObject.Find("TaserDrone: 0") != null && actFlag0 && Input.GetKey(KeyCode.K))
        {
            GameObject obj = GameObject.Find("TaserDrone: 0");
            Destroy(obj);
            desFlag0 = true;
            print("[VR] It's been destroyed!");
            print("[VR] actFlag0: "+ actFlag0);
            print("[VR] desFlag0: "+ desFlag0);
            print("??");
        }


        //if (GameObject.Find("TaserDrone: 0") == null && !desFlag0) // Determine if there are objects spawned by Non-VR player
        //{
        //    desFlag0 = true;
        //}
        //if (taserEnemy[0] == null && actFlag[0]) td1 = true;
    }


}
