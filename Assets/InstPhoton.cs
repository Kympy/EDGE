using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InstPhoton : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if(PhotonNetwork.IsConnected)
        PhotonNetwork.Instantiate("Player", Vector3.one, Quaternion.identity);
    }
}
