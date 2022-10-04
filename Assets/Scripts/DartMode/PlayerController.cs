using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] GameObject CamPosition;

    private RotateToMouse rotateToMouse;
    private MovementChracterController movement;
    GameObject nearObject;
    bool idown;
    private GameObject Axe;
    public GameObject Knife;
    public bool[] hasAxe;
    public bool[] hasKnife;
    public Animator anim;
    Rigidbody rb;
    float press = 0f;
    float maxpress = 1000f;
    public GameObject ItemFactory;
    public Transform ThrowPoint;
    public Camera cam;
    AudioSource audioSource;
    public AudioClip audioWalk;
    public AudioClip audiothrowing;

    float delayTime = 0;

    private void Awake()
    {
        if (photonView.IsMine == false) return;
        cam = GameObject.Find("PlayerCam").GetComponent<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();


        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementChracterController>();

        this.audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false) return;
        cam.gameObject.transform.position = CamPosition.transform.position;
        cam.gameObject.transform.rotation = CamPosition.transform.rotation;
        UpdateRotate();
        UpdateMove();
        GetInput();



        if (Input.GetButtonUp("Fire1"))
        {

            /*   Debug.Log("´øÁ³´Ù");*/
        }

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("hold", true);

            
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("hold", false);
            anim.SetTrigger("throw");
            audioSource.clip = audiothrowing;
            audioSource.Play();
        }

        if (Input.GetMouseButton(0))
        {
            press += Time.deltaTime * 200f;
            /*Debug.Log(press);*/

            if (press > maxpress)
            {
                press = maxpress;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Throwing();
            press = 0f;
        }
    }




    private void Throwing()
    {
        GameObject throwingObj = Instantiate(ItemFactory);
        throwingObj.transform.position = ThrowPoint.position;
        throwingObj.transform.rotation = ThrowPoint.rotation;
        /*Axe.transform.up = ThrowPoint.up;*/

        throwingObj.GetComponent<item>().itemSpeed = press;

        //Axe.transform.forward = ThrowPoint.forward;

        /*Axe.rb.position = ThrowPoint.position;
        Axe.transform.forward = ThrowPoint.forward;*/
    }

    void GetInput()
    {
        /*idown = Input.GetButtonDown("Interation");*/

    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }


    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movement.MoveTo(new Vector3(x, 0, z));


        if (x != 0 || z != 0)
        {
            delayTime += Time.deltaTime;
            anim.SetBool("walk", true);


            audioSource.clip = audioWalk;

            if (delayTime > audioSource.clip.length)
            {
                audioSource.Play();
                delayTime = 0;
            }
        }

        else
        {
            anim.SetBool("walk", false);
        }
        //anim.SetBool("walk", (x != 0 || z != 0));
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Axe")
        {
            nearObject = other.gameObject;
            Debug.Log("µµ³¢´ç");
        }

        if (other.tag == "Knife")
        {
            nearObject = other.gameObject;
            Debug.Log("Ä®ÀÌ´ç");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Axe")
        {
            nearObject = null;
        }

        if (other.tag == "Knife")
        {
            nearObject = null;
        }
    }
}


