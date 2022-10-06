using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;



public class GameSceneLogic : MonoBehaviourPun
{
    GameManager gameManager = null;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    [PunRPC]
    void RPCNextScene()
    {
        Debug.Log("Scene 호출");

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

        // 프레임의 영향을 받지 않는 UnScale Time 사용
        yield return new WaitForSecondsRealtime(10f);

        // if (PhotonNetwork.IsConnected) // && PhotonNetwork.IsMasterClient

        // Scene 이동 전 GameManager 파괴
        
        if (photonView.IsMine)
        {
            gameManager.IsDestroy();
        }

        // 모든 클라이언트와 마스터 클라이언트의 LoadLevel 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log($"코루틴 호출  {PhotonNetwork.AutomaticallySyncScene}");


        if (curPlay != null)
        {
            Debug.Log(curPlay.name);
            // Destroy(curPlay.gameObject);
            PhotonNetwork.RemoveBufferedRPCs();
        }

        yield return new WaitForSeconds(1f);
        // MasterClient Scene 이동        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GunFight");
        }
    }
}
