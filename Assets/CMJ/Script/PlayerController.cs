using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

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
    

    private void Awake()
    {
        if (photonView.IsMine == false) return;
        cam = GameObject.Find("PlayerCam").GetComponent<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        

        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementChracterController>();
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
        Interation();
        
        

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
        GameObject Axe = Instantiate(ItemFactory);
        Axe.transform.position = ThrowPoint.position;
        Axe.transform.rotation = ThrowPoint.rotation;
        /*Axe.transform.up = ThrowPoint.up;*/

        Axe.GetComponent<item>().itemSpeed = press;

        //Axe.transform.forward = ThrowPoint.forward;

        /*Axe.rb.position = ThrowPoint.position;
        Axe.transform.forward = ThrowPoint.forward;*/
    }

    void GetInput()
    {
        /*idown = Input.GetButtonDown("Interation");*/
        
    }

   

    void Interation()
    {
        if (idown && nearObject != null)
        {
            if (nearObject.tag == "Axe")
            {
                item item = nearObject.GetComponent<item>();
                int AxeIndex = item.value;
                hasAxe[AxeIndex] = true;
                /*Debug.Log("µµ³¢¸Ô¾ú´ç");*/

                if (hasAxe[AxeIndex] == true)
                {
                    Axe.SetActive(true);
                    Knife.SetActive(false);

                }

            }

            if (nearObject.tag == "Knife")
            {
                item item = nearObject.GetComponent<item>();
                int KnifeIndex = item.value;
                hasKnife[KnifeIndex] = true;
                /*Debug.Log("Ä®¸Ô¾ú´ç");*/
            }
        }
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
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Axe")
        {
            nearObject = other.gameObject;
            /*Debug.Log("µµ³¢´ç");*/
        }

        if (other.tag == "Knife")
        { 
            nearObject = other.gameObject;
            /*Debug.Log("Ä®ÀÌ´ç");*/
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


