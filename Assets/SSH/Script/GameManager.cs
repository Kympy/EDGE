using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks
{
    GameSceneLogic gameSceneLogic;
    PlayerControl playerControl;

    [SerializeField] Transform MPos;
    [SerializeField] Transform CPos;

    bool LoginMaster = false;
    bool LoginClient = false;

    public int myViewID = 0;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            // Debug.Log("LobbyPos");
            LobbyPos();

            // Master�� Client �� �� ���� ���� ��� GunFight Scene���� �̵��ϴ� �Լ� ȣ��
            if (PhotonNetwork.IsConnected && LoginClient && LoginMaster && PhotonNetwork.IsMasterClient)
            {
                // RpcTarget.MasterClient�� ��� ���� ������ NextScene���� 
                // MasterClient Ȯ�� ���ʿ�?
                photonView.RPC("NextScene", RpcTarget.MasterClient);
            }
        }

        else if(SceneManager.GetActiveScene().name == "GunFight")
        {
            // Debug.Log("GunFightPos");
            GunFightPos();
        }

        // ���� ViewID�� �ش��ϴ� ������Ʈ ã��
        if (photonView.IsMine)
        {
            // PlayerControl[] : room�� ����� �÷��̾��� ������ �ӽ÷� ������� ������� 
            PlayerControl[] playerList;

            // PlayerControl ��ũ��Ʈ�� �پ��ִ� GameObject������Ʈ�� ã��
            playerList = GameObject.FindObjectsOfType<PlayerControl>();
            // PlayerControl �迭�� ���������� Ž���ϸ鼭 foreach�� ó��
            foreach (PlayerControl pC in playerList)
            {
                // ������ �Էµ� viewID�� �� playerControl�� ����� ViewID�� ���Ͽ� �� ViewID ����
                if (pC.gameObject.GetPhotonView().ViewID == myViewID)
                {
                    playerControl = pC;
                    Debug.Log("photonView.IsMine �εε���");
                }
            }
            // Cowboy��� �̸��� ���� GameObject�� ã�� GetComponent<PlayerControl>
            // ���� : prefab���� ���� �� �̸��� Name(Clone)���� ����� ã������  
            // playerList = GameObject.Find("Cowboy").GetComponent<PlayerControl>();
        }


        // GameSceneLogic��� �̸��� ���� GameObject�� ã�� GameSceneLogic Component�� �޾ƿ�
        gameSceneLogic = GameObject.Find("GameSceneLogic").GetComponent<GameSceneLogic>();
    }

    void LobbyPos()
    {
        Vector3 Pos = Vector3.zero;

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            Pos = MPos.position;
            LoginMaster = true;
        }

        else if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient == false)
        {
            Pos = CPos.position;
            LoginClient = true;
        }

        // Player ����
        GameObject objectViewID = PhotonNetwork.Instantiate("Cowboy", Pos, Quaternion.identity);

        Debug.Log("Lobby ����" + objectViewID.transform.position);

        //objectViewID.GetComponent<PhotonView>();
        // ���� ����
        // ������ Player object�� ViewID�� ������
        myViewID = objectViewID.GetPhotonView().ViewID;
    }

    void GunFightPos()
    {
        Vector3 pos = Vector3.zero;

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            pos = MPos.position;
        }

        else if (PhotonNetwork.IsConnected & PhotonNetwork.IsMasterClient == false)
        {
            pos = CPos.position;
        }

        // Player ����
        GameObject objectViewID = PhotonNetwork.Instantiate("Cowboy", pos, Quaternion.identity);

        Debug.Log("GunFight ����" + objectViewID.transform.position);

        myViewID = objectViewID.GetPhotonView().ViewID;
    }
}