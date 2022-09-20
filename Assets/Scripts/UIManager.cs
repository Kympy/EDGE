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

    private SniperControl MyPlayer = null;

    private void Start()
    {
        if (photonView.IsMine == false) return;

        InitUserName();
        MyPlayer = FindObjectOfType<SniperControl>();
        InitUserHP();
    }
    public void InitUserHP()
    {
        MyHP = GameObject.Find("HPInfo").GetComponent<TextMeshProUGUI>();
        MyHP.text = MyPlayer.CurrentHP + " / " + MyPlayer.Max_HP;
    }
    public void InitUserName()
    {
        Player_1_Name = GameObject.Find("1PName").GetComponent<TextMeshProUGUI>();
        Player_2_Name = GameObject.Find("2PName").GetComponent<TextMeshProUGUI>();

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetUserName", RpcTarget.AllBuffered, 1, PhotonNetwork.NickName);
        }
        else
        {
            photonView.RPC("SetUserName", RpcTarget.AllBuffered, 2, PhotonNetwork.NickName);
        }
    }
    [PunRPC]
    public void SetUserName(int Number, string Nickname)
    {
        if(Number == 1)
        {
            Player_1_Name.text = "1P : " + Nickname;
        }
        else
        {
            Player_2_Name.text = "2P : " + Nickname;
        }
    }
}
