using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;

public class GunFightSceneUI : MonoBehaviourPun
{
    [SerializeField]
    GameObject resultWin;
    [SerializeField]
    GameObject resultLose;

    bool isWin = false;
    bool isLose = false;

    public void ResultWin()
    {
        isWin = true;
        if (isWin)
        {
            resultWin.SetActive(true);
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
    }
}
