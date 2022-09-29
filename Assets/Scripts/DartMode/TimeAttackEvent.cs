using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAttackEvent : MonoBehaviour
{
    [SerializeField] private int targetscore;
    [SerializeField] ScoreText ST;
    public GameObject HitEffect;
    AudioSource audioSource;
    public AudioClip audioHit;
    // Start is called before the first frame update
    private void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade"
            || collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Knife")
        {
            audioSource.clip = audioHit;
            audioSource.Play();

            Instantiate(HitEffect, transform.position, transform.rotation);
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;


            if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade"
                || collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Knife")
            {
                /*ST.getScore += targetscore;
                ST.UpdateScore();*/
            }
            /*Debug.Log(ST.getScore);*/
        }

        else
            Debug.Log("ÃÄ³Â´ç");
    }

}
