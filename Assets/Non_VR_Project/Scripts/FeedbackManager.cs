using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FeedbackManager : MonoBehaviour {

    public GameObject vrPlayer = null;
    public SpawnObjects spawnobjectNVR;
    [SerializeField] private Transform manModel;
    [SerializeField] private Transform playerWeapon_nonvr;
    [SerializeField] private Transform laserPointer_nonvr;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject Wave;
    [SerializeField] private float manScale;
    private System.Random rdIndex;

    //private GameObject[] originalEnemiesLst;
    //private GameObject[] newEnemiesLst_Taser;
    private float timer;
    private float cooldown = 0.5f;
    // Use this for initialization
    void Start () {
        
        manModel.gameObject.SetActive(true);
        manScale = manModel.gameObject.transform.localScale.x;
        timer = Time.time;
        rdIndex = new System.Random(DateTime.Now.Millisecond);
        //originalEnemiesLst = new GameObject[]();
        Bullet.GetComponent<BeamCollision>().enabled = false;
    }
    void FindKart()
    {
        if (GameObject.Find("VR_Player") != null && GameObject.Find("Client side") != null)
        {
            GameObject obj = GameObject.Find("VR_Player");
            vrPlayer = obj;
            spawnobjectNVR = vrPlayer.GetComponent<SpawnObjects>();
        }
    }

    //private List<GameObject> TraverseTaserShooter(List<GameObject> lst) {

    //    if (GameObject.FindWithTag("Enemy"))
    //    {
    //        GameObject ob = GameObject.FindWithTag("Enemy")
    //    }

    //    return 
    //}
    // Update is called once per frame
    void Update()
    {
        if (vrPlayer == null)
        {
            FindKart();
        }
        if (vrPlayer != null)
        {
            manModel.transform.position = spawnobjectNVR.transform.position;
            manModel.transform.rotation = spawnobjectNVR.transform.rotation;
            playerWeapon_nonvr.transform.position = spawnobjectNVR.playerWeaponPos;
            playerWeapon_nonvr.transform.rotation = spawnobjectNVR.playerWeaponRot;
            //Debug.Log("Weapon Rotation: " + playerWeapon_nonvr.transform.rotation.eulerAngles);
            laserPointer_nonvr.transform.position = spawnobjectNVR.laserPointerPos;
            laserPointer_nonvr.transform.rotation = spawnobjectNVR.laserPointerRot;
            //Debug.Log("PlayerWeapon's Position: " + spawnobjectNVR.playerWeaponPos);
            //Debug.Log("PlayerWeapon's Rotation: " +  spawnobjectNVR.playerWeaponRot);

            //if (spawnobjectNVR.isPlayerShoot)
            if (spawnobjectNVR.isPlayerShoot)
            {

                // Shooting behavior
                GameObject s1 = (GameObject)Instantiate(Bullet, playerWeapon_nonvr.transform.position, playerWeapon_nonvr.transform.rotation);
                s1.GetComponent<BeamParam>().SetBeamParam(this.GetComponent<BeamParam>());
                
                GameObject wav = (GameObject)Instantiate(Wave, playerWeapon_nonvr.transform.position, playerWeapon_nonvr.transform.rotation);
                wav.transform.localScale *= 0.25f;
                wav.transform.Rotate(Vector3.left, 90.0f);
                wav.GetComponent<BeamWave>().col = this.GetComponent<BeamParam>().BeamColor;

            }

        }

        if (GameObject.FindGameObjectsWithTag("Enemy") != null && Time.time - timer > cooldown && spawnobjectNVR.isShoot)
        {
            timer = Time.time;
            GameObject[] originalEnemiesLst = GameObject.FindGameObjectsWithTag("Enemy");
            int length = originalEnemiesLst.Length;
            Debug.Log("The number of Enemy is: " + length);

            List<GameObject> newEnemiesLst_Taser = new List<GameObject>();
            foreach (GameObject enemy in originalEnemiesLst)
            {
                if (enemy.name == "Enlarged Taser") {
                    newEnemiesLst_Taser.Add(enemy);
                }
            }
            Debug.Log("The number of Tasershooter is: " + newEnemiesLst_Taser.Count);

            int index = rdIndex.Next(newEnemiesLst_Taser.Count);
            Debug.Log("Random Taser index = " + index);
            Vector3 dir = spawnobjectNVR.transform.position - newEnemiesLst_Taser[index].transform.position;
            Quaternion rotation = Quaternion.LookRotation(dir);
            GameObject s1 = (GameObject)Instantiate(Bullet, newEnemiesLst_Taser[index].transform.position, rotation);
            s1.GetComponent<BeamParam>().SetBeamParam(this.GetComponent<BeamParam>());
        }
    }
    

}
