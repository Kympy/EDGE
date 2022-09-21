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
        // 포톤 기본설정을 가지고 마스터 서버 연결
        PhotonNetwork.ConnectUsingSettings();

        // 버튼 기능
        login = GameObject.Find("LogButton").GetComponent<Button>();

        // 버튼 기능추가 (람다 or 델리게이트)
        // 람다식 : 인자 전달 X
        // login.onClick.AddListener(() => LoginButton());

        // 델리게이트 : 인자 전달 가능
/*
        login.onClick.AddListener(delegate
        {  // 사용자 닉네임 설정
            PhotonNetwork.NickName = "User1";
            // 방 설정
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
        });
*/
        // 위와 같음        
        login.onClick.AddListener(delegate { LoginButton(); });
    }

    void LoginButton()
    {
        // 사용자 닉네임 설정
        PhotonNetwork.NickName = "User1";
        // 방 설정
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        // 둘다 같음
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
