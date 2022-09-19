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
        followCameraPos = GameObject.Find("CameraPos"); // Find : ���� 1ȸ ����� �������� Update�� ���� ���������� ����
        // followCameraPos = transform.GetChild(2).gameObject; 
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Invoke("Unlock", 3f); // ���� 3�ʵ� ���콺 ȸ�� Ȱ��ȭ
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
        // ���콺 �̵��� ���� ȸ��(Y��)
        playerFollowCam.transform.rotation = Quaternion.Euler(0, mouseX, 0);
        // ī�޶��� ��ġ�� Ư����ġ(followCameraPos)�� ��ȯ (���� ����)
        playerFollowCam.transform.position = followCameraPos.transform.position;
    }

    void PlayerAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("isAttack");
            attackDelay = false;
            Invoke("AttackDelay", 2f); // ������ �ð� 2��
            Debug.Log("�߻�");
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
                Debug.Log("����");
            }
        }


        /*
        if (rayHit.transform.tag == "Player")
        {
            Debug.Log("����");
        }
        */
    }
}
