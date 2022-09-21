using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


public class PlayerControl : MonoBehaviourPun
{
    GameObject playerFollowCam = null;
    [SerializeField]
    GameObject followCameraPos = null;
    [SerializeField]
    Animator anim = null;

    float mouseX = 0f;
    float rotateSpeed = 100f;

    float moveX = 0f;
    float moveY = 0f;
    float moveSpeed = 10f;

    int curBullet = 1;
    int maxBullet = 6;

    bool isStart = false;
    bool attackDelay = false;
    bool attackAble = true;

    // Lobby Scene일 경우 기능 활성화를 위한 변수 선언 및 초기화
    bool playerLobbyActive = false;

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
    }

    void Start()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView가 IsMine 일때만 실행 

        Invoke("Unlock", 3f); // 시작 3초뒤 마우스 회전 활성화
    }

    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView가 IsMine 일때만 실행 

        FollowCameraTrans();

        if (isStart)
        {
            PlayerRotate();
        }
        if (attackAble && attackDelay)
        {
            PlayerAttack();
        }
        if (playerLobbyActive)
        {
            PlayerMove();
            Debug.Log("플레이어 이동 준비됨");
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
        transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }

    void PlayerMove()
    {
        moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.position = new Vector3(moveX, moveY, 0);
    }

    void FollowCameraTrans()
    {
        // 마우스 이동에 따른 회전(Y축)
        playerFollowCam.transform.rotation = Quaternion.Euler(0, mouseX, 0);
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
            RayCamera();
        }

        else if (Input.GetButtonDown("Fire1")&& !playerLobbyActive)
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
            RayCamera();

            curBullet++;

        }
    }

    void AttackDelay()
    {
        attackDelay = true;
    }


    void RayCamera()
    {
        // rayCamera = Camera.main.ViewportPointToRay(transform.position);
        Debug.DrawRay(playerFollowCam.transform.position, playerFollowCam.transform.forward * 50f, Color.red);

        if (Physics.Raycast(playerFollowCam.transform.position, playerFollowCam.transform.forward * 5f, out RaycastHit rayHit, Mathf.Infinity))
        {
            if (rayHit.transform.tag == "Player")
            {
                Debug.Log("죽음");
            }
        }


        /*
        if (rayHit.transform.tag == "Player")
        {
            Debug.Log("죽음");
        }
        */
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
}
