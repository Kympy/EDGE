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
    private float shootRot; // Fire time's X Rotation
    private float recoilPower; // Recoil X Rotation Value
    // Fire Position
    private Transform shootPos;
    private Transform ZoomShootPosition;
    // Player Control Booleans
    private bool IsZoom = false;
    // Keyboard Input Checker
    private bool hasHorizontalInput;
    private bool hasVerticalInput;
    private bool IsMove;
    // Camera
    private Camera PlayerCamera; // Player Following Camera
    private Camera ScopeCamera; // Sniper rifle scope
    private float ClampedX = 0f; // Fixed Camera X rotation
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
    // Effect
    private GameObject RealSmoke = null; // Player Arm smoke
    private GameObject FakeSmoke = null; // Player body smoke
    private GameObject RealMuzzle = null; // Player Arm muzzle
    private GameObject FakeMuzzle = null; // Player body muzzle
    private void Awake()
    {
        if (photonView.IsMine == false) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        if (photonView.IsMine == false) return;

        PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        _ArmAnimator = PlayerCamera.GetComponentInChildren<Animator>();
        _PlayerAnimator = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();
        ZoomInPos = GameObject.Find("ZoomInPos").transform;
        ZoomOutPos = GameObject.Find("ZoomOutPos").transform;
        PlayerCameraPos = GameObject.Find("CameraPosition").transform;

        ScopeCamera = GameObject.Find("ScopeCamera").GetComponent<Camera>();

        UpperBody = _PlayerAnimator.GetBoneTransform(HumanBodyBones.Spine);

        Arm = GameObject.Find("PlayerArmPivot");
        shootPos = GameObject.Find("ShootPos").transform;
        ZoomShootPosition = GameObject.Find("ZoomShootPos").transform;

        RealSmoke = GameObject.Find("RealSmoke");
        FakeSmoke = GameObject.Find("FakeSmoke");
        RealMuzzle = GameObject.Find("RealMuzzle");
        FakeMuzzle = GameObject.Find("FakeMuzzle");

        RealSmoke.SetActive(false);
        FakeSmoke.SetActive(false);
        RealMuzzle.SetActive(false);
        FakeMuzzle.SetActive(false);

        Arm.transform.localPosition = ZoomOutPos.localPosition;
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        PlayerCamera.transform.position = PlayerCameraPos.position;
        DoMovement();
    }
    private void Update()
    {
        if (photonView.IsMine == false) return;

        InputMovement();
        CameraRotation();
        ZoomScope();
        UpdateZoomValue();
        Fire();
        UpdateAnimation();
    }
    private void LateUpdate()
    {
        if (photonView.IsMine == false) return;

        UpperBody.eulerAngles = new Vector3(0f, UpperBody.eulerAngles.y, -mouseYUpper);
    }
    private void UpdateZoomValue()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && IsZoom)
        {
            ScopeCamera.fieldOfView += -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
            if (ScopeCamera.fieldOfView > 10f)
            {
                ScopeCamera.fieldOfView = 10f;
            }
        }
    }
    private void ZoomScope()
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsZoom = !IsZoom;
            if (ZoomCoroutine != null)
            {
                StopCoroutine(ZoomCoroutine);
                ZoomCoroutine = null;
            }
            ZoomCoroutine = StartCoroutine(Zoom());
        }
    }
    private void CameraRotation() // Mouse Rotation + Recoil Power
    {
        if (IsZoom)
        {
            mouseX = Input.GetAxis("Mouse X") * zoomMouseSpeed * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * zoomMouseSpeed * Time.deltaTime;
            mouseYUpper += Input.GetAxis("Mouse Y") * zoomMouseSpeed * Time.deltaTime;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
            mouseYUpper += Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        }

        //mouseX = mouseX > 180f ? mouseX - 360f : mouseX;
        //mouseY = mouseY > 180f ? mouseY - 360f : mouseY;
        mouseYUpper = mouseYUpper > 180f ? mouseYUpper - 360f : mouseYUpper;


        mouseYUpper = Mathf.Clamp(mouseYUpper, 40f, 160f); // mouse Y + 90f

        PlayerCamera.transform.eulerAngles += new Vector3(-mouseY + recoilPower, mouseX, 0f);

        ClampedX = PlayerCamera.transform.eulerAngles.x;
        ClampedX = ClampedX > 180f ? ClampedX - 360f : ClampedX;
        ClampedX = Mathf.Clamp(ClampedX, -50f, 70f);

        transform.eulerAngles += new Vector3(0f, mouseX, 0f);
        PlayerCamera.transform.eulerAngles = new Vector3(ClampedX, transform.eulerAngles.y, 0f);
    }
    private void InputMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        IsMove = hasHorizontalInput || hasVerticalInput;
    }
    private void DoMovement()
    {
        if (IsMove)
        {
            moveVector = (horizontal * transform.right + vertical * transform.forward).normalized;
            //transform.Translate(moveSpeed * Time.deltaTime * moveVector);
            _Rigidbody.position += moveSpeed * Time.deltaTime * moveVector;
        }
    }
    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _PlayerAnimator.SetTrigger("Fire");
            _ArmAnimator.SetTrigger("Fire"); // Play Animation
            RealSmoke.SetActive(true);
            FakeSmoke.SetActive(true);
            RealMuzzle.SetActive(true);
            FakeMuzzle.SetActive(true);
            shootRot = PlayerCamera.transform.eulerAngles.x;
            shootRot = shootRot > 180f ? shootRot - 360f : shootRot;
            Debug.Log(shootRot);
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
            recoilPower = -rotValueX;
            if(timer > 0.2f)
            {
                recoilPower = 0f;
                StartCoroutine(RecoilDown());
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator RecoilDown()
    {
        float rotValueX = 0f;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (IsZoom)
            {
                rotValueX = 0.04f;
            }
            else
            {
                rotValueX = 0.2f;
            }

            recoilPower = rotValueX;

            if (timer > 0.2f)
            {
                recoilPower = 0f;
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
        if(collision.gameObject.CompareTag("Dirt"))
        {
            
        }
    }
}
