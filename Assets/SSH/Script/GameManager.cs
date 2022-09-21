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
            // Cowboy라는 이름을 가진 GameObject를 찾아 GetComponent<PlayerControl>
            // 단점 : prefab으로 생성 시 이름이 Name(Clone)으로 변경돼 찾지못함  
            // playerList = GameObject.Find("Cowboy").GetComponent<PlayerControl>();
        }


        // GameSceneLogic라는 이름을 가진 GameObject를 찾아 GameSceneLogic Component를 받아옴
        gameSceneLogic = GameObject.Find("GameSceneLogic").GetComponent<GameSceneLogic>();

    }

    // Start is called before the first frame update
    void Start()
    {
        // 현재 Scene을 확인하여 Player 기능 온/오프
        CurSceneFind();
    }

    // Update is called once per frame0[ㅔ
    void Update()
    {

    }

    void CurSceneFind()
    {
        // Lobby Scene 플레이어 일부 기능 활성화 
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
        // 위와 같음
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