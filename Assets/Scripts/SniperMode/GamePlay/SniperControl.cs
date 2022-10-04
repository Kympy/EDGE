using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent (typeof(SniperAudio))]

public class SniperControl : PlayerHeader, IPunObservable
{
    [SerializeField] private GameObject[] changedObjects = new GameObject[12];
    private SniperGameManager gameManager = null;
    private Coroutine CrouchCo = null;

    #region Variables
    private float UpperRotation = 0f;
    private int ZoomLevel = 0;
    public float min;
    public float max;
    public GameObject NamePos = null; // Player Nickname Position
    private bool IsDead = false;
    // Player Control Values
    public bool Is_Move { get { return IsMove; } }
    public bool Is_Fire { get { return IsFire; } }
    #endregion

    private void Awake()
    {
        SetLayer();
        gameManager = GameObject.FindObjectOfType<SniperGameManager>();
        gameManager.PlayerList.Add(this.gameObject); // Add Me On Player List
        HP = MaxHP;
        DeathCam.enabled = false;
        if (photonView.IsMine == false) return;
        photonView.RPC("RPC_RigidbodyDisable", RpcTarget.OthersBuffered);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        for (int i = 0; i < changedObjects.Length; i++)
        {
            changedObjects[i].layer = LayerMask.NameToLayer("MyServerPlayer");
        }        
    }
    private void Start()
    {
        if (photonView.IsMine == false) return;

        RagdollToggle(true);
        // Components
        BrainCam = GameObject.Find("BrainCam").GetComponent<Camera>();
        _PlayerAudio = GetComponent<SniperAudio>();
        _PlayerAnimator = GetComponent<Animator>();
        UpperBody = _PlayerAnimator.GetBoneTransform(HumanBodyBones.Spine);
        _Rigidbody = GetComponent<Rigidbody>();
        // Virtual Arm
        PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
        _ArmAnimator = PlayerCamera.GetComponentInChildren<Animator>();
        Arm = GameObject.Find("PlayerArmPivot");
        // Positions
        ScopeCamera = PlayerCamera.gameObject.GetComponent<PlayerCamCompo>().ScopeCamera;
        ZoomInPos = GameObject.Find("ZoomInPos").transform;
        ZoomOutPos = GameObject.Find("ZoomOutPos").transform;
        CasingPos = GameObject.Find("CasingPos").transform;
        shootPos = GameObject.Find("ShootPos").transform;
        ZoomShootPosition = GameObject.Find("ZoomShootPos").transform;
        // Effects
        ArmCasing = PlayerCamera.gameObject.GetComponent<PlayerCamCompo>().ArmCasing;
        RealSmoke = PlayerCamera.gameObject.GetComponent<PlayerCamCompo>().RealSmoke;
        RealMuzzle = PlayerCamera.gameObject.GetComponent<PlayerCamCompo>().RealMuzzle;
        //ArmCasing = GameObject.Find("ArmCasing");
        //Initializing Active State
        //ArmCasing.SetActive(false);

        FakeMuzzle.SetActive(false);
        FakeSmoke.SetActive(false);
        // Set Arm Position
        Arm.transform.localPosition = ZoomOutPos.localPosition;
        
        if (ScopeCamera.gameObject.TryGetComponent<CinemachineBrain>(out CinemachineBrain brain))
        {
            brain.enabled = false;
        }
    } // Find Components & Initialize

    private void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        if (IsDead) return;

