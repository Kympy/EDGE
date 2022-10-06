using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;



public class GameSceneLogic : MonoBehaviourPun
{
    GameManager gameManager = null;

    private void Awake()
    {
        gameManager = GameObject.Find("LobbyManager").GetComponent<GameManager>();

        if (PhotonNetwork.IsMasterClient)
        {
            ODINAPIHandler.Instance.ProcessBettingCoin(ODINAPIHandler.COIN_TYPE.zera);
        }
    }

    

    [PunRPC]
    void RPCNextScene()
    {
        Debug.Log("Scene ȣ��");

        StartCoroutine(NextScene());
    }
    /*
        [PunRPC]
        void RPCReloadScene()
        {

        }
    */

    IEnumerator NextScene()
    {
        GameObject curPlay = FindObjectOfType<PlayerControl>().gameObject;

        // �������� ������ ���� �ʴ� UnScale Time ���
        yield return new WaitForSecondsRealtime(10f);
                
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
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GunFight");
        }
    }
}
