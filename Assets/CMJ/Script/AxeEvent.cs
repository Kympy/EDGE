using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class AxeEvent : MonoBehaviourPunCallbacks
{
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Awake()
    {
        // 포톤 기본설정을 가지고 마스터 서버 연결함
        PhotonNetwork.ConnectUsingSettings();

        // 하이어라키에 존재하는 오브젝트 이름으로 검색
        button = GameObject.Find("Button").GetComponent<Button>();

        // 버튼에 기능을 추가함 (델리게이트 또는 람다식 사용)

        // 람다식
        button.onClick.AddListener(() => LoginButton());

        // 델리게이트 (생성한 함수의 내용을 {}를 통해 무명함수를 만듦)
/*        button.onClick.AddListener(delegate { PhotonNetwork.NickName = "AXE"; PhotonNetwork.JoinOrCreateRoom("AxeRoom", new RoomOptions { MaxPlayers = 2 }, null); });*/
       
    }

    // 버튼에 연결 할 함수
    void LoginButton()
    {
        PhotonNetwork.NickName = "AXE";

        // 방 만들기 
        // 1. 방 이름, 2. 방 옵션(인원 등)
        PhotonNetwork.JoinOrCreateRoom("AxeRoom", new RoomOptions { MaxPlayers = 2}, null );
    }

    public override void OnJoinedRoom()
    {
        // 빌드 넘버를 호출함
        PhotonNetwork.LoadLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
