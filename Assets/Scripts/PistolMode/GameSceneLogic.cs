using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;



public class GameSceneLogic : MonoBehaviourPun
{
    [PunRPC]
    void RPCScene()
    {
        Debug.Log("Scene 호출");

        StartCoroutine(NextScene());
    }

    [PunRPC]
    // 승리
    // void WinUI()
    // 패배
    // void LoseUI()


    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(10f);

        // if (PhotonNetwork.IsConnected) // && PhotonNetwork.IsMasterClient

        // 모든 클라이언트와 마스터 클라이언트의 LoadLevel 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log($"코루틴 호출  {PhotonNetwork.AutomaticallySyncScene}");

        GameObject curPlay = FindObjectOfType<PlayerControl>().gameObject;

        if (curPlay != null)
        {
            Debug.Log(curPlay.name);
            Destroy(curPlay.gameObject);
        }

        yield return new WaitForSeconds(1f);
        // MasterClient Scene 이동        

        PhotonNetwork.LoadLevel("GunFight");

    }


}
