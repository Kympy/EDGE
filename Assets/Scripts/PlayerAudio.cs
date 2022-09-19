using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerAudio : MonoBehaviourPun
{
    //private PlayerControl _PlayerControl = null;

    private float footTimer = 0f;

    private AudioSource[] PlayerSound = null;

    private AudioClip[] Foot = null;
    private AudioClip Fire = null;
    private AudioClip Reload = null;

    private int RandFootSound = 0;

    private void Awake()
    {
        if(photonView.IsMine)
        {
            PlayerSound = GetComponents<AudioSource>();
            //_PlayerControl = this.GetComponent<PlayerControl>();
            Foot = Resources.LoadAll<AudioClip>("Sound/Foot");
            PlayerSound[0].clip = Foot[0];
            Fire = Resources.Load<AudioClip>("Sound/SniperFire");
            Reload = Resources.Load<AudioClip>("Sound/Reload");
        }
    }
    public void WalkSound()
    {
        footTimer += Time.deltaTime;
        if (footTimer > PlayerSound[0].clip.length)
        {
            RandFootSound = Random.Range(0, Foot.Length);
            PlayerSound[0].clip = Foot[RandFootSound];
            PlayerSound[0].Play();
            footTimer = 0f;
        }
    }
    public void FireSound()
    {
        PlayerSound[1].clip = Fire;
        PlayerSound[1].Play();
        Invoke("ReloadSound", 1f);
    }
    private void ReloadSound()
    {
        PlayerSound[2].clip = Reload;
        PlayerSound[2].Play();
        PlayerSound[3].PlayDelayed(0.5f);
    }
}
