using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] GameObject CamPosition;


    private RotateToMouse rotateToMouse;
    private GameObject Axe;
    public GameObject Knife;
    public bool[] hasAxe;
    public bool[] hasKnife;

    public int TotalCount = 0;
    public bool isEnd = false;

    Rigidbody rb;
    float press = 0f;
    float maxpress = 1000f;
    public GameObject ItemFactory;
    private Transform ThrowPoint;
    public Camera cam;
    AudioSource audioSource;
    public AudioClip audioWalk;
    public AudioClip audiothrowing;

    float delayTime = 0;

    public string AxeResourcePath = "DartMode/AxePrefab";
    public string KnifeResourcePath = "DartMode/KnifePrefab";

    bool isAxe = true;
    bool isHold = false;
    public bool IsHold { get { return isHold; } }

    [SerializeField] GameObject FakeAxe = null;
    [SerializeField] GameObject FakeKnife = null;

    private void Awake()
    {


        if (photonView.IsMine == false) return;
        cam = GameObject.Find("PlayerCam").GetComponent<Camera>();
        cam.transform.rotation = Quaternion.identity;
        ThrowPoint = cam.gameObject.transform.GetChild(0);
        FakeAxe.SetActive(true);
        FakeKnife.SetActive(false);
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
        //cam.gameObject.transform.rotation = Quaternion.Euler(cam.gameObject.transform.rotation.x, transform.rotation.y, transform.rotation.z);
        GetInput();

        if (Input.GetMouseButton(0))
        {
            isHold = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHold = false;
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
        Swap();
    }

    void Swap()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isAxe = !isAxe;
            if (isAxe == true)
            {
                // 도끼 활성화
                FakeAxe.SetActive(true);
                FakeKnife.SetActive(false);
            }

            else
            {
                // 칼 활성화
                FakeAxe.SetActive(false);
                FakeKnife.SetActive(true);
            }
        }
    }

    private void Throwing()
    {
        GameObject throwingObj = null;



        if (TotalCount < 5)
        {
            TotalCount++;

            if (isAxe == true)
            {
                throwingObj = PhotonNetwork.Instantiate(AxeResourcePath, ThrowPoint.position, Quaternion.LookRotation(ThrowPoint.forward));
            }

            else
            {
                throwingObj = PhotonNetwork.Instantiate(KnifeResourcePath, ThrowPoint.position, Quaternion.LookRotation(ThrowPoint.forward));
            }
            //추가
            if (throwingObj.tag == "Axe")
            {
                throwingObj.GetComponent<Rigidbody>().AddForce(throwingObj.transform.forward * press * 1.5f + throwingObj.transform.up * press * 0.8f);
                throwingObj.GetComponent<Rigidbody>().AddTorque(throwingObj.transform.right * press * 100000f);
            }
            else if (throwingObj.tag == "Knife")
            {
                throwingObj.GetComponent<Rigidbody>().AddForce(throwingObj.transform.forward * press * 1.5f);
            }

            if (TotalCount == 5)
            {
                FindObjectOfType<DartGameManager>().gameObject.GetPhotonView().RPC("EndGame", RpcTarget.All);
            }
        }

        //throwingObj.transform.position = ThrowPoint.position;
        //throwingObj.transform.rotation = ThrowPoint.rotation;
        /*Axe.transform.up = ThrowPoint.up;*/

        // 수정 
        //throwingObj.GetComponent<item>().itemSpeed = press;


        //Axe.transform.forward = ThrowPoint.forward;

        /*Axe.rb.position = ThrowPoint.position;
        Axe.transform.forward = ThrowPoint.forward;*/
    }

    void GetInput()
    {
        /*idown = Input.GetButtonDown("Interation");*/

    }

    // 내 클론에게 전달되는 함수
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 스트림에 쓸 때
        {
            stream.SendNext(FakeAxe.activeSelf); // 가짜 도끼의 현재 상태를 전달함
            stream.SendNext(FakeKnife.activeSelf); // 가짜 칼의 현재 상태를 전달함
        }

        else // 스트림에 읽어 올 때
        {
            FakeAxe.SetActive((bool)stream.ReceiveNext());
            FakeKnife.SetActive((bool)stream.ReceiveNext());
        }
    }
}


