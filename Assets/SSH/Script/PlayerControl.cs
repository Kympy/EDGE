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

    // Lobby Scene�� ��� ��� Ȱ��ȭ�� ���� ���� ���� �� �ʱ�ȭ
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
        // photonView�� IsMine �϶��� ���� 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerFollowCam = GameObject.Find("PlayerFollowCam");
        // Find : ���� 1ȸ ����� �������� Update�� ���� ���������� ����
        // followCameraPos = transform.GetChild(2).gameObject; 
    }

    void Start()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView�� IsMine �϶��� ���� 

        Invoke("Unlock", 3f); // ���� 3�ʵ� ���콺 ȸ�� Ȱ��ȭ
    }

    void Update()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView�� IsMine �϶��� ���� 

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
            Debug.Log("�÷��̾� �̵� �غ��");
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
        // ���콺 �̵��� ���� ȸ��(Y��)
        playerFollowCam.transform.rotation = Quaternion.Euler(0, mouseX, 0);
        // ī�޶��� ��ġ�� Ư����ġ(followCameraPos)�� ��ȯ (���� ����)
        playerFollowCam.transform.position = followCameraPos.transform.position;
    }

    void PlayerAttack()
    {
        if (Input.GetButtonDown("Fire1") && playerLobbyActive)
        {
            anim.SetTrigger("isAttack");
            attackDelay = false;
            Invoke("AttackDelay", 0.5f); // ������ �ð� 0.5��
            Debug.Log("[�κ�] �߻�");
            RayCamera();
        }

        else if (Input.GetButtonDown("Fire1")&& !playerLobbyActive)
        {
            if (curBullet == maxBullet)
            {
                Debug.Log("���� �پ�");
                attackAble = false;
            }

            anim.SetTrigger("isAttack");
            attackDelay = false;

            Invoke("AttackDelay", 2f); // ������ �ð� 2��
            Debug.Log("�߻�");
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

    public void LobbyPlayerActive()
    {
        // �κ� ������ ��ȯ���� �� �÷��̾� 
        playerLobbyActive = true;
    }

    public void GunFightPlayerActive()
    {
        // �κ� ������ ��ȯ���� �� �÷��̾� 
        playerLobbyActive = false;
    }
}
