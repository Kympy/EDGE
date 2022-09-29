using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeatherManager : MonoBehaviourPun
{
    [SerializeField] public Material[] Skyboxes;
    private void Awake()
    {
        Skyboxes = Resources.LoadAll<Material>("SniperMode/Skyboxes");
    }
    public void ApplyRandomSky()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            int i = Random.Range(0, Skyboxes.Length);
            photonView.RPC("RandomSky", RpcTarget.AllBuffered, i);
        }
    }
    [PunRPC]
    private void RandomSky(int num)
    {
        RenderSettings.skybox = Skyboxes[num];
    }
}
