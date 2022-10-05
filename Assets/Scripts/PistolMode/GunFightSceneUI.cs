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

    TextMeshPro ReloadText = null;
    
    bool isWin = false;
    bool isLose = false;
    
    int Count = 0;

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
        isWin = true;
        if (isWin)
        {
            resultWin.SetActive(true);

        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(reloadScene());
        }
    }

    [PunRPC]
    void ResultLose()
    {
        Debug.Log("ResultLose »£√‚");
        isLose = true;
        if (isLose)
        {
            Debug.Log("ResultLose False");
            resultLose.SetActive(true);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(reloadScene());
        }
    }

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
        Count++;

        PhotonNetwork.AutomaticallySyncScene = true;

        yield return new WaitForSeconds(3);
    }
}
