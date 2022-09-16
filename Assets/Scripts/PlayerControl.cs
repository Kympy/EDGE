using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


public class PlayerControl : MonoBehaviourPun
{
    // Sensitivity
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSpeed = 10f;
    [SerializeField] private float zoomMouseSpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 50f;
    // Player Upper Bone
    private Transform UpperBody = null;
    // Player Rigidbody
    private Rigidbody _Rigidbody = null;
    // Prefab
    private GameObject BulletPrefab = null;
    // Player Virtual Arm
    private GameObject Arm = null;
    // Player Control Values
    // Keyboard Movement
    private float horizontal;
    private float vertical;
    private Vector3 moveVector;
    // Mouse Movement
    private float mouseX = 0f;
    private float mouseY = 0f;
    // Mouse Upper Rotation
    private float mouseYUpper = 90f;
    // Fire
    private float shootRot;
    // Fire Position
    public Transform shootPos;
    public Transform ZoomShootPosition;
    // Player Control Booleans
    private bool IsZoom = false;
    // Keyboard Input Checker
    private bool hasHorizontalInput;
    private bool hasVerticalInput;
    private bool IsMove;
    // Camera
    private Camera PlayerCamera;
    private Camera ScopeCamera; // Sniper rifle scope
    // Zoom Coroutine
    private Coroutine ZoomCoroutine = null;
    // Recoil Coroutine
    private Coroutine ReCoilCoroutine = null;
    // Zoom In / Out Arm Position
    private Transform ZoomOutPos;
    private Transform ZoomInPos;
    // Player Following Camera Position
    private Transform PlayerCameraPos;
    // Animator
    private Animator _ArmAnimator;
    private Animator _PlayerAnimator;

    private void Awake()
    {
        BulletPrefab = Resources.Load<GameObject>("Bullets");
        if (BulletPrefab == null) Debug.LogError("Bullet prefab load failed");


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        if (photonView.IsMine == false) return;

        PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        _ArmAnimator = PlayerCamera.GetComponentInChildren<Animator>();
        _PlayerAnimator = GetComponent<Animator>();
        //_Rigidbody = GetComponent<Rigidbody>();
        ZoomInPos = GameObject.Find("ZoomInPos").transform;
        ZoomOutPos = GameObject.Find("ZoomOutPos").transform;
        PlayerCameraPos = GameObject.Find("CameraPosition").transform;

        ScopeCamera = GameObject.Find("ScopeCamera").GetComponent<Camera>();

        UpperBody = _PlayerAnimator.GetBoneTransform(HumanBodyBones.Spine);

        Arm = GameObject.Find("PlayerArmPivot");
        shootPos = GameObject.Find("ShootPos").transform;
        ZoomShootPosition = GameObject.Find("ZoomShootPos").transform;

        Arm.transform.localPosition = ZoomOutPos.localPosition;
    }
    private void LateUpdate()
    {
        //Debug.Log("Upper : " + mouseY);
        if (photonView.IsMine == false) return;
        UpperBody.eulerAngles = new Vector3(0f, UpperBody.eulerAngles.y, -mouseYUpper);
    }
    private void FixedUpdate()
    {
        PlayerCamera.transform.position = PlayerCameraPos.position;
        Movement();
    }
    private void Update()
    {
        if (photonView.IsMine == false) return;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        IsMove = hasHorizontalInput || hasVerticalInput;

        if(IsZoom)
        {
            mouseX += Input.GetAxis("Mouse X") * zoomMouseSpeed * Time.deltaTime;
            mouseY += Input.GetAxis("Mouse Y") * zoomMouseSpeed * Time.deltaTime;
            mouseYUpper += Input.GetAxis("Mouse Y") * zoomMouseSpeed * Time.deltaTime;
        }
        else
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
            mouseY += Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
            mouseYUpper += Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        }

        mouseX = mouseX > 180f ? mouseX - 360f : mouseX;
        mouseY = mouseY > 180f ? mouseY - 360f : mouseY;
        mouseYUpper = mouseYUpper > 180f ? mouseYUpper - 360f : mouseYUpper;

