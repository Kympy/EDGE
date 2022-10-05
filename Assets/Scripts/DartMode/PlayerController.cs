using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] GameObject CamPosition;

    private RotateToMouse rotateToMouse;
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

    public string AxeResourcePath = "DartMode/AxePrefab";
    public string KnifeResourcePath = "DartMode/KnifePrefab";

    private void Awake()
    {
        if (photonView.IsMine == false) return;
        cam = GameObject.Find("PlayerCam").GetComponent<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();


        rotateToMouse = GetComponent<RotateToMouse>();

        this.audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false) return;
        cam.gameObject.transform.position = CamPosition.transform.position;
        cam.gameObject.transform.rotation = CamPosition.transform.rotation;
        UpdateRotate();
        GetInput();



        if (Input.GetButtonUp("Fire1"))
        {

            /*   Debug.Log("던졌다");*/
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
        GameObject throwingObj = PhotonNetwork.Instantiate(AxeResourcePath, ThrowPoint.position, ThrowPoint.rotation);
        //throwingObj.transform.position = ThrowPoint.position;
        //throwingObj.transform.rotation = ThrowPoint.rotation;
        /*Axe.transform.up = ThrowPoint.up;*/

        // 수정 
        //throwingObj.GetComponent<item>().itemSpeed = press;

        //추가
        if (throwingObj.tag == "Axe")
        {
            Debug.Log("테스트");
            throwingObj.GetComponent<Rigidbody>().AddForce(transform.forward * press * 1.5f + transform.up * press * 0.8f);
            throwingObj.GetComponent<Rigidbody>().AddTorque(transform.right * press * 100000f); 
        }
        else if (throwingObj.tag == "Knife")
        {
            throwingObj.GetComponent<Rigidbody>().AddForce(transform.forward * press * 1.5f);
        }

        Debug.Log("테스트2");
        
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


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Axe")
        {
            nearObject = other.gameObject;
            Debug.Log("도끼당");
        }

        if (other.tag == "Knife")
        {
            nearObject = other.gameObject;
            Debug.Log("칼이당");
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


