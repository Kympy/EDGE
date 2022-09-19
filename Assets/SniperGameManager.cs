using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SniperGameManager : MonoBehaviourPunCallbacks
{
    #region Player Position Initialize Variables
    private GameObject _1PHouse = null;
    private GameObject _2PHouse = null;
    private Transform _1PPos; // Player 1 Position
    private Transform _2PPos; // Player 2 Position

    private float _1PrandX = 0f; // Player 1 Random X position
    private float _2PrandX = 0f; // Player 2 Random X position
    #endregion
    [SerializeField] private WeatherManager _WeatherManager = null;

    private SniperGameManager() { }
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient) // If master, Create Random Value and Send
        {
            photonView.RPC("RandPos", RpcTarget.AllBuffered, Random.Range(-250f, 0f), Random.Range(0f, 250f));
        }
        _WeatherManager.ApplyRandomSky();
    }

    [PunRPC]
    public void RandPos(float randX1, float randX2) // Move Start Position by random X value
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
        }

    }
}
