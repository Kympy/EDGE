using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

[RequireComponent (typeof(PlayerAudio))]

public class SniperControl : PlayerHeader, IPunObservable
{
    #region Variables
    private Vector3 UpperRotation;
    private bool DevMode = false;
    private Text mode = null;

    private bool FakeMuzzleActive = false;
    private bool FakeSmokeActive = false;

    public GameObject NamePos = null; // Player Nickname Position
    // Player Control Values
    public bool Is_Move { get { return IsMove; } }
    public bool Is_Fire { get { return IsFire; } }
    #endregion

    private void Awake()
    {
        if (photonView.IsMine == false) return;
        mode = GameObject.Find("Dev").GetComponent<Text>();
        mode.text = "DevMode : " + DevMode.ToString();
        HP = MaxHP;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        if (photonView.IsMine == false) return;

        // Components
        _PlayerAudio = GetComponent<PlayerAudio>();
        _PlayerAnimator = GetComponent<Animator>();
        _Rigidbody = GetComponent<Rigidbody>();
        // Virtual Arm
        PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        _ArmAnimator = PlayerCamera.GetComponentInChildren<Animator>();
        Arm = GameObject.Find("PlayerArmPivot");
        // Positions
        //PlayerCameraPos = GameObject.Find("CameraPosition").transform;
        ScopeCamera = GameObject.Find("ScopeCamera").GetComponent<Camera>();
        ZoomInPos = GameObject.Find("ZoomInPos").transform;
        ZoomOutPos = GameObject.Find("ZoomOutPos").transform;
        CasingPos = GameObject.Find("CasingPos").transform;
        shootPos = GameObject.Find("ShootPos").transform;
        ZoomShootPosition = GameObject.Find("ZoomShootPos").transform;
        // Bone
        //UpperBody = _PlayerAnimator.GetBoneTransform(HumanBodyBones.Spine);
        // Effects
        RealSmoke = GameObject.Find("RealSmoke");
        //FakeMuzzle = GameObject.Find("FakeMuzzle");
        //FakeSmoke = GameObject.Find("FakeSmoke");
        RealMuzzle = GameObject.Find("RealMuzzle");
        ArmCasing = GameObject.Find("ArmCasing");

        ArmCasing.SetActive(false);
        RealSmoke.SetActive(false);
        RealMuzzle.SetActive(false);

        FakeMuzzle.SetActive(false);
        FakeSmoke.SetActive(false);
        // Set Arm Position
        Arm.transform.localPosition = ZoomOutPos.localPosition;
    } // Find Components & Initialize

    private void FixedUpdate()
    {
        if (photonView.IsMine == false) return;

        PlayerCamera.transform.position = PlayerCameraPos.position;
        DoMovement();
        photonView.RPC("UpdateServerBone", RpcTarget.AllBuffered, new Vector3(UpperBody.eulerAngles.x, UpperBody.eulerAngles.y, -mouseYUpper));
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
        DevModeToggle();
        UpdateEffectToggle();
    }
    private void LateUpdate()
    {
        if (photonView.IsMine == false)
        {
            //UpperBody.eulerAngles = UpperRotation;
        }
    }
    private void DevModeToggle()
    {
        if(Input.GetKeyDown(KeyCode.F2))
        {
            DevMode = !DevMode;
            mode.text = "DevMode : " + DevMode.ToString();
        }
    }
    #region Zoom Function
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
                if (PlayerCamera.fieldOfView > 60f)
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
    #endregion

    #region Movement
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
            _PlayerAudio.WalkSound();
            moveVector = (horizontal * transform.right + vertical * transform.forward).normalized;
            _Rigidbody.position += moveSpeed * Time.deltaTime * moveVector;
        }
    }
    #endregion
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


        mouseYUpper = Mathf.Clamp(mouseYUpper, 20f, 150f); // mouse Y + 90f

        PlayerCamera.transform.eulerAngles += new Vector3(-mouseY + recoilPower, mouseX, 0f);

        ClampedX = PlayerCamera.transform.eulerAngles.x;
        ClampedX = ClampedX > 180f ? ClampedX - 360f : ClampedX;
        ClampedX = Mathf.Clamp(ClampedX, -50f, 70f);

        transform.eulerAngles += new Vector3(0f, mouseX, 0f);
        PlayerCamera.transform.eulerAngles = new Vector3(ClampedX, transform.eulerAngles.y, 0f);
    }
    #region Fire Function
    private void Fire()
    {
        if (Input.GetMouseButtonDown(0) && IsFire == false)
        {
            _PlayerAudio.FireSound();
            _PlayerAnimator.SetBool("IsFire", true);
            IsFire = true;
            _ArmAnimator.SetTrigger("Fire"); // Play Animation
            // Arm Effect
            RealMuzzle.SetActive(true);
            RealSmoke.SetActive(true);
            // Player Effect
            FakeMuzzle.SetActive(true);
            FakeSmoke.SetActive(true);

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
    }
    public void Reload() // Animation Event
    {
        _PlayerAnimator.SetBool("IsFire", false);
        IsFire = false;
    }
    public void Casing() // Animation Event
    {
        ArmCasing.SetActive(true);
        PhotonNetwork.Instantiate("Case", CasingPos.position, CasingPos.rotation);
    }
    #endregion

    #region Recoil Function
    private IEnumerator ReCoilUp()
    {
        if (DevMode == true) yield break;
        float rotValueX = 0f;
        float timer = 0f;
        while(true)
        {
            timer += Time.fixedDeltaTime;
            if(IsZoom)
            {
                rotValueX += 0.04f;
            }
            else
            {
                rotValueX += 0.12f;
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
            timer += Time.fixedDeltaTime;
            if (IsZoom)
            {
                rotValueX = 0.04f;
            }
            else
            {
                rotValueX = 0.12f;
            }

            recoilPower = rotValueX;

            if (timer > 0.6f)
            {
                recoilPower = 0f;
                yield break;
            }
            yield return null;
        }
    }
    #endregion
    private void UpdateAnimation()
    {
        _PlayerAnimator.SetBool("IsMove", IsMove);
        _PlayerAnimator.SetFloat("Horizontal", horizontal);
        _PlayerAnimator.SetFloat("Vertical", vertical);
    } // Control Animator
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Dirt"))
        {
            
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
        if(stream.IsWriting)
        {
            stream.SendNext(FakeMuzzle.activeSelf);
            stream.SendNext(FakeSmoke.activeSelf);
        }
        else
        {
            FakeMuzzle.SetActive((bool)stream.ReceiveNext());
            FakeSmoke.SetActive((bool)stream.ReceiveNext());
        }
    }

    private void UpdateEffectToggle()
    {
        
    }
    [PunRPC]
    public void UpdateServerBone(Vector3 rotation)
    {
        UpperRotation = rotation;
    }
}
