using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class test : MonoBehaviourPun
{
    // GUI�� code�� 
    private void OnGUI()
    {   
        // ���� ����, ���� Ȯ��
        GUI.Label(new Rect(20f,50f,200f,20f), "���� ���� : " + PhotonNetwork.NetworkClientState.ToString());
        GUI.Label(new Rect(20f,70f,200f,20f), "���� ���� : " + PhotonNetwork.Server.ToString());
        GUI.Label(new Rect(20f,90f,200f,20f), "������ ���� : " + PhotonNetwork.IsMasterClient.ToString());
        GUI.Label(new Rect(20f,110f,200f,20f), "�� - ������ �� : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        GUI.Label(new Rect(20f,130f,200f,20f), "�� : " + PhotonNetwork.InRoom.ToString());
        GUI.Label(new Rect(20f,150f,200f,20f), "���� ���� : " + PhotonNetwork.PhotonServerSettings.DevRegion.ToString());        
    }
}
