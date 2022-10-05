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
        PhotonNetwork.AutomaticallySyncScene = true;

        isWin = true;
        if (isWin && GameManager.Instance.WinCount < 2)
        {
            resultWin.SetActive(true);
        }
        StartCoroutine(reloadScene());
    }

    public void ResultLose()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log("ResultLose 호출");
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
        if (GameManager.Instance.WinCount == 2 && PhotonNetwork.IsMasterClient)
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
            // PhotonNetwork.LoadLevel("GunFight");
            PhotonNetwork.LoadLevel("Loading");
            Debug.Log($"다음 라운드 WinCount : {GameManager.Instance.WinCount}");
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
}
