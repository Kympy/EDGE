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
            // ��� Ŭ���̾�Ʈ�� ������ Ŭ���̾�Ʈ�� LoadLevel ����ȭ
            PhotonNetwork.AutomaticallySyncScene = true;

            // MasterClient Scene �̵�
            PhotonNetwork.LoadLevel("GunFight");
        }        
    }
}
