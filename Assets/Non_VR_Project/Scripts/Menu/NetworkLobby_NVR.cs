using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

public class NetworkLobby_NVR : NetworkManager {

    static public NetworkLobby_NVR _singleton;

    [HideInInspector]
    public bool _isMatchmaking = false;
    protected bool _disconnectServer = false;
    protected ulong _currentMatchID;
    NetworkID networkIDforCatcart;
    public bool _connecting = false;
    // Use this for initialization
    void Start () {
        _singleton = this;
        StartMatchMaker();
        matchMaker.ListMatches(0, 6, "", true, 0, 0, CheckMatchList);
        //Debug.Log("Gotten ID: " + (System.UInt64)networkIDforCatcart);

    }
    public void CheckMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
        for (int i = 0; i < matches.Count; ++i)
        {
            if (matches[i].name == "catcart")
            {
                networkIDforCatcart = matches[i].networkId;
               // Debug.Log("Gotten ID: " + (System.UInt64)networkIDforCatcart);
                _isMatchmaking = true;
                matchMaker.JoinMatch(networkIDforCatcart, "", "", "", 0, 0, HandleJoinedMatch);
                break;
            }
        }

    }
    public void HandleJoinedMatch(bool success, string extendedinfo, MatchInfo responsedata) {
        StartClient(responsedata);
        //Debug.Log("Joiend ID: " + (System.UInt64)responsedata.networkId);
    }

    //public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    //{
    //    this.OnMatchJoined(success, extendedInfo, matchInfo);
    //    StartClient(matchInfo);
    //    //_currentMatchID = (System.UInt64)matchInfo.networkId;
    //}
    // Update is called once per frame

    void Update () {
        //if (_isMatchmaking && !_connecting)
        //{
        //    matchMaker.JoinMatch(networkIDforCatcart, "", "", "", 0, 0, OnMatchJoined);
        //    _connecting = true;
        //}


        //if (!_isMatchmaking) {
        //    matchMaker.ListMatches(0, 6, "", true, 0, 0, CheckMatchList);
        //    _connecting = true;
        //}
        //if (_connecting)
        //{
        //    matchMaker.JoinMatch(networkIDforCatcart, "", "", "", 0, 0, OnMatchJoined);
        //    _connecting = false;
        //}



        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isMatchmaking)
            {
                StopClient();
                StopMatchMaker();
                //matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
                _disconnectServer = true;
                Application.Quit();
            }
        }
    }
}
