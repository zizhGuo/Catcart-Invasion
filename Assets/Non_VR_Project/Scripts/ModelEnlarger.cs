using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEnlarger : MonoBehaviour {

    [SerializeField] int enemyCount_Taser = 0;
    [SerializeField] int enemyCount_Catcher = 0;
    [SerializeField] int enemyCount_FallingDriller = 0;
    [SerializeField] int enemyCount_DillerTrap = 0;
    [SerializeField] float scaler_TaserShooter = 4.0f;
    [SerializeField] float scaler_Catcher = 1.1f;
    [SerializeField] float scaler_FallingDriller = 6.0f;
    [SerializeField] float scaler_DrillerTrap = 6.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // ---------------------------------------------------------------------------------------------------- 1. TaserShooter
        if (GameObject.Find("NewTaserShooterEnemy(Clone)") != null)
        {
            GameObject ob = GameObject.Find("NewTaserShooterEnemy(Clone)");
            ob.name = "Enlarged Taser";
            //
            // Set the local enemy, Taser,  a larger scale. 
            //

            Transform child_0 = ob.transform.GetChild(0).transform;
            child_0.localScale = new Vector3(child_0.localScale.x * scaler_TaserShooter, child_0.localScale.y * scaler_TaserShooter, child_0.localScale.z * scaler_TaserShooter);
            
            
            //ob.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);

            //.Log("One wild enemy's been caught! Its name is : " + ob.name + " Enemies count:  " + enemyCount_Taser);
            enemyCount_Taser++;
        }
        // ---------------------------------------------------------------------------------------------------- 2. Catcher
        if (GameObject.Find("MultCatcherEnemy(Clone)") != null)
        {
            GameObject ob = GameObject.Find("MultCatcherEnemy(Clone)");
            ob.name = "Enlarged Catcher: " + enemyCount_Catcher;
            //
            // Set the local enemy, Catcher,  a larger scale. 
            //
            Transform child_0 = ob.transform.GetChild(0).transform;
            child_0.localScale = new Vector3(child_0.localScale.x * scaler_Catcher, child_0.localScale.y * scaler_Catcher, child_0.localScale.z * scaler_Catcher);

            Transform child_1 = ob.transform.GetChild(2).transform;
            child_1.localScale = new Vector3(child_1.localScale.x * scaler_Catcher, child_1.localScale.y * scaler_Catcher, child_1.localScale.z * scaler_Catcher);

            //ob.transform.localScale = new Vector3(2, 2, 2);
            //Debug.Log("One wild enemy's been caught! Its name is : " + ob.name + " Enemies count:  " + enemyCount_Catcher);
            enemyCount_Catcher++;
        }
        // ---------------------------------------------------------------------------------------------------- 3. FallingDriller
        if (GameObject.Find("MultNewDrillerEnemy_Temp(Clone)") != null)
        {
            GameObject ob = GameObject.Find("MultNewDrillerEnemy_Temp(Clone)");
            ob.name = "Enlarged FallingDriller: " + enemyCount_FallingDriller;
            //
            // Set the local enemy, FallingDriller,  a larger scale. 
            //
            ob.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
            //Debug.Log("One wild enemy's been caught! Its name is : " + ob.name + " Enemies count:  " + enemyCount_FallingDriller);
            enemyCount_FallingDriller++;
        }
        // ---------------------------------------------------------------------------------------------------- 4. DrillerTrap
        if (GameObject.Find("MultDrillerSpawnMoveArea(Clone)") != null)
        {
            GameObject ob = GameObject.Find("MultDrillerSpawnMoveArea(Clone)");
            ob.name = "Enlarged DrillerTrap: " + enemyCount_DillerTrap;
            //
            // Set the local enemy, DrillerTrap,  a larger scale. 
            //
            //ob.transform.localScale = new Vector3(2, 2, 2);
            //Debug.Log("One wild enemy's been caught! Its name is : " + ob.name + " Enemies count:  " + enemyCount_DillerTrap);
            enemyCount_DillerTrap++;
        }

        //if (GameObject.FindWithTag("Enemy") != null)
        //{
        //    GameObject ob = GameObject.FindWithTag("Enemy");
        //    if (ob.name == "MultTaserShooterEnemy(Clone)")
        //    {
        //        ob.name = "Enlarged one";
        //        Debug.Log("One wild enemy's been caught!");
        //    }
        //}
    }
}
