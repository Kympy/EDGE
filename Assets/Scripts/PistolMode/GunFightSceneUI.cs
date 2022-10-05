using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;

public class GunFightSceneUI : MonoBehaviourPun
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

    int WinCount = 0;

    private void Awake()
    {
        ReloadText = gameObject.GetComponent<TextMeshPro>();

        // Win Score
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        reload.SetActive(false);
    }

    public void ResultWin()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        isWin = true;
        if (isWin && WinCount < 3)
        {
            WinCount++;
            resultWin.SetActive(true);
        }
        StartCoroutine(reloadScene());
    }

    [PunRPC]
    void ResultLose()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log("ResultLose È£Ãâ");
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
        if (WinCount == 3 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainLobby");
        }

        yield return new WaitForSeconds(3f);

        isWin = false;
        isLose = false;
        reload.SetActive(false);
        resultWin.SetActive(false);
        resultLose.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GunFight");
        }
    }

    void WinCountUI()
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
}
