using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


public class GameSceneLogic : MonoBehaviourPunCallbacks
{
    public void LobbyPos()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            Vector3 MasterPos = new Vector3(-1.5f, 0.5f, 1.5f);

            PhotonNetwork.Instantiate("Cowboy", MasterPos, Quaternion.identity);
        }

        else if (PhotonNetwork.IsConnected)
        {
            Vector3 ClientPos = new Vector3(0, -0.9f, -27);

            PhotonNetwork.Instantiate("Cowboy", ClientPos, Quaternion.identity);
        }
    }

    public void GunFightPos()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            Vector3 MasterPos = new Vector3(0, -0.5f, -10);

            PhotonNetwork.Instantiate("Cowboy", MasterPos, Quaternion.identity);
        }

        else if (PhotonNetwork.IsConnected)
        {
            Vector3 ClientPos = new Vector3(-6.5f, 0, 5.5f);

            PhotonNetwork.Instantiate("Cowboy", ClientPos, Quaternion.Euler(0f,125,0));
        }
    }
}
