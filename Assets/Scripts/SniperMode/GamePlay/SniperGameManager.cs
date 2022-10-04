using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SniperGameManager : Singleton<SniperGameManager> 
{
    public List<GameObject> PlayerList = new List<GameObject>();
    #region Player Position Initialize Variables
    private GameObject _1PHouse = null;
    private GameObject _2PHouse = null;
    private Transform _1PPos; // Player 1 Position
    private Transform _2PPos; // Player 2 Position

    private float _1PrandX = 0f; // Player 1 Random X position
    private float _2PrandX = 0f; // Player 2 Random X position
    #endregion
    [SerializeField] private WeatherManager _WeatherManager = null;
    [SerializeField] private SniperUIManager _UIManager = null;
    [SerializeField] private Camera MyCamera = null; // Player Arm Camera
    [SerializeField] private GameObject Enemy = null;

    private bool IsEnd = false;

    public SniperUIManager GetUI { get { return _UIManager; } }
    public PrefabData prefabData = null;
    public GameObject GetEnemy { get { return Enemy; } }
    private SniperGameManager() { }
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        if (PhotonNetwork.IsMasterClient) // If master, Create Random Value and Send
        {
            photonView.RPC("PlayerInst", RpcTarget.AllBuffered, Random.Range(-250f, 0f), Random.Range(0f, 250f));
            StartCoroutine(ODINAPIHandler.Instance.ProcessBettingCoin(ODINAPIHandler.COIN_TYPE.zera));
        }
        _WeatherManager.ApplyRandomSky();
        PhotonNetwork.CurrentRoom.IsOpen = false;      
    }
    private void Update()
    {
        EnemyInCamera();
    }
    [PunRPC]
    public void PlayerInst(float randX1, float randX2) // Move Start Position by random X value
    {
        _1PrandX = randX1;
        _2PrandX = randX2;
        // Move House
        _1PHouse = GameObject.Find("1PHouse");
        _1PHouse.transform.position += new Vector3(_1PrandX, 0f, 0f);
        _1PPos = GameObject.Find("1PPosition").transform;

        _2PHouse = GameObject.Find("2PHouse");
        _2PHouse.transform.position += new Vector3(_2PrandX, 0f, 0f);
        _2PPos = GameObject.Find("2PPosition").transform;


        if (PhotonNetwork.IsConnected)
        {
            GameObject player = PhotonNetwork.Instantiate("SniperMode/Player", Vector3.one, Quaternion.identity); // Create First
            if (PhotonNetwork.IsMasterClient) // Master == Player 1
            {
                player.transform.position = _1PPos.position; // Move
                player.transform.rotation = _1PPos.rotation;
            }
            else // Other == Player 2
            {
                player.transform.position = _2PPos.position;
                player.transform.rotation = _2PPos.rotation;
            }
            _UIManager.InitUserHP();
        }
    }
    public void EnemyInCamera() // Is Enemy Player in my camera?
    {
        if(PlayerList.Count > 1)
        {
            Enemy = PlayerList[1];
        }
        if (Enemy != null)
        {
            if (PlayerList[0].GetComponent<SniperControl>().Is_Zoom)
            {
                _UIManager.HideNickName();
                return;
            }

            Vector3 screenPoint = Vector3.zero;
            Vector3 namePoint = Vector3.zero;

            screenPoint = MyCamera.WorldToViewportPoint(Enemy.transform.position); // Player Arm Camera
            namePoint = MyCamera.WorldToScreenPoint(Enemy.GetComponent<SniperControl>().NamePos.transform.position);

            if (screenPoint.z > 0f &&
                screenPoint.x > 0f && screenPoint.x < 1f &&
                screenPoint.y > 0f && screenPoint.y < 1f)
            {
                
                //Debug.Log("In!!");
                _UIManager.SetNickNamePosition(PlayerList[1].GetPhotonView().Owner.NickName, namePoint);
            }
            else
            {
                //Debug.Log("Out!!");
                _UIManager.HideNickName();
            }
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(otherPlayer != PhotonNetwork.LocalPlayer)
        {
            if (IsEnd == false)
            {
                StartCoroutine(ODINAPIHandler.Instance.ProcessRequestDeclareWinner(ODINAPIHandler.COIN_TYPE.zera, ODINAPIHandler.Winner.Me));
                photonView.RPC("GameEndByPlayerExit", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    public void GameEndByPlayerExit()
    {
        IsEnd = true;
        Debug.Log(ODINAPIHandler.Instance.GetDeclare_Winner().message);
        _UIManager.gameObject.GetPhotonView().RPC("ProcessGameExit", RpcTarget.All);
    }
    [PunRPC]
    public void GameEnd(ODINAPIHandler.Winner winner)
    {
        IsEnd = true;
        switch(winner)
        {
            case ODINAPIHandler.Winner.Me:
                {
                    StartCoroutine(ODINAPIHandler.Instance.ProcessRequestDeclareWinner(ODINAPIHandler.COIN_TYPE.zera, ODINAPIHandler.Winner.Me));
                    break;
                }
            case ODINAPIHandler.Winner.Other:
                {
                    StartCoroutine(ODINAPIHandler.Instance.ProcessRequestDeclareWinner(ODINAPIHandler.COIN_TYPE.zera, ODINAPIHandler.Winner.Other));
                    break;
                }
            default: { Debug.Log("GameManager : Winner declare error"); break; }
        }
        _UIManager.gameObject.GetPhotonView().RPC("ProcessGameEnd", RpcTarget.All, winner);
    }
}
