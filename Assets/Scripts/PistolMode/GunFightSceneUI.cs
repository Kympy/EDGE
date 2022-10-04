using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;

public class GunFightSceneUI : MonoBehaviourPun
{
    [SerializeField]
    TextMeshProUGUI resultWin;
    [SerializeField]
    TextMeshProUGUI resultLose;

    bool isWin = false;
    bool isLose = false;

    public void ResultWin()
    {
        isWin = true;
        if (isWin)
        {
            resultWin.enabled = true;
        }
    }

    [PunRPC]
    void ResultLose()
    {
        isLose = true;
        if (isLose)
        {
            resultLose.enabled = true;
        }
    }

}
