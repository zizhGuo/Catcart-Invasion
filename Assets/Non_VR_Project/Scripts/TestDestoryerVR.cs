using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDestoryerVR : MonoBehaviour
{
    [SerializeField] float counter_TaserShooter;
    [SerializeField] float cd_TaserShooter = 0.2f;
    [SerializeField] int index = 0;
    // Use this for initialization
    void Start()
    {
        counter_TaserShooter = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] EnemiesList = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (Time.time - counter_TaserShooter > cd_TaserShooter && Input.GetKey(KeyCode.Z))
        {
            //if (EnemiesList[index].name == "NewTaserShooterEnemy(Clone)")
            //{
            //    Destroy(EnemiesList[index]);
            //    counter_TaserShooter = Time.time;
            //}

            if (EnemiesList[index].name == "MultCatcherEnemy(Clone)")
            {
                Destroy(EnemiesList[index]);
                counter_TaserShooter = Time.time;
            }
            else index++;
            //if (EnemiesList[0].name == "MultNewDrillerEnemy_Temp(Clone)")
            //{
            //    Destroy(EnemiesList[0]);
            //    counter_TaserShooter = Time.time;
            //}

        }
    }
}
