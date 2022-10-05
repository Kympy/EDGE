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
        isWin = true;
        if (isWin && (GameManager.Instance.MasterWinCount < 2 || GameManager.Instance.ClientWinCount < 2))
        {
            resultWin.SetActive(true);
        }
        StartCoroutine(reloadScene());
    }

    public void ResultLose()
    {
        Debug.Log("ResultLose ȣ��");
        isLose = true;
        if (isLose)
        {
            Debug.Log("ResultLose False");
            resultLose.SetActive(true);
        }
        StartCoroutine(reloadScene());
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


    // GunFight ReloadScene
    IEnumerator reloadScene()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (GameManager.Instance.MasterWinCount == 2 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("�κ� ��!");
            //PhotonNetwork.LoadLevel("MainLobby");
            StartCoroutine(moveMainLobby());
        }

        else if (GameManager.Instance.ClientWinCount == 2 && PhotonNetwork.IsMasterClient == false)
        {
            gameObject.GetComponent<PhotonView>().RPC("MoveMainLobby", RpcTarget.MasterClient);
        }

        yield return new WaitForSeconds(3f);

        isWin = false;
        isLose = false;
        reload.SetActive(false);
        resultWin.SetActive(false);
        resultLose.SetActive(false);

        if (GameManager.Instance.ClientWinCount <2 && GameManager.Instance.MasterWinCount < 2 && PhotonNetwork.IsMasterClient)
        {
            // PhotonNetwork.LoadLevel("GunFight");
            PhotonNetwork.LoadLevel("Loading");
        }
    }

    public void WinCountUI(int WinCount)
    {
        switch (WinCount)
        {
            case 1:
                WinIcon[0].SetActive(true);
                break;

            case 2:
                WinIcon[1].SetActive(true);
                break;

            default:
                break;
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
