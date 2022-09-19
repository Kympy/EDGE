using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    GameObject playerFollowCam = null;
    GameObject followCameraPos = null;
    Animator anim = null;

    float mouseX = 0f;
    float rotateSpeed = 100f;

    int curBullet = 0;
    int maxBullet = 6;

    bool isStart = false;
    bool attackDelay = false;

    /*
        Ray rayCamera;
        RaycastHit rayHit;
    */
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerFollowCam = GameObject.Find("PlayerFollowCam");
        followCameraPos = GameObject.Find("CameraPos"); // Find : 최초 1회 사용은 괜찮지만 Update에 사용시 성능저하의 원인
        // followCameraPos = transform.GetChild(2).gameObject; 
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Invoke("Unlock", 3f); // 시작 3초뒤 마우스 회전 활성화
    }

    void Update()
    {
        FollowCameraTrans();

        if (isStart)
        {
            PlayerRotate();
        }
        if (attackDelay)
        {
            PlayerAttack();
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

    void FollowCameraTrans()
    {
        // 마우스 이동에 따른 회전(Y축)
        playerFollowCam.transform.rotation = Quaternion.Euler(0, mouseX, 0);
        // 카메라의 위치를 특정위치(followCameraPos)로 변환 (시점 고정)
        playerFollowCam.transform.position = followCameraPos.transform.position;
    }

    void PlayerAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("isAttack");
            attackDelay = false;
            Invoke("AttackDelay", 2f); // 재장전 시간 2초
            Debug.Log("발사");
            RayCamera();
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
}
