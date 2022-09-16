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



    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerFollowCam = GameObject.Find("PlayerFollowCam");
        followCameraPos = GameObject.Find("CameraPos");
        // followCameraPos = transform.GetChild(2).gameObject; 
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Invoke("Unlock", 3f);
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
        playerFollowCam.transform.rotation = Quaternion.Euler(0, mouseX, 0);
        playerFollowCam.transform.position = followCameraPos.transform.position;
    }

    void PlayerAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("isAttack");
            attackDelay = false;
            Invoke("AttackDelay", 2f);
            Debug.Log("น฿ป็");
        }
    }

    void AttackDelay()
    {
        attackDelay = true;
    }
}
