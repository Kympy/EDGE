using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour
{
    [Header("WalkAudio")]
    [SerializeField]
    AudioSource audioWalk;
    [Header("FireAudio")]
    [SerializeField]
    AudioSource audioFire;
    [Header("DeadAudio")]
    [SerializeField]
    AudioSource audioDead;

    [SerializeField]
    AudioClip[] playerWalk = new AudioClip[4];
    [SerializeField]
    AudioClip playerFire;
    [SerializeField]
    AudioClip playerDead;

    public string PlayerState;

    public void WalkAudio()
    {
        Debug.Log("�ȴ���");

        audioWalk.clip = playerWalk[Random.Range(0, 4)];
            // audioWalk.clip.length
        for (float i=0; i < audioWalk.clip.length; i += Time.deltaTime)
        {
            audioWalk.Play();

        }
    }

    public void FireAudio()
    {
        Debug.Log("�����");

        audioFire.clip = playerFire;
        audioFire.Play();

    }

    public void DeadAudio()
    {
        Debug.Log("�״���");

        audioDead.clip = playerDead;
        audioDead.Play();
    }

}
