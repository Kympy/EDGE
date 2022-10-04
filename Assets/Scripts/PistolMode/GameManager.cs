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

            // Master와 Client 둘 다 접속 했을 경우 GunFight Scene으로 이동하는 함수 호출 //&& PhotonNetwork.IsMasterClient
            if (PhotonNetwork.IsConnected && LoginClient)
            {
                // RpcTarget.MasterClient일 경우 적용 범위와 NextScene에서 
                // 호출 할 GameObject의 RPC를 지정해줘야됨
                GameObject.Find("GameSceneLogic").GetComponent<PhotonView>().RPC("RPCScene", RpcTarget.AllBuffered);
            }
        }

        else if (SceneManager.GetActiveScene().name == "GunFight")
        {
            // GunFight Scene : Master Pos, Client Pos setting
            GunFightPos();
        }

        // 나의 ViewID에 해당하는 오브젝트 찾기
        if (photonView.IsMine)
        {
            // PlayerControl[] : room에 저장된 플레이어의 정보를 임시로 담기위한 저장공간 
            PlayerControl[] playerList;

            // PlayerControl 스크립트가 붙어있는 GameObject오브젝트를 찾음
            playerList = GameObject.FindObjectsOfType<PlayerControl>();
            // PlayerControl 배열을 순차적으로 탐색하면서 foreach문 처리
            foreach (PlayerControl pC in playerList)
            {
                // 서버에 입력된 viewID와 내 playerControl에 저장된 ViewID를 비교하여 내 ViewID 추출
                if (pC.gameObject.GetPhotonView().ViewID == myViewID)
                {
                    playerControl = pC;
                }
            }
            // Player라는 이름을 가진 GameObject를 찾아 GetComponent<PlayerControl>
            // 단점 : prefab으로 생성 시 이름이 Name(Clone)으로 변경돼 찾지못함  
            // playerList = GameObject.Find("Player").GetComponent<PlayerControl>();
        }

        // GameSceneLogic라는 이름을 가진 GameObject를 찾아 GameSceneLogic Component를 받아옴
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

        // Player 생성
        GameObject objectViewID = PhotonNetwork.Instantiate("PistolMode/Player", Pos, Quaternion.identity);

        //objectViewID.GetComponent<PhotonView>();
        // 위와 같음
        // 생성된 Player object의 ViewID를 가져옴
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

        // Player 생성
        //GameObject objectViewID = PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);
        GameObject objectViewID = PhotonNetwork.Instantiate("PistolMode/Player", pos, Quaternion.identity);

        myViewID = objectViewID.GetPhotonView().ViewID;
    }
}