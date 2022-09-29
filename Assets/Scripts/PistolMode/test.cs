using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class test : MonoBehaviourPun
{
    // GUI를 code로 
    private void OnGUI()
    {   
        // 연결 상태, 서버 확인
        GUI.Label(new Rect(20f,50f,200f,20f), "연결 상태 : " + PhotonNetwork.NetworkClientState.ToString());
        GUI.Label(new Rect(20f,70f,200f,20f), "현재 서버 : " + PhotonNetwork.Server.ToString());
        GUI.Label(new Rect(20f,90f,200f,20f), "마스터 여부 : " + PhotonNetwork.IsMasterClient.ToString());
        GUI.Label(new Rect(20f,110f,200f,20f), "방 - 접속자 수 : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        GUI.Label(new Rect(20f,130f,200f,20f), "방 : " + PhotonNetwork.InRoom.ToString());
        GUI.Label(new Rect(20f,150f,200f,20f), "접속 지역 : " + PhotonNetwork.PhotonServerSettings.DevRegion.ToString());        
    }
}
