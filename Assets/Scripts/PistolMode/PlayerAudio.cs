using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;


public class PlayerAudio : MonoBehaviourPun
{
    [Header("WalkAudio")]
    [SerializeField]
    AudioSource audioWalk = null;
    [Header("FireAudio")]
    [SerializeField]
    AudioSource audioFire = null;
    [Header("DeadAudio")]
    [SerializeField]
    AudioSource audioDead = null;

    [SerializeField]
    AudioClip[] playerWalk = new AudioClip[4];
    [SerializeField]
    AudioClip playerFire = null;
    [SerializeField]
    AudioClip playerDead = null;

    float walkTime = 0;

    public string PlayerState;

    private void Awake()
    {/*
        if (photonView.IsMine)
        {
            Debug.Log(audioWalk.name);
            audioWalk.clip = playerWalk[0];
            Debug.Log(audioWalk.clip.length);
            Debug.Log(playerWalk[0].name);

        }*/
    }

    public void WalkAudio()
    {
        // Debug.Log("°È´ÂÁß");

        walkTime += Time.deltaTime;

        Debug.Log(walkTime);

        // audioWalk.clip.length
        if(walkTime > audioWalk.clip.length)
        {
            audioWalk.clip = playerWalk[Random.Range(0, 4)];
            audioWalk.Play();
            walkTime = 0;
        }
    }

    public void FireAudio()
    {
        // Debug.Log("½î´ÂÁß");

        audioFire.clip = playerFire;
        audioFire.Play();
    }

    public void DeadAudio()
    {
        Debug.Log("Á×´ÂÁß");

        audioDead.clip = playerDead;
        audioDead.Play();
    }

}