        PlayerCamera.transform.position = PlayerCameraPos.position;
        DoMovement();
        photonView.RPC("UpdateServerBone", RpcTarget.AllBuffered, -mouseYUpper);
    }
    private void Update()
    {
        if (photonView.IsMine == false) return;
        if (IsDead) return;

        InputMovement();
        CameraRotation();
        ZoomScope();
        UpdateZoomValue();
        Fire();
        Crouch();
        UpdateAnimation();
    }
    private void LateUpdate()
    {
        if (IsDead) return;
        if (photonView.IsMine == false)
        {
            UpperRotation = UpperRotation > 180f ? UpperRotation - 360f : UpperRotation;
            UpperRotation = Mathf.Clamp(UpperRotation, -50f, 70f);
            UpperBody.eulerAngles = new Vector3(UpperBody.eulerAngles.x, UpperBody.eulerAngles.y, UpperBody.eulerAngles.z + UpperRotation);
        }
    }
    [PunRPC]
    private void RagdollToggle(bool toggle)
    {
        _PlayerAnimator.enabled = toggle;
        for (int i = 0; i < Bones.Length; i++)
        {
            Bones[i].isKinematic = toggle;
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
                PlayerCamera.m_Lens.FieldOfView -= 4f;
                if (PlayerCamera.m_Lens.FieldOfView < 9f)
                {
                    PlayerCamera.m_Lens.FieldOfView = 9f;
                }
                if (Arm.transform.localPosition == ZoomInPos.transform.localPosition && PlayerCamera.m_Lens.FieldOfView == 9f)
                {
                    yield break;
                }
            }
            else
            {
                Arm.transform.localPosition = Vector3.MoveTowards(Arm.transform.localPosition, ZoomOutPos.localPosition, Time.deltaTime * 2f);
                PlayerCamera.m_Lens.FieldOfView += 4f;
                if (PlayerCamera.m_Lens.FieldOfView > 60f)
                {
                    PlayerCamera.m_Lens.FieldOfView = 60f;
                }
                if (Arm.transform.localPosition == ZoomOutPos.transform.localPosition && PlayerCamera.m_Lens.FieldOfView == 60f)
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
            else if(ScopeCamera.fieldOfView < 0.1f)
            {
                ScopeCamera.fieldOfView = 0.1f;
            }
        }
    }
    private void ZoomScope()
    {
        if(Input.GetKeyDown(KeyCode.R) && IsZoom)
        {
            ZoomLevel++;
            if(ZoomLevel > 2)
            {
                ZoomLevel = 0;
            }

            switch (ZoomLevel)
            {
                case 0:
                    {
                        ScopeCamera.fieldOfView = 10f;
                        break;
                    }
                case 1:
                    {
                        ScopeCamera.fieldOfView = 5f;
                        break;
                    }
                case 2:
                    {
                        ScopeCamera.fieldOfView = 1f;
                        break;
                    }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            IsZoom = !IsZoom;
            //if (IsZoom) BrainCam.cullingMask = ScopeCamCulling;
            //else BrainCam.cullingMask = PlayerCamCulling;
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
            mouseYUpper = Input.GetAxis("Mouse Y") * zoomMouseSpeed * Time.deltaTime;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
            mouseYUpper = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        }

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
                PhotonNetwork.Instantiate("SniperMode/Bullets", ZoomShootPosition.position, ZoomShootPosition.rotation);
            }
            else
            {
                PhotonNetwork.Instantiate("SniperMode/Bullets", shootPos.position, shootPos.rotation);

                if (ReCoilCoroutine != null)
                {
                    StopCoroutine(ReCoilCoroutine);
                    ReCoilCoroutine = null;
                }
                ReCoilCoroutine = StartCoroutine(ReCoilUp());
            }
            Invoke("Casing", 1f);
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
        PhotonNetwork.Instantiate("SniperMode/Case", CasingPos.position, CasingPos.rotation);
    }
    #endregion
    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            IsCrouch = true;
            if(CrouchCo != null)
            {
                StopCoroutine(CrouchCo);
            }
            PlayerCameraPos.transform.localPosition = new Vector3(0f, Mathf.Lerp(PlayerCameraPos.transform.localPosition.y, 0.908f, 0.5f), 0f);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            IsCrouch = false;
            CrouchCo = StartCoroutine(StandUp());
        }
    }
    private IEnumerator StandUp()
    {
        while(true)
        {
            PlayerCameraPos.transform.localPosition += new Vector3(0f, 0.1f, 0f);
            
            if(PlayerCameraPos.transform.localPosition.y > 1.7f)
            {
                PlayerCameraPos.transform.localPosition = new Vector3(0f, 1.7f, 0f);
                CrouchCo = null;
                yield break;
            }
            
            yield return null;
        }
    }
    #region Recoil Function
    private IEnumerator ReCoilUp()
    {
        float rotValueX = 0f;
        float timer = 0f;
        while(true)
        {
            timer += Time.fixedDeltaTime;
            rotValueX += 0.1f;
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
            rotValueX += 0.09f;
            recoilPower = rotValueX;

            if (timer > 0.2f)
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
        _PlayerAnimator.SetBool("IsCrouch", IsCrouch);
    } // Control Animator

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
        if(stream.IsWriting)
        {
            stream.SendNext(FakeMuzzle.activeSelf);
            stream.SendNext(FakeSmoke.activeSelf);
            //stream.SendNext(UpperBody.eulerAngles);
        }
        else
        {
            FakeMuzzle.SetActive((bool)stream.ReceiveNext());
            FakeSmoke.SetActive((bool)stream.ReceiveNext());
            //UpperRotation = (Vector3)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void GetDamage(float damage)
    {
        HP -= damage;
        SniperGameManager.Instance.GetUI.ShowBlood();
        Debug.Log(HP);
        if(HP <= 0f && IsDead == false)
        {
            IsDead = true;
            HP = 0f;
            DeathCam.enabled = true;
            BrainCam.cullingMask = DeathCamCulling;
            photonView.RPC("RagdollToggle", RpcTarget.All, false); // Ragdoll mode ON
            gameManager.gameObject.GetPhotonView().RPC("GameEnd", RpcTarget.MasterClient, ODINAPIHandler.Winner.Other);
        }
        SniperGameManager.Instance.GetUI.UpdateHP(HP, MaxHP); // Update UI
    }
    [PunRPC]
    public void UpdateServerBone(float rotation)
    {
        UpperRotation += rotation;
    }
    [PunRPC]
    public void RPC_RigidbodyDisable()
    {
        _Rigidbody.isKinematic = true;
    }
    private void SetLayer()
    {
        PlayerCamCulling = 1 << LayerMask.NameToLayer("Default") |
        1 << LayerMask.NameToLayer("TransparentFX") |
        1 << LayerMask.NameToLayer("Igonre Raycast") |
        1 << LayerMask.NameToLayer("Water") |
        1 << LayerMask.NameToLayer("UI") |
        1 << LayerMask.NameToLayer("Scope") |
        1 << LayerMask.NameToLayer("Player") |
        1 << LayerMask.NameToLayer("PlayerArm") |
        1 << LayerMask.NameToLayer("DeadZone") |
        1 << LayerMask.NameToLayer("Bullet");

        ScopeCamCulling = 1 << LayerMask.NameToLayer("Default") |
        1 << LayerMask.NameToLayer("TransparentFX") |
        1 << LayerMask.NameToLayer("Igonre Raycast") |
        1 << LayerMask.NameToLayer("Water") |
        1 << LayerMask.NameToLayer("UI") |
        1 << LayerMask.NameToLayer("Scope") |
        1 << LayerMask.NameToLayer("Player") |
        1 << LayerMask.NameToLayer("DeadZone") |
        1 << LayerMask.NameToLayer("Bullet");

        DeathCamCulling = 1 << LayerMask.NameToLayer("Default") |
        1 << LayerMask.NameToLayer("TransparentFX") |
        1 << LayerMask.NameToLayer("Igonre Raycast") |
        1 << LayerMask.NameToLayer("Water") |
        1 << LayerMask.NameToLayer("UI") |
        1 << LayerMask.NameToLayer("Scope") |
        1 << LayerMask.NameToLayer("Player") |
        1 << LayerMask.NameToLayer("DeadZone") |
        1 << LayerMask.NameToLayer("Bullet") | 
        1 << LayerMask.NameToLayer("MyServerPlayer");
    }
}
