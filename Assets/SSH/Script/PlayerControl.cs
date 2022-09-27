using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class PlayerControl : MonoBehaviourPun
{
    GameObject playerFollowCam = null;
    [SerializeField]
    GameObject followCameraPos = null;
    [SerializeField]
    Animator anim = null;

    Transform PlayerChest;
    Rigidbody rb;

    // 자식 오브젝트에 있는 rigidBody
    // = ragdoll
    Rigidbody[] rbChild = new Rigidbody[13];

    BullCount bulletUI = null;

    float mouseX = 0f;
    float mouseY = 0f;

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

    /*
        Ray rayCamera;
        RaycastHit rayHit;
    */



    private void Awake()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView가 IsMine 일때만 실행 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerFollowCam = GameObject.Find("PlayerFollowCam");
        // Find : 최초 1회 사용은 괜찮지만 Update에 사용시 성능저하의 원인
        // followCameraPos = transform.GetChild(2).gameObject; 

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // 자식 오브젝트에 붙어있는 rigidBody
        rbChild = GetComponentsInChildren<Rigidbody>();

        if (SceneManager.GetActiveScene().name == "GunFight")
        {
            bulletUI = GameObject.Find("CurBullet").GetComponent<BullCount>();
        }
    }

    void Start()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView가 IsMine 일때만 실행 

        Invoke("Unlock", 3f); // 시작 3초뒤 마우스 회전 활성화

        Debug.Log("player     " + transform.position);

        // PlayerChest에 BoneTransform 넣기
        if (anim)
        {
            PlayerChest = anim.GetBoneTransform(HumanBodyBones.Chest);
        }

        // ragdoll의 isKinematic : true
        // Player Death 전에 isKinematic false로 변환 필요
        for (int i = 0; i < rbChild.Length ; i++ )
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
    }

    // 진행 순서 : Update -> animation -> LateUpdate
    // chest 회전 후 animation에 의해 초기화되는 경우를 방지하기 위해 LaUpdate 사용 
    private void LateUpdate()
    {
        if (isStart && isAlive)
        {
            PlayerRotate();
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
        mouseY += Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed;

        if (mouseY < -360) mouseY += 360;
        if (mouseY > 360) mouseY -= 360;

        // 회전각 제한
        mouseY = Mathf.Clamp(mouseY, limitMinMouseY, limitMaxMouseY);

        // 제한된 mouseY을 입력받아 Chest 회전
        PlayerChest.transform.localEulerAngles = new Vector3(0, 0, -mouseY);

        transform.rotation = Quaternion.Euler(0, mouseX, 0);        
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
        }

        else if(moveX == 0 && moveZ == 0)
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
            anim.SetTrigger("isAttack");
            attackDelay = false;
            Invoke("AttackDelay", 0.5f); // 재장전 시간 0.5초
            Debug.Log("[로비] 발사");
            
            GunFire();
        }

        else if (Input.GetButtonDown("Fire1") && !playerLobbyActive && isAlive)
        {
            if (curBullet == maxBullet)
            {
                Debug.Log("으앙 다씀");
                attackAble = false;
            }

            anim.SetTrigger("isAttack");
            attackDelay = false;

            Invoke("AttackDelay", 2f); // 재장전 시간 2초
            Debug.Log("발사");
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
        // rayCamera = Camera.main.ViewportPointToRay(transform.position);
        Debug.DrawRay(playerFollowCam.transform.position, playerFollowCam.transform.forward * 50f, Color.red);

        if (Physics.Raycast(playerFollowCam.transform.position, playerFollowCam.transform.forward * 5f, out RaycastHit rayHit, Mathf.Infinity))
        {
            if (rayHit.transform.tag == "Player")
            {
                Debug.Log("죽음");

                //rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBuffered);
                //rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBuffered);
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBufferedViaServer);

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
            // gameSceneLogic.GunFightPos();
        }
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
}

