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
    private void Awake()
    {
        _1PHouse = GameObject.Find("1PHouse");
        _1PPos = GameObject.Find("1PPosition").transform;
        _1PHouse.transform.position += new Vector3(Random.Range(0f, -250f), 0f, 0f);

        _2PHouse = GameObject.Find("2PHouse");
        _2PPos = GameObject.Find("2PPosition").transform;
        _2PHouse.transform.position += new Vector3(Random.Range(0f, 250f), 0f, 0f);
    }
    private void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            GameObject player = PhotonNetwork.Instantiate("Player", Vector3.one, Quaternion.identity);
            if(PhotonNetwork.IsMasterClient)
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
}
