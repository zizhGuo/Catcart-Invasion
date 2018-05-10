using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;

public class SetupLocalPlayers : NetworkBehaviour
{
    public GameManager gameManager; // The game manager
    public CameraMoving camerasMoving;

    private void Start()
    {
        string clientName = "Client side";
        string serverName = "Server side";
        //CmdSendName(serverName);
        //Cursor.visible = false;
        if (!isServer && isLocalPlayer) // non Server
        {
            CmdSendName(clientName);
        }
        if (isServer && isLocalPlayer) RpcSendNameToClient(serverName);
        //if (isLocalPlayer) 
        //else          // Server
        //{
        //    CmdSendName(serverName);
        //    RpcSendNameToClient(serverName);
        //}
        // else RpcSendNameToClient("VR(ServerSide)");
        if (!isLocalPlayer && gameManager != null)
        {
            CmdSendName(serverName);
        }

        if (!isLocalPlayer && FindObjectOfType<CameraMoving>())
        {
            transform.GetChild(3).gameObject.SetActive(true);
            GetComponent<MeshRenderer>().enabled = false;
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            //transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
        }
        if (isLocalPlayer && FindObjectOfType<CameraMoving>())
        {
            transform.GetChild(2).GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void Update()
    {
        
        // Update the position of local player.
        if (isLocalPlayer && gameManager != null) // If this is running on the VR side
        {
            transform.position = gameManager.playerKart.transform.position; // VR player's position
            transform.rotation = gameManager.playerKart.transform.rotation;
        }
        if (isLocalPlayer && camerasMoving) // If this is running on the non-VR side
        {
            transform.position = camerasMoving.GetComponent<ObjectsPlacer>().spawnPosition; // Non-VR player's position
        }
    }

    public override void OnStartLocalPlayer() // Define VR player and Non-VR player.
    {
        base.OnStartLocalPlayer();

        if (FindObjectOfType<GameManager>())
        {
            gameManager = FindObjectOfType<GameManager>();    // Define this player as the VR player.
            LocalIPAddress();
        }
        if (FindObjectOfType<CameraMoving>())
        {
            camerasMoving = FindObjectOfType<CameraMoving>(); // Define this player as the Non-VR player.
        }
    }
    [Command]
    void CmdSendName(string name)
    {
        RpcSendNameToClient(name);
        //transform.name = name;
    }

    [ClientRpc]
    void RpcSendNameToClient(string name)
    {
        transform.name = name;
    }
    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }

        print(localIP);
        return localIP;
    }
}
