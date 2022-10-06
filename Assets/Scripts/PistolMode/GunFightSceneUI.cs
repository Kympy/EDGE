using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

using Photon.Pun;

public class GunFightSceneUI : Singleton<GunFightSceneUI>
{
    [SerializeField]
    GameObject resultWin = null;
    [SerializeField]
    GameObject resultLose = null;
    [SerializeField]
    GameObject reload = null;

    [SerializeField]
    GameObject[] WinIcon = new GameObject[2];

    TextMeshPro ReloadText = null;

    bool isWin = false;
    bool isLose = false;

    private void Awake()
    {
        for (int i = 0; i < WinIcon.Length; i++)
        {
            WinIcon[i].SetActive(false);
        }

        if (GameManager.Instance.WinCount == 1)
        {
            WinIcon[0].SetActive(true);
        }
        else if (GameManager.Instance.WinCount >= 2)
        {
            WinIcon[0].SetActive(true);
            WinIcon[1].SetActive(true);
        }

        ReloadText = gameObject.GetComponent<TextMeshPro>();
    }

    void Start()
    {
        reload.SetActive(false);
    }

    public void ResultWin()
    {
        resultWin.SetActive(true);
    }
    [PunRPC]
    public void ResultLose()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        resultLose.SetActive(true);
    }

    // Bullet Reload
    public IEnumerator Reload()
    {
        int Count = 0;
        while (Count < 2)
        {
            reload.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            reload.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            Count++;
        }
    }

    [PunRPC]
    public void RPC_ReloadScene()
    {
        StartCoroutine(reloadScene());
    }

    // GunFight ReloadScene
    IEnumerator reloadScene()
    {
        yield return new WaitForSeconds(3f);

        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("RoomScene");
        }
    }

    public void WinCountUI()
    {
        ResultWin();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        switch (GameManager.Instance.WinCount)
        {
            case 1:
                WinIcon[0].SetActive(true);
                photonView.RPC("RPC_ReloadScene", RpcTarget.All);

                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(ODINAPIHandler.Instance.ProcessRequestDeclareWinner(ODINAPIHandler.COIN_TYPE.zera, ODINAPIHandler.Winner.Me));
                }

                else
                {
                    gameObject.GetComponent<PhotonView>().RPC("TransODINAPIHandlerOder", RpcTarget.MasterClient);
                }

                break;

            case 2:
                WinIcon[1].SetActive(true);
                photonView.RPC("GameEnd", RpcTarget.All);
                break;

            default:
                {
                    Debug.Log("Error");
                    break;
                }
        }

    }

    [PunRPC]
    void TransODINAPIHandlerOder()
    {
        Debug.Log("호출 당함");
        StartCoroutine(ODINAPIHandler.Instance.ProcessRequestDeclareWinner(ODINAPIHandler.COIN_TYPE.zera, ODINAPIHandler.Winner.Other));
    }

    [PunRPC]
    public void GameEnd()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(moveMainLobby());
        }
    }

    [PunRPC]
    void MoveMainLobby()
    {
        StartCoroutine(moveMainLobby());
    }

    IEnumerator moveMainLobby()
    {
        yield return new WaitForSecondsRealtime(4f);

        PhotonNetwork.LoadLevel("MainLobby");
    }
}
