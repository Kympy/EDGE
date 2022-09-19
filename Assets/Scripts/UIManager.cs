using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIManager : MonoBehaviourPun
{
    private TextMeshProUGUI Player_1_Name = null;
    private TextMeshProUGUI Player_2_Name = null;

    private TextMeshProUGUI MyHP = null;

    private void Awake()
    {
        Player_1_Name = GameObject.Find("1PName").GetComponent<TextMeshProUGUI>();
        Player_2_Name = GameObject.Find("2PName").GetComponent<TextMeshProUGUI>();

        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetUserName", RpcTarget.AllBuffered);
        }

    }
    [PunRPC]
    public void SetUserName()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Player_1_Name.text = "1P : " + PhotonNetwork.NickName;
        }
        else
        {
            Player_2_Name.text = "2P : " + PhotonNetwork.NickName;
        }
    }
}
