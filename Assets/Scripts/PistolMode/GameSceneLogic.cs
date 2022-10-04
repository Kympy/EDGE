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

    [PunRPC]
    // �¸�
    // void WinUI()
    // �й�
    // void LoseUI()


    IEnumerator NextScene()
    {
        GameObject curPlay = FindObjectOfType<PlayerControl>().gameObject;

        yield return new WaitForSeconds(10f);

        // if (PhotonNetwork.IsConnected) // && PhotonNetwork.IsMasterClient

        // ��� Ŭ���̾�Ʈ�� ������ Ŭ���̾�Ʈ�� LoadLevel ����ȭ
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log($"�ڷ�ƾ ȣ��  {PhotonNetwork.AutomaticallySyncScene}");


        if (curPlay != null)
        {
            Debug.Log(curPlay.name);
            // Destroy(curPlay.gameObject);
            PhotonNetwork.RemoveBufferedRPCs();
        }

        yield return new WaitForSeconds(1f);
        // MasterClient Scene �̵�        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GunFight");
        }
    }


}
