using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Muzzleflash : MonoBehaviourPun, IPunObservable 
{
    GameObject MuzzleFireFlash;

    private void Awake()
    {
        MuzzleFireFlash = GameObject.Find("Muzzleflash");
    }
    
    public void MuzzleflashActive()
    {
        gameObject.SetActive(true);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(this.gameObject.activeSelf);
        }
        else
        {
            this.gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}
