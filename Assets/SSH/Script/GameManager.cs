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


    public int myViewID = 0;

    private void Awake()
    {
        

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            Debug.Log("LobbyPos");
            LobbyPos();
        }
        else
        {
            Debug.Log("GunFightPos");
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
            // Cowboy��� �̸��� ���� GameObject�� ã�� GetComponent<PlayerControl>
            // ���� : prefab���� ���� �� �̸��� Name(Clone)���� ����� ã������  
            // playerList = GameObject.Find("Cowboy").GetComponent<PlayerControl>();
        }


        // GameSceneLogic��� �̸��� ���� GameObject�� ã�� GameSceneLogic Component�� �޾ƿ�
        gameSceneLogic = GameObject.Find("GameSceneLogic").GetComponent<GameSceneLogic>();

    }

    // Start is called before the first frame update
    void Start()
    {
        // ���� Scene�� Ȯ���Ͽ� Player ��� ��/����
        CurSceneFind();
    }

    // Update is called once per frame0[��
    void Update()
    {

    }

    void CurSceneFind()
    {
        // Lobby Scene �÷��̾� �Ϻ� ��� Ȱ��ȭ 
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            playerControl.LobbyPlayerActive();
            // gameSceneLogic.LobbyPos();
        }

        else if(SceneManager.GetActiveScene().name == "GunFight")
        {
            playerControl.GunFightPlayerActive();
            // gameSceneLogic.GunFightPos();
        }
    }

    void LobbyPos()
    {
        Vector3 Pos = Vector3.zero;

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            Pos = MPos.position;
        }

        else if (PhotonNetwork.IsConnected)
        {
            Pos = CPos.position;
        }

        GameObject objectViewID = PhotonNetwork.Instantiate("Cowboy", Pos, Quaternion.identity);

        Debug.Log(objectViewID.transform.position);

        //objectViewID.GetComponent<PhotonView>();
        // ���� ����
        myViewID = objectViewID.GetPhotonView().ViewID;
        
    }

    void GunFightPos()
    {

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            Vector3 MasterPos = new Vector3(0, -0.5f, -10);

            PhotonNetwork.Instantiate("Cowboy", MasterPos, Quaternion.identity);
        }

        else if (PhotonNetwork.IsConnected)
        {
            Vector3 ClientPos = new Vector3(-6.5f, 0, 5.5f);

            PhotonNetwork.Instantiate("Cowboy", ClientPos, Quaternion.Euler(0f, 125, 0));
        }
    }
}