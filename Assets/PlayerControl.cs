using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerControl : MonoBehaviourPun
{
    private float horizontal;
    private float vertical;

    private float mouseX;
    private float mouseY;

    private bool hasHorizontalInput;
    private bool hasVerticalInput;

    private bool IsMove;
    private bool TimeMode = false;

    private Vector3 moveVector;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float mouseSpeed = 10f;
    [SerializeField] private float zoomMouseSpeed = 0.1f;

    public GameObject bullet;
    public Transform shootPos;
    public GameObject upper;

    public GameObject zoom;
    public GameObject gun;
    public GameObject normalCanvas;

    private bool IsZoom = false;
    public Transform zoomPos;
    private Camera myCamera;
    private Coroutine myCo;
    private float shootRot;
    private void Awake()
    {
        if (photonView.IsMine == false) return;
        myCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }
    private void Start()
    {
        if (photonView.IsMine == false) return;

        zoom = GameObject.Find("ZoomCanvas");
        gun = GameObject.Find("Gun");

        normalCanvas = GameObject.Find("Canvas");
        upper = GameObject.Find("Player (1)");
        shootPos = GameObject.Find("ShootPos").transform;
        zoomPos = GameObject.Find("zoomPos").transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        zoom.SetActive(false);
    }
    private void Update()
    {
        myCamera.transform.position = transform.position + new Vector3(0f, 0.8f, 0f);
        if (photonView.IsMine == false) return;
        if(Input.GetKeyDown(KeyCode.P))
        {
            TimeMode = !TimeMode;
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        IsMove = hasHorizontalInput || hasVerticalInput;

        if(IsZoom)
        {
            mouseX = Input.GetAxis("Mouse X") * zoomMouseSpeed;
            mouseY = Input.GetAxis("Mouse Y") * zoomMouseSpeed;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
            mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;
        }
        myCamera.transform.eulerAngles += new Vector3(-mouseY, mouseX, 0f);
        upper.transform.eulerAngles = new Vector3(myCamera.transform.eulerAngles.x, myCamera.transform.eulerAngles.y, 0f);
        transform.eulerAngles += new Vector3(0f, mouseX, 0f);
        if(TimeMode)
        {
            Time.timeScale = moveSpeed / maxSpeed;
        }
        if(Input.GetMouseButtonDown(1))
        {
            IsZoom = !IsZoom;
            /*
            zoomCount++;
            IsZoom = true;
            if(zoomCount >= 4f)
            {
                IsZoom = false;
                zoomCount = 0;
            }
            */

        }
        if (IsZoom)
        {
            /*
            if (zoomCount == 1)
                Camera.main.fieldOfView = 60f / 6f;
            else if (zoomCount == 2)
                Camera.main.fieldOfView = 60f / 8f;
            else if (zoomCount == 3)
                Camera.main.fieldOfView = 60f / 10f;
            */
            gun.SetActive(false);
            normalCanvas.SetActive(false);
            zoom.SetActive(true);
        }
        else
        {
            myCamera.fieldOfView = 60f;
            gun.SetActive(true);
            normalCanvas.SetActive(true);
            zoom.SetActive(false);
        }
        if (IsMove)
        {
            moveVector = new Vector3(horizontal, 0f, vertical);
            PlusSpeed();

            transform.Translate(moveSpeed * Time.deltaTime * moveVector);
        }
        else
        {
            MinusSpeed();
        }
        if(Input.GetAxis("Mouse ScrollWheel") != 0 && IsZoom)
        {
            myCamera.fieldOfView += -Input.GetAxis("Mouse ScrollWheel") * 10f;
            if(myCamera.fieldOfView > 60f)
            {
                myCamera.fieldOfView = 60f;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            shootRot = myCamera.transform.eulerAngles.x;
            shootRot = shootRot > 180f ? shootRot - 360f : shootRot;
            if (IsZoom)
            {
                PhotonNetwork.Instantiate("Bullets", zoomPos.position, zoomPos.rotation);
            }
            else
            {
                PhotonNetwork.Instantiate("Bullets", shootPos.position, shootPos.rotation);
            }
            if (myCo != null)
            {
                StopCoroutine(myCo);
                myCo = null;
            }
            myCo = StartCoroutine(FireForce());

        }
    }
    private IEnumerator FireForce()
    {
        float rotValueX = 0f;
        float timer = 0f;
        while(true)
        {
            timer += Time.deltaTime;
            if(IsZoom)
            {
                rotValueX += 0.02f;
            }
            else
            {
                rotValueX += 0.1f;
            }

            myCamera.transform.eulerAngles -= new Vector3(rotValueX, 0f, 0f);
            if(timer > 0.1f)
            {
                StartCoroutine(DownForce());
                StopCoroutine(myCo);
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator DownForce()
    {
        float rotValueX = 0f;
        float randomOffset = 0f;
        while (true)
        {
            if (IsZoom)
            {
                rotValueX = 0.02f;
            }
            else
            {
                rotValueX = 0.1f;
            }

            myCamera.transform.eulerAngles += new Vector3(rotValueX, 0f, 0f);
            float desiredX = myCamera.transform.eulerAngles.x;
            desiredX = desiredX > 180f ? desiredX - 360f : desiredX;
            if (IsZoom)
            {
                randomOffset = Random.Range(-0.2f, 0.21f);
            }
            else
            {
                randomOffset = Random.Range(-1.0f, 1.01f);
            }

            if (desiredX > shootRot + randomOffset)
            {
                yield break;
            }
            //if (timer > 0.3f)
            //{
                //StopCoroutine(myCo);
                //yield break;
            //}
            yield return null;
        }
    }
    private void PlusSpeed()
    {
        Debug.Log(moveSpeed);
        moveSpeed += 0.01f;
        if(moveSpeed > maxSpeed)
        {
            moveSpeed = maxSpeed;
        }
    }
    private void MinusSpeed()
    {
        moveSpeed -= 0.1f;
        if(moveSpeed < 0f)
        {
            moveSpeed = 0f;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Debug.LogError("Hit Player!!");
        }
    }
}
