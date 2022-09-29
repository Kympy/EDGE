using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Result : MonoBehaviourPun
{
    [SerializeField]
    GameObject resultWin;
    [SerializeField]
    GameObject resultLose;


    [PunRPC]
    void ResultWin()
    {

    }

    [PunRPC]
    void ResultLose()
    {

    }

}
