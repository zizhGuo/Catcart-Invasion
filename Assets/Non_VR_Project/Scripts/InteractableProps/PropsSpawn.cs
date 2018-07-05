using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PropsSpawn : NetworkBehaviour
{
    [SerializeField]
    private GameObject testPropPrefab;
    [SerializeField]
    private GameObject TrafficLightsPrefab;
    [SerializeField]
    private GameObject HummerPrefab;

    GameObject NVRPlayer = null;

    NetworkPlayer client;

    private int playerCount = 0;

    void Start () {
        

    }

    void Update()
    {
        if (GameObject.Find("Non-VR player") && NVRPlayer == null) {
            NVRPlayer = GameObject.Find("Non-VR player");
            GameObject[] Spawners = GameObject.FindGameObjectsWithTag("InteractableSpawner");
            CmdSpawnProps(NVRPlayer, Spawners);

            GameObject[] Spawners_Traffic = GameObject.FindGameObjectsWithTag("InteractableTrafficSpawner");
            CmdSpawnProps_Traffic(NVRPlayer, Spawners_Traffic);


        }
    }

    [Command]
    void CmdSpawnProps(GameObject player, GameObject[] spawners) {

        for (int i = 0; i < spawners.Length; i++) {
            Vector3 pos = spawners[i].transform.position;
            Quaternion rot = spawners[i].transform.rotation;
            GameObject prop = Instantiate(testPropPrefab, pos, rot);
            NetworkServer.SpawnWithClientAuthority(prop, player);
        }
        //Vector3 pos = spawnPosition.transform.position;
        //GameObject prop = Instantiate(testPropPrefab, pos, spawnPosition.transform.rotation);
        //NetworkServer.Spawn(prop);
        //NetworkServer.SpawnWithClientAuthority(prop, player);
    }
    [Command]
    void CmdSpawnProps_Traffic(GameObject player, GameObject[] spawners)
    {
        //Debug.Log("tRY TO SPAWN traiffic lights");
        for (int i = 0; i < spawners.Length; i++)
        {
            Vector3 pos = spawners[i].transform.position;
            Quaternion rot = spawners[i].transform.rotation;
            GameObject prop = Instantiate(TrafficLightsPrefab, pos, rot);
            NetworkServer.SpawnWithClientAuthority(prop, player);

            Vector3 posCar = spawners[i].transform.GetChild(0).transform.position;
            Quaternion rotCar = spawners[i].transform.GetChild(0).transform.rotation;
            GameObject Car = Instantiate(HummerPrefab, posCar, rotCar);          
            //Car.transform.parent = prop.transform;           
            NetworkServer.SpawnWithClientAuthority(Car, player);
            //CmdMakeAsChild(Car, Car.transform.parent.gameObject);

            //var CarID = Car.GetComponent<NetworkIdentity>().netId;
            //var PropID = prop.GetComponent<NetworkIdentity>().netId;

            //ClientScene.objects[CarID].transform.parent = ClientScene.objects[PropID].transform;
            //RpcMakeAsChild123(Car.GetComponent<NetworkIdentity>(), prop.GetComponent<NetworkIdentity>());
            //RpcMakeAsChild123(Car.GetComponent<NetworkIdentity>(), Car.transform.parent.gameObject.GetComponent<NetworkIdentity>());
            //RpcShowNetID();
            //RpcMakeAsChild(Car, Car.transform.parent.gameObject);
            //GameGameObject.Find
        }
    }
    [Command]
    void CmdMakeAsChild(GameObject a, GameObject b)
    {
        a.transform.parent = b.transform;
        //RpcMakeAsChild(a, a.transform.parent.gameObject);
    }
    [ClientRpc]
    void RpcMakeAsChild(GameObject a, GameObject b)
    {
        a.transform.parent = b.transform;
        Debug.Log("Link to gameobject together!");
    }
    [ClientRpc]
    void RpcMakeAsChild123(NetworkIdentity a, NetworkIdentity b)
    {
        ClientScene.objects[a.netId].transform.parent = ClientScene.objects[b.netId].transform;
    }
    [ClientRpc]
    void RpcShowNetID() {
        Debug.Log("123");
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("OnPlayerConnected!");
        //Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
    }

    public override void OnStartClient()
    {
        Debug.Log("OnStartClient!");
    }
    public override void OnStartServer() {

        Debug.Log("OnStartServer!");
    }
    //public void OnClientConnect()
    //{
    //    CmdSpawnProps();
    //    Debug.Log("OnStartServer!");
    //}
    public override void OnStartAuthority()
    {
        Debug.Log("OnStartAuthority!");
    }
}
