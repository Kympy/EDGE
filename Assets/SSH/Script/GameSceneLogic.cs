using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class GameSceneLogic : MonoBehaviourPun
{
    [PunRPC]
    void NextScene()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            // 모든 클라이언트와 마스터 클라이언트의 LoadLevel 동기화
            PhotonNetwork.AutomaticallySyncScene = true;

            // MasterClient Scene 이동
            PhotonNetwork.LoadLevel("GunFight");
        }        
    }
}
