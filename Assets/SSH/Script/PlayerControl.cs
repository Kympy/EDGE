using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    GameObject playerFollowCam = null;
    GameObject followCameraPos = null;
    Animator anim = null;

    float mouseX = 0f;
    float rotateSpeed = 200f;

    bool isStar = false;

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
        Invoke("UnlockTrans", 3f);
    }

    void Update()
    {
        FollowCameraTrans();
        if (isStar)
        {
            PlayerRotate();
        }

        PlayerAttack();
    }

    void UnlockTrans()
    {
        isStar = true;
    }

    void PlayerRotate()
    {
        mouseX += Input.GetAxis("Mouse X")*Time.deltaTime*rotateSpeed;
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
        }
    }
}
