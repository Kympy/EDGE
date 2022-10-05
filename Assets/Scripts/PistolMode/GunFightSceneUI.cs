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
        DontDestroyOnLoad(this.gameObject);
        ReloadText = gameObject.GetComponent<TextMeshPro>();
    }

    void Start()
    {
        reload.SetActive(false);

        for (int i = 0; i < WinIcon.Length; i++)
        {
            WinIcon[i].SetActive(false);
        }
    }

    public void ResultWin()
    {
        resultWin.SetActive(true);
    }
    [PunRPC]
    public void ResultLose()
    {
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

        isWin = false;
        isLose = false;
        reload.SetActive(false);
        resultWin.SetActive(false);
        resultLose.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Loading");
        }
    }

    public void WinCountUI()
    {
        ResultWin();
        switch (GameManager.Instance.WinCount)
        {
            case 1:
                WinIcon[0].SetActive(true);
                photonView.RPC("RPC_ReloadScene", RpcTarget.All);
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
    public void GameEnd()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if(PhotonNetwork.IsMasterClient)
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
