using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DartGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject PlayerSpawnPosition;

    // Start is called before the first frame update
    private void Awake()
    {
        /*if (photonView.IsMine == false) return;*/
        PhotonNetwork.Instantiate("DartMode/Player", PlayerSpawnPosition.transform.position, PlayerSpawnPosition.transform.rotation);
    }

    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
