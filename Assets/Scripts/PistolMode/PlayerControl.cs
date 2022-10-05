using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviourPun
{
    GameObject playerFollowCam = null;
    [SerializeField]
    GameObject followCameraPos = null;
    [SerializeField] // isMine
    Animator anim = null;

    [SerializeField]
    Transform PlayerChest = null;

    Rigidbody rb = null;

    PlayerAudio PA = null;

    // 자식 오브젝트에 있는 rigidBody
    // = ragdoll 
    Rigidbody[] rbChild = new Rigidbody[13];

    BullCount bulletUI = null;

    // 승리 UI
    GunFightSceneUI gunFightSceneUI = null;

    GameManager gameManager = null;

    float mouseX = 0f;
    float inputMouseY = 0f;
    float mouseY = 0f;
    float punMouseY = 0f;

    float limitMinMouseY = -40f;
    float limitMaxMouseY = 40f;

    float rotateSpeed = 100f;

    float moveX = 0f;
    float moveZ = 0f;
    float moveSpeed = 3f;

    int curBullet = 1;
    int maxBullet = 6;

    bool isStart = false;
    bool attackDelay = false;
    bool attackAble = true;

    // Lobby Scene일 경우 기능 활성화를 위한 변수 선언 및 초기화
    bool playerLobbyActive = false;

    bool isAlive = true;

    // Particle 호출
    [SerializeField]
    GameObject MF = null;


    /*
        Ray rayCamera;
        RaycastHit rayHit;
    */

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        PA = GetComponent<PlayerAudio>();

        MF.GetComponent<ParticleSystem>().Play();

        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView가 IsMine 일때만 실행 

        // GunFight Scene : other Client Player error -> photonView Other isKinematic == false 
        photonView.RPC("PhyPlayer", RpcTarget.OthersBuffered);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerFollowCam = GameObject.Find("PlayerFollowCam");
        // Find : 최초 1회 사용은 괜찮지만 Update에 사용시 성능저하의 원인
        // followCameraPos = transform.GetChild(2).gameObject; 

        // 자식 오브젝트에 붙어있는 rigidBody
        rbChild = GetComponentsInChildren<Rigidbody>();

        if (SceneManager.GetActiveScene().name == "GunFight")
        {
            bulletUI = GameObject.Find("CurBullet").GetComponent<BullCount>();
            gunFightSceneUI = GameObject.Find("GunFightUI").GetComponent<GunFightSceneUI>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

    void Start()
    {
        MF.SetActive(false);

        // Player의 photonView가 IsMine일때 PlayerChest에 BoneTransform 넣기
        if (anim && photonView.IsMine)
        {
            PlayerChest = anim.GetBoneTransform(HumanBodyBones.Chest);
        }
        /*
                // Player의 photonView가 IsMine==false (나에게 보여지는 상대방 Player Clone)일때 BoneTransform 넣기
                if (anim && photonView.IsMine == false)
                {
                    OtherPlayerChest = anim.GetBoneTransform(HumanBodyBones.Chest);
                }
        */
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView가 IsMine 일때만 실행 


        Invoke("Unlock", 3f); // 시작 3초뒤 마우스 회전 활성화

        // Debug.Log("player     " + transform.position);



        // ragdoll의 isKinematic : true
        // Player Death 전에 isKinematic false로 변환 필요
        for (int i = 0; i < rbChild.Length; i++)
        {
            if (i >= 1)
            {
                rbChild[i].isKinematic = true;
            }
        }

        // 현재 Scene을 확인하여 Player 기능 온/오프
        CurSceneFind();
    }

    void Update()
    {
        // Debug.Log("player     " + transform.position);
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView가 IsMine 일때만 실행 

        FollowCameraTrans();

        if (attackAble && attackDelay)
        {
            PlayerAttack();
        }
        if (playerLobbyActive)
        {
            PlayerMove();
        }

        // trans to PUNRpc : MouseY
        // InPutMouseY();
    }

    // 진행 순서 : Update -> animation -> LateUpdate
    // chest 회전 후 animation에 의해 초기화되는 경우를 방지하기 위해 LaUpdate 사용 
    private void LateUpdate()
    {
        // Player PhotonView == IsMine
        if (isStart && isAlive && photonView.IsMine)
        {
            PlayerRotate();
        }

        // PRC target.ohter로 상대방에게 보여지는 PlayerClone에 Mouse Y값 누적
        // 상대방의 누적된 mouseY 값을 내 Client의 상대방 Player Clone에 반영 
        // else if (isStart && isAlive && photonView.IsMine == false)
        else if (photonView.IsMine == false)
        {
            // 나에게 보여질 상대방의 mouseY 값이 누적된 함수 호출 필요
            InPutMouseY();
        }
    }

    void Unlock()
    {
        isStart = true;
        attackDelay = true;
    }


    void PlayerRotate()
    {
        mouseX += Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;
        inputMouseY = Input.GetAxis("Mouse Y");
        mouseY += inputMouseY * Time.deltaTime * rotateSpeed;

        if (mouseY < -360) mouseY += 360;
        if (mouseY > 360) mouseY -= 360;

        // 회전각 제한
        mouseY = Mathf.Clamp(mouseY, limitMinMouseY, limitMaxMouseY);

        // Debug.Log(mouseY);

        // 제한된 mouseY을 입력받아 Chest 회전
        PlayerChest.transform.localEulerAngles = new Vector3(0, 0, -mouseY);

        transform.rotation = Quaternion.Euler(0, mouseX, 0);

        photonView.RPC("PlayerRotateY", RpcTarget.OthersBuffered, inputMouseY);
    }

    void InPutMouseY()
    {
        if (punMouseY < -360) punMouseY += 360;
        if (punMouseY > 360) punMouseY -= 360;

        // 회전각 제한
        punMouseY = Mathf.Clamp(punMouseY, limitMinMouseY, limitMaxMouseY);

        PlayerChest.transform.localEulerAngles = new Vector3(0, 0, -punMouseY);
    }

    void PlayerMove()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        /*
                if (moveX != 0 || moveZ != 0)
                {
                    transform.position += new Vector3(moveX, 0, moveZ);
                }
        */
        Vector3 moveVec = (transform.forward * moveZ + transform.right * moveX).normalized;
        if (moveX != 0 || moveZ != 0)
        {
            rb.position += moveVec * moveSpeed * Time.deltaTime;
            //transform.Translate(new Vector3(moveX, 0, moveZ).normalized * moveSpeed * Time.deltaTime);

            anim.SetBool("isRun", true);

            // PlayerWalk Audio
            PA.WalkAudio();
        }

        else if (moveX == 0 && moveZ == 0)
        {
            anim.SetBool("isRun", false);
        }


    }

    void FollowCameraTrans()
    {
        // 마우스 이동에 따른 회전
        playerFollowCam.transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

        // 카메라의 위치를 특정위치(followCameraPos)로 변환 (시점 고정)
        playerFollowCam.transform.position = followCameraPos.transform.position;
    }

    void PlayerAttack()
    {

        if (Input.GetButtonDown("Fire1") && playerLobbyActive)
        {
            photonView.RPC("FireAnim", RpcTarget.AllBuffered);

            attackDelay = false;
            Invoke("AttackDelay", 0.5f); // 재장전 시간 0.5초
            Debug.Log("[로비] 발사");

            GunFire();
        }

        else if (Input.GetButtonDown("Fire1") && !playerLobbyActive && isAlive)
        {
            if (curBullet == maxBullet)
            {
                attackAble = false;
            }

            anim.SetTrigger("isAttack");
            attackDelay = false;

            // Reload UI 호출
            StartCoroutine(gunFightSceneUI.Reload());

            Invoke("AttackDelay", 2f); // 재장전 시간 2초
            GunFire();

            bulletUI.countBullet(curBullet);
            curBullet++;
        }
    }

    void AttackDelay()
    {
        attackDelay = true;
    }

    void GunFire()
    {
        // Revolver Fire Audio
        PA.FireAudio();

        MF.gameObject.GetComponent<Muzzleflash>().MuzzleflashActive();

        // rayCamera = Camera.main.ViewportPointToRay(transform.position);
        Debug.DrawRay(playerFollowCam.transform.position, playerFollowCam.transform.forward * 50f, Color.red);

        if (Physics.Raycast(playerFollowCam.transform.position, playerFollowCam.transform.forward * 5f, out RaycastHit rayHit, Mathf.Infinity))
        {
            if (rayHit.transform.tag == "Player" && SceneManager.GetActiveScene().name != "Lobby")
            {
                Debug.Log("죽음");

                // 피격받은 플레이어 LOSE UI 호출
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("IsHit", RpcTarget.All);

                // Win UI 호출
                gunFightSceneUI.ResultWin();
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBuffered);
            }

            if (rayHit.transform.tag == "SaloonObject")
            {
                //rayHit.rigidbody.isKinematic = false;

                // SaloonObject 피격 시 playerFollowCam.transform.forward로 AddForce
                // rayHit.transform.gameObject.GetComponent<Rigidbody>().AddForce(playerFollowCam.transform.forward * 10f, ForceMode.Impulse);
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("FireHit", RpcTarget.AllBuffered, playerFollowCam.transform.forward * 10f);
            }
        }
    }

    [PunRPC]
    void IsHit()
    {
        if (photonView.IsMine)
        {
            gunFightSceneUI.ResultLose();
        }
    }


    public void LobbyPlayerActive()
    {
        // 로비 씬으로 전환됐을 때 플레이어 
        playerLobbyActive = true;
    }

    public void GunFightPlayerActive()
    {
        // 로비 씬으로 전환됐을 때 플레이어 
        playerLobbyActive = false;
    }

    void CurSceneFind()
    {
        // Lobby Scene 플레이어 일부 기능 활성화 
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            Debug.Log("로비 입장");
            LobbyPlayerActive();
            Unlock(); // 플레이어 회전 활성화
            // gameSceneLogic.LobbyPos();
        }

        else if (SceneManager.GetActiveScene().name == "GunFight")
        {
            // Lobby씬에서 활성화된 isStart, attackDelay 비활성화
            isStart = false;
            attackDelay = false;

            Invoke("Unlock", 3f); // 3초 뒤 플레이어 isStart, attackDelay

            GunFightPlayerActive();
            //gameSceneLogic.GunFightPos();
        }
    }

    [PunRPC]
    void FireAnim()
    {
        anim.SetTrigger("isAttack");
    }

    [PunRPC]
    void AnimControl()
    {
        isAlive = false;

        Debug.Log($"isAlive  : {isAlive}");

        anim.SetTrigger("isDeath");
        //anim.enabled = false;        

        Destroy(gameObject, 4f);

    }

    [PunRPC]
    void PlayerRotateY(float inputMouseY)
    {

        punMouseY += inputMouseY;

        //Debug.Log("Chest 움직임 : " + inputMouseY);
    }

    [PunRPC]
    void PhyPlayer()
    {
        rb.isKinematic = true;
    }

}

