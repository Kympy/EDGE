using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ServerLogin : MonoBehaviourPunCallbacks
{
    public Button button;
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(); // Applicate Connection to Master Server
        button.onClick.AddListener(() => LoginStart());
    }
    private void LoginStart()
    {
        PhotonNetwork.NickName = "TestUser";
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 4 }, null); // Check Room and Join or Create
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.LoadLevel(1);
    }
}
