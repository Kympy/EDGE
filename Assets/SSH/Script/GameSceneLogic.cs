using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;


public class GameSceneLogic : MonoBehaviourPun
{
    [PunRPC]
    void RPCScene()
    {
        Debug.Log("Scene ȣ��");

        StartCoroutine(NextScene());        
    }

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(10f);

       // if (PhotonNetwork.IsConnected) // && PhotonNetwork.IsMasterClient
        
            // ��� Ŭ���̾�Ʈ�� ������ Ŭ���̾�Ʈ�� LoadLevel ����ȭ
            PhotonNetwork.AutomaticallySyncScene = true;

            Debug.Log($"�ڷ�ƾ ȣ��  {PhotonNetwork.AutomaticallySyncScene}");

            // MasterClient Scene �̵�
            PhotonNetwork.LoadLevel("GunFight");
        
    }
}
