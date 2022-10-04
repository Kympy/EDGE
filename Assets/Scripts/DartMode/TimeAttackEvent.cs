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
    public int Randomtarget;
    public GameObject light;

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
       if (Input.GetKeyDown(KeyCode.Space))
        {
            Randomtarget = Random.Range(0, 3);
            RandomPlaying();
            Debug.Log("·£´ý½ÃÀÛ");
        }
     
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade"
            || collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Knife")
        {
            
            audioSource.clip = audioHit;
            audioSource.Play();

            Vector3 contact = collision.contacts[0].point;
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact);
            Instantiate(HitEffect, contact, rot);

            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;


            if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade"
                || collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Knife")
            {
                /*ST.getScore += targetscore;
                /*ST.UpdateScore();*/
            }
            /*Debug.Log(ST.getScore);*/
        }

        else
            Debug.Log("ÃÄ³Â´ç");
    }

    void RandomPlaying()
    {
        if (Randomtarget == 0)
        {
            gameObject.SetActive(true);
            Debug.Log("ºÒÄÑÁü");
        }

        else if (Randomtarget == 1)
        {
            gameObject.SetActive(true);
            Debug.Log("ºÒÄÑÁü2");
        }

        else if (Randomtarget == 2)
        {
            gameObject.SetActive(true);
            Debug.Log("ºÒÄÑÁü3");
        }
    }
}
