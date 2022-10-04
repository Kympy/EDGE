using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeatherManager : MonoBehaviourPun
{
    [SerializeField] public Material[] Skyboxes;
    [SerializeField] private Light Sun;
    [SerializeField] private GameObject Rain;
    private void Awake()
    {
        Rain.SetActive(false);
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
        switch(num)
        {
            case 0:
                {
                    Sun.intensity = 0.5f;
                    Rain.SetActive(true);
                    break;
                }
            case 1:
                {
                    Sun.intensity = 1f;
                    Rain.SetActive(false);
                    break;
                }
            case 2:
                {
                    Sun.intensity = 0.5f;
                    Rain.SetActive(false);
                    break;
                }
            case 3:
                {
                    Sun.intensity = 0.6f;
                    Rain.SetActive(true);
                    break;
                }
            case 4:
                {
                    Sun.intensity = 0.5f;
                    Rain.SetActive(false);
                    break;
                }
        }
        RenderSettings.skybox = Skyboxes[num];
    }
}
