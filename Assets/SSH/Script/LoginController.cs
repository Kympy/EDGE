using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;


public class LoginController : MonoBehaviourPunCallbacks
{
    Button login;

    void Awake()
    {
        // ���� �⺻������ ������ ������ ���� ����
        PhotonNetwork.ConnectUsingSettings();

        // ��ư ���
        login = GameObject.Find("LogButton").GetComponent<Button>();

        // ��ư ����߰� (���� or ��������Ʈ)
        // ���ٽ� : ���� ���� X
        // login.onClick.AddListener(() => LoginButton());

        // ��������Ʈ : ���� ���� ����
/*
        login.onClick.AddListener(delegate
        {  // ����� �г��� ����
            PhotonNetwork.NickName = "User1";
            // �� ����
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
        });
*/
        // ���� ����        
        login.onClick.AddListener(delegate { LoginButton(); });
    }

    void LoginButton()
    {
        // ����� �г��� ����
        PhotonNetwork.NickName = "User1";
        // �� ����
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        // �Ѵ� ����
        // PhotonNetwork.LoadLevel(1);
        PhotonNetwork.LoadLevel("Lobby");        
    }



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
