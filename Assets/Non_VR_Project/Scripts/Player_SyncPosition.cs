using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player_SyncPosition : NetworkBehaviour  // We don't use this script yet (This is the self-made position intepolation script)
{
    [SyncVar]
    private Vector3 syncPos;

    [SerializeField]Transform myTransform;
    [SerializeField]float lerpRate = 15;

    private void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
    }
    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
            syncPos = pos;        
    }

    [Client]
    void TransmitPosition()
    {
        //if (isLocalPlayer)
        //{
            CmdProvidePositionToServer(myTransform.position);

        //}
    }

}
