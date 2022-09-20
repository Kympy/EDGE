using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SniperGameManager : MonoBehaviourPunCallbacks
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
    [SerializeField] private UIManager _UIManager = null;
    [SerializeField] private Camera MyCamera = null; // Player Arm Camera
    [SerializeField] private GameObject Enemy = null;

    public GameObject GetEnemy { get { return Enemy; } }
    private SniperGameManager() { }
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient) // If master, Create Random Value and Send
        {
            photonView.RPC("PlayerInst", RpcTarget.AllBuffered, Random.Range(-250f, 0f), Random.Range(0f, 250f));
        }
        _WeatherManager.ApplyRandomSky();
        
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
            GameObject player = PhotonNetwork.Instantiate("Player", Vector3.one, Quaternion.identity); // Create First
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
            DisableMesh();
        }
    }
    
    public void DisableMesh()
    {
        if (photonView.IsMine)
        {
            GameObject[] disableObj = GameObject.FindGameObjectsWithTag("WillDisable");

            for (int i = 0; i < disableObj.Length; i++)
            {
                disableObj[i].AddComponent<DisableRenderer>();
            }
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
            /*
            if (PlayerList[0].GetComponent<SniperControl>().Is_Zoom)
            {
                screenPoint = MyCamera.GetComponentInChildren<Camera>().WorldToViewportPoint(Enemy.transform.position); // Scope Camera
                namePoint = MyCamera.GetComponentInChildren<Camera>().WorldToScreenPoint(Enemy.GetComponent<SniperControl>().NamePos.transform.position);
            }
            else
            {
                screenPoint = MyCamera.WorldToViewportPoint(Enemy.transform.position); // Player Arm Camera
                namePoint = MyCamera.WorldToScreenPoint(Enemy.GetComponent<SniperControl>().NamePos.transform.position);
            }
            */
            screenPoint = MyCamera.WorldToViewportPoint(Enemy.transform.position); // Player Arm Camera
            namePoint = MyCamera.WorldToScreenPoint(Enemy.GetComponent<SniperControl>().NamePos.transform.position);

            if (screenPoint.z > 0f &&
                screenPoint.x > 0f && screenPoint.x < 1f &&
                screenPoint.y > 0f && screenPoint.y < 1f)
            {
                
                Debug.Log("In!!");
                _UIManager.SetNickNamePosition(PlayerList[1].GetPhotonView().Owner.NickName, namePoint);
            }
            else
            {
                Debug.Log("Out!!");
                _UIManager.HideNickName();
            }
        }
    }
}
