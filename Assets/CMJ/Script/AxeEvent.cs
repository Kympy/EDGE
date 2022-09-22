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
        // ���� �⺻������ ������ ������ ���� ������
        PhotonNetwork.ConnectUsingSettings();

        // ���̾��Ű�� �����ϴ� ������Ʈ �̸����� �˻�
        button = GameObject.Find("Button").GetComponent<Button>();

        // ��ư�� ����� �߰��� (��������Ʈ �Ǵ� ���ٽ� ���)

        // ���ٽ�
        button.onClick.AddListener(() => LoginButton());

        // ��������Ʈ (������ �Լ��� ������ {}�� ���� �����Լ��� ����)
/*        button.onClick.AddListener(delegate { PhotonNetwork.NickName = "AXE"; PhotonNetwork.JoinOrCreateRoom("AxeRoom", new RoomOptions { MaxPlayers = 2 }, null); });*/
       
    }

    // ��ư�� ���� �� �Լ�
    void LoginButton()
    {
        PhotonNetwork.NickName = "AXE";

        // �� ����� 
        // 1. �� �̸�, 2. �� �ɼ�(�ο� ��)
        PhotonNetwork.JoinOrCreateRoom("AxeRoom", new RoomOptions { MaxPlayers = 2}, null );
    }

    public override void OnJoinedRoom()
    {
        // ���� �ѹ��� ȣ����
        PhotonNetwork.LoadLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
