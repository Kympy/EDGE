using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ImpactDestroy : MonoBehaviourPun
{
    private void Awake()
    {
        Invoke("DestroyThis", 1f);
    }
    private void DestroyThis()
    {
        //PhotonNetwork.Destroy(this.gameObject);
    }
}
