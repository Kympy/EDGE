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

    // bool LoginMaster = false;
    bool LoginClient = false;

    public int myViewID = 0;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            LobbyPos();

            // Master�� Client �� �� ���� ���� ��� GunFight Scene���� �̵��ϴ� �Լ� ȣ�� //&& PhotonNetwork.IsMasterClient
            if (PhotonNetwork.IsConnected && LoginClient)
            {
                // RpcTarget.MasterClient�� ��� ���� ������ NextScene���� 
                // ȣ�� �� GameObject�� RPC�� ��������ߵ�
                GameObject.Find("GameSceneLogic").GetComponent<PhotonView>().RPC("RPCScene", RpcTarget.AllBuffered);
            }
        }

        else if (SceneManager.GetActiveScene().name == "GunFight")
        {
            // GunFight Scene : Master Pos, Client Pos setting
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
                }
            }
            // Player��� �̸��� ���� GameObject�� ã�� GetComponent<PlayerControl>
            // ���� : prefab���� ���� �� �̸��� Name(Clone)���� ����� ã������  
            // playerList = GameObject.Find("Player").GetComponent<PlayerControl>();
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
            //LoginMaster = true;
        }

        else if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient == false)
        {
            Pos = CPos.position;
            LoginClient = true;
        }

        // Player ����
        GameObject objectViewID = PhotonNetwork.Instantiate("PistolMode/Player", Pos, Quaternion.identity);

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
            //Debug.Log(pos);
        }

        else if (PhotonNetwork.IsConnected & PhotonNetwork.IsMasterClient == false)
        {
            pos = CPos.position;
            //Debug.Log(pos);
        }

        // Player ����
        //GameObject objectViewID = PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);
        GameObject objectViewID = PhotonNetwork.Instantiate("PistolMode/Player", pos, Quaternion.identity);

        myViewID = objectViewID.GetPhotonView().ViewID;
    }
}