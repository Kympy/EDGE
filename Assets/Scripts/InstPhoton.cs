using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InstPhoton : MonoBehaviourPunCallbacks
{
    private Transform _1PPos;
    private Transform _2PPos;

    private GameObject _1PHouse = null;
    private GameObject _2PHouse = null;

    private float _1PrandX = 0f;
    private float _2PrandX = 0f;

    [PunRPC]
    public void RandPos(float randX1, float randX2)
    {
        _1PrandX = randX1;
        _2PrandX = randX2;

        _1PHouse = GameObject.Find("1PHouse");
        _1PHouse.transform.position += new Vector3(_1PrandX, 0f, 0f);
        _1PPos = GameObject.Find("1PPosition").transform;

        _2PHouse = GameObject.Find("2PHouse");
        _2PHouse.transform.position += new Vector3(_2PrandX, 0f, 0f);
        _2PPos = GameObject.Find("2PPosition").transform;

        if (PhotonNetwork.IsConnected)
        {
            GameObject player = PhotonNetwork.Instantiate("Player", Vector3.one, Quaternion.identity);
            if (PhotonNetwork.IsMasterClient)
            {
                player.transform.position = _1PPos.position;
                player.transform.rotation = _1PPos.rotation;
            }
            else
            {
                player.transform.position = _2PPos.position;
                player.transform.rotation = _2PPos.rotation;
            }
        }

    }
    private void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RandPos", RpcTarget.AllBuffered, Random.Range(-250f, 0f), Random.Range(0f, 250f));
        }
    }
}
