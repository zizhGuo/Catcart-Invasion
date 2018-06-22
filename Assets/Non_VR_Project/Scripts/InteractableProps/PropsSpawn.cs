using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PropsSpawn : NetworkBehaviour
{
    [SerializeField]
    private GameObject testPropPrefab;
    [SerializeField]
    private GameObject spawnPosition;

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
        }
    }

    [Command]
    void CmdSpawnProps(GameObject player, GameObject[] spawners) {

        for (int i = 0; i < spawners.Length; i++) {
            Vector3 pos = spawners[i].transform.position;
            GameObject prop = Instantiate(testPropPrefab, pos, spawnPosition.transform.rotation);
            NetworkServer.SpawnWithClientAuthority(prop, player);
        }
        //Vector3 pos = spawnPosition.transform.position;
        //GameObject prop = Instantiate(testPropPrefab, pos, spawnPosition.transform.rotation);
        //NetworkServer.Spawn(prop);
        //NetworkServer.SpawnWithClientAuthority(prop, player);
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