        mouseY = Mathf.Clamp(mouseY, -50f, 70f);
        mouseYUpper = Mathf.Clamp(mouseYUpper, 40f, 160f); // mouse Y + 90f

        PlayerCamera.transform.eulerAngles = new Vector3(-mouseY, mouseX, 0f);
        //transform.eulerAngles = new Vector3(0f, mouseX, 0f);
        //_Rigidbody.rotation = Quaternion.Euler(0f, mouseX, 0f);
        transform.rotation = Quaternion.Euler(0f, mouseX, 0f);

        if(Input.GetMouseButtonDown(1))
        {
            IsZoom = !IsZoom;
            if(ZoomCoroutine != null)
            {
                StopCoroutine(ZoomCoroutine);
                ZoomCoroutine = null;
            }
            ZoomCoroutine = StartCoroutine(Zoom());
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0 && IsZoom)
        {
            ScopeCamera.fieldOfView += -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
            if(ScopeCamera.fieldOfView > 10f)
            {
                ScopeCamera.fieldOfView = 10f;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            _ArmAnimator.SetTrigger("Fire");
            shootRot = PlayerCamera.transform.eulerAngles.x;
            shootRot = shootRot > 180f ? shootRot - 360f : shootRot;
            if (IsZoom)
            {
                PhotonNetwork.Instantiate("Bullets", ZoomShootPosition.position, ZoomShootPosition.rotation);
            }
            else
            {
                PhotonNetwork.Instantiate("Bullets", shootPos.position, shootPos.rotation);
            }
            if (ReCoilCoroutine != null)
            {
                StopCoroutine(ReCoilCoroutine);
                ReCoilCoroutine = null;
            }
            ReCoilCoroutine = StartCoroutine(ReCoilUp());

        }
        UpdateAnimation();
    }
    private void Movement()
    {
        if (IsMove)
        {
            moveVector = (horizontal * transform.right + vertical * transform.forward).normalized;
            //transform.Translate(moveSpeed * Time.deltaTime * moveVector);
            transform.position += moveSpeed * Time.deltaTime * moveVector;
        }
    }
    private IEnumerator Zoom()
    {
        while (true)
        {
            if (IsZoom)
            {
                Arm.transform.localPosition = Vector3.MoveTowards(Arm.transform.localPosition, ZoomInPos.localPosition, Time.deltaTime * 2f);
                PlayerCamera.fieldOfView -= 4f;
                if (PlayerCamera.fieldOfView < 9f)
                {
                    PlayerCamera.fieldOfView = 9f;
                }
                if (Arm.transform.localPosition == ZoomInPos.transform.localPosition && PlayerCamera.fieldOfView == 9f)
                {
                    yield break;
                }
            }
            else
            {
                Arm.transform.localPosition = Vector3.MoveTowards(Arm.transform.localPosition, ZoomOutPos.localPosition, Time.deltaTime * 2f);
                PlayerCamera.fieldOfView += 4f;
                if(PlayerCamera.fieldOfView > 60f)
                {
                    PlayerCamera.fieldOfView = 60f;
                }
                if (Arm.transform.localPosition == ZoomOutPos.transform.localPosition && PlayerCamera.fieldOfView == 60f)
                {
                    yield break;
                }

            }
            yield return null;
        }
    }
    private IEnumerator ReCoilUp()
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

            PlayerCamera.transform.eulerAngles -= new Vector3(rotValueX, 0f, 0f);
            if(timer > 0.1f)
            {
                StartCoroutine(RecoilDown());
                StopCoroutine(ReCoilCoroutine);
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator RecoilDown()
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

            PlayerCamera.transform.eulerAngles += new Vector3(rotValueX, 0f, 0f);
            float desiredX = PlayerCamera.transform.eulerAngles.x;
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
            yield return null;
        }
    }
    private void UpdateAnimation()
    {
        _PlayerAnimator.SetBool("IsMove", IsMove);
        _PlayerAnimator.SetFloat("Horizontal", horizontal);
        _PlayerAnimator.SetFloat("Vertical", vertical);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Debug.LogError("Hit Player!!");
        }
    }
}
