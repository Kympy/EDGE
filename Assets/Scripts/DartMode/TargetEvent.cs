using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class TargetEvent : MonoBehaviour
{   
    [SerializeField] private int targetscore;
    [SerializeField] ScoreText ST;
    AudioSource audioSource;
    public AudioClip audioHit;
    public GameObject HitEffect;
    // Start is called before the first frame update
    private void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade"
            || collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Knife")
        {
            audioSource.clip = audioHit;
            audioSource.Play();

            PhotonNetwork.Instantiate("DartMode/WoodEffect", collision.contacts[0].point, Quaternion.LookRotation(-collision.contacts[0].normal));

            collision.gameObject.GetPhotonView().RPC("ObjReset", RpcTarget.All);

            if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade"
                || collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Knife")
            {
                ST.getScore += targetscore;
                ST.AddScore();
            }
        }
    }
}
