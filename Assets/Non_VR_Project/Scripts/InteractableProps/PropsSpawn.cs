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
        if (this.connectionToClient != null) {
            Debug.Log("connectionToClient != null");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //CmdSpawnProps();
        }
        if (GameObject.Find("Non-VR player") && NVRPlayer == null) {
            NVRPlayer = GameObject.Find("Non-VR player");
            CmdSpawnProps(NVRPlayer);
        }
    }

    [Command]
    void CmdSpawnProps(GameObject player) {
        Vector3 pos = new Vector3(-750f, 3f, 0f);
        GameObject prop = Instantiate(testPropPrefab, pos, Quaternion.identity);
        //NetworkServer.Spawn(prop);
        NetworkServer.SpawnWithClientAuthority(prop, player);
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

        //GameObject prop = Instantiate(testPropPrefab, spawnPosition.transform.position, Quaternion.identity);
        //NetworkServer.Spawn(prop);
        //NetworkServer.SpawnWithClientAuthority(prop, this.connectionToClient)
        //CmdSpawnProps();
        Debug.Log("OnStartServer!");
    }
    //public void OnClientConnect()
    //{
    //    CmdSpawnProps();
    //    Debug.Log("OnStartServer!");
    //}
    public override void OnStartAuthority()
    {
        //CmdSpawnProps();
        Debug.Log("OnStartAuthority!");
    }
    //Coroutine SpawnProcess()
}
