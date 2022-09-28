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
        yield return new WaitForSeconds(10f);

        // if (PhotonNetwork.IsConnected) // && PhotonNetwork.IsMasterClient

        // ��� Ŭ���̾�Ʈ�� ������ Ŭ���̾�Ʈ�� LoadLevel ����ȭ
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log($"�ڷ�ƾ ȣ��  {PhotonNetwork.AutomaticallySyncScene}");

        GameObject.FindObjectOfType<PlayerControl>().gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        // MasterClient Scene �̵�        

        PhotonNetwork.LoadLevel("GunFight");

    }


}
