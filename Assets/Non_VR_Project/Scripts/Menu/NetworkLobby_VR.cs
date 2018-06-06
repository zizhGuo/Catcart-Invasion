using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

public class NetworkLobby_VR : NetworkManager {

    static public NetworkLobby_VR _singleton;

    [HideInInspector]
    public bool _isMatchmaking = false;
    protected bool _disconnectServer = false;
    protected ulong _currentMatchID;
    // Use this for initialization
    void Start () {
        _singleton = this;
        StartMatchMaker();
        matchMaker.CreateMatch("catcart",
                4,
                true,
                "", "", "", 0, 0,
                OnMatchCreate);
        _isMatchmaking = true;
        Debug.Log("MatchMaking status: " + _isMatchmaking);
    }
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        StartHost(matchInfo);
        _currentMatchID = (System.UInt64)matchInfo.networkId;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isMatchmaking)
            {
                StopHost();
                matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
                _disconnectServer = true;
                Application.Quit();
            }
        }
	}
}
