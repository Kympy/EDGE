using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

[RequireComponent (typeof(PlayerAudio))]

public class SniperControl : PlayerHeader, IPunObservable
{
    [SerializeField] private GameObject[] changedObjects = new GameObject[12];

    private Coroutine CrouchCo = null;

    #region Variables
    private Vector3 UpperRotation;
    private Text mode = null;
    private int ZoomLevel = 0;

    public GameObject NamePos = null; // Player Nickname Position
    // Player Control Values
    public bool Is_Move { get { return IsMove; } }
    public bool Is_Fire { get { return IsFire; } }
    #endregion

    private void Awake()
    {
        GameObject.FindObjectOfType<SniperGameManager>().PlayerList.Add(this.gameObject); // Add Me On Player List
        HP = MaxHP;

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
        _PlayerAudio = GetComponent<PlayerAudio>();
        _PlayerAnimator = GetComponent<Animator>();
        UpperBody = _PlayerAnimator.GetBoneTransform(HumanBodyBones.Spine);
        _Rigidbody = GetComponent<Rigidbody>();
        // Virtual Arm
        PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        _ArmAnimator = PlayerCamera.GetComponentInChildren<Animator>();
        Arm = GameObject.Find("PlayerArmPivot");
        // Positions
        ScopeCamera = GameObject.Find("ScopeCamera").GetComponent<Camera>();
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
    } // Find Components & Initialize

    private void FixedUpdate()
    {
        if (photonView.IsMine == false) return;
        
        PlayerCamera.transform.position = PlayerCameraPos.position;
        DoMovement();
        //photonView.RPC("UpdateServerBone", RpcTarget.AllBuffered, new Vector3(UpperBody.eulerAngles.x, UpperBody.eulerAngles.y, -mouseYUpper));
    }
    private void Update()
    {
        if (photonView.IsMine == false) return;

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
        if (photonView.IsMine == true)
        {
            UpperBody.Rotate(UpperRotation);
        }
    }
    private void RagdollToggle(bool toggle)
    {
        for(int i = 0; i < Bones.Length; i++)
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
            if (ScopeCamera.fieldOfView > 20f)
            {
                ScopeCamera.fieldOfView = 20f;
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
                        ScopeCamera.fieldOfView = 20f;
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

        //mouseX = mouseX > 180f ? mouseX - 360f : mouseX;
        //mouseY = mouseY > 180f ? mouseY - 360f : mouseY;
        //mouseYUpper = mouseYUpper > 180f ? mouseYUpper - 360f : mouseYUpper;
        //mouseYUpper = Mathf.Clamp(mouseYUpper, 20f, 150f); // mouse Y + 90f

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
        PhotonNetwork.Instantiate("Case", CasingPos.position, CasingPos.rotation);
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
            stream.SendNext(new Vector3(mouseYUpper, 0, 0));
        }
        else
        {
            FakeMuzzle.SetActive((bool)stream.ReceiveNext());
            FakeSmoke.SetActive((bool)stream.ReceiveNext());
            UpperRotation = (Vector3)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void GetDamage(float damage)
    {
        HP -= damage;
        SniperGameManager.Instance.GetUI.UpdateHP(HP, MaxHP);
        SniperGameManager.Instance.GetUI.ShowBlood();
        Debug.Log(HP);
    }

    [PunRPC]
    public void UpdateServerBone(Vector3 rotation)
    {
        UpperRotation = rotation;
    }
    [PunRPC]
    public void RPC_RigidbodyDisable()
    {
        _Rigidbody.isKinematic = true;
    }
}
