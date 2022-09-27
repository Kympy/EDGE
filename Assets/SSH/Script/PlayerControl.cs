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

    // �ڽ� ������Ʈ�� �ִ� rigidBody
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

    // Lobby Scene�� ��� ��� Ȱ��ȭ�� ���� ���� ���� �� �ʱ�ȭ
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
        // photonView�� IsMine �϶��� ���� 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerFollowCam = GameObject.Find("PlayerFollowCam");
        // Find : ���� 1ȸ ����� �������� Update�� ���� ���������� ����
        // followCameraPos = transform.GetChild(2).gameObject; 

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // �ڽ� ������Ʈ�� �پ��ִ� rigidBody
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
        // photonView�� IsMine �϶��� ���� 

        Invoke("Unlock", 3f); // ���� 3�ʵ� ���콺 ȸ�� Ȱ��ȭ

        Debug.Log("player     " + transform.position);

        // PlayerChest�� BoneTransform �ֱ�
        if (anim)
        {
            PlayerChest = anim.GetBoneTransform(HumanBodyBones.Chest);
        }

        // ragdoll�� isKinematic : true
        // Player Death ���� isKinematic false�� ��ȯ �ʿ�
        for (int i = 0; i < rbChild.Length ; i++ )
        {
            if (i >= 1)
            {
                rbChild[i].isKinematic = true;
            }
        }

        // ���� Scene�� Ȯ���Ͽ� Player ��� ��/����
        CurSceneFind();
    }

    void Update()
    {
        // Debug.Log("player     " + transform.position);
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView�� IsMine �϶��� ���� 

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

    // ���� ���� : Update -> animation -> LateUpdate
    // chest ȸ�� �� animation�� ���� �ʱ�ȭ�Ǵ� ��츦 �����ϱ� ���� LaUpdate ��� 
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

        // ȸ���� ����
        mouseY = Mathf.Clamp(mouseY, limitMinMouseY, limitMaxMouseY);

        // ���ѵ� mouseY�� �Է¹޾� Chest ȸ��
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
        // ���콺 �̵��� ���� ȸ��
        playerFollowCam.transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

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
            
            GunFire();
        }

        else if (Input.GetButtonDown("Fire1") && !playerLobbyActive && isAlive)
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
                Debug.Log("����");

                //rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBuffered);
                //rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBuffered);
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBufferedViaServer);

            }

            if (rayHit.transform.tag == "SaloonObject")
            {
                //rayHit.rigidbody.isKinematic = false;

                // SaloonObject �ǰ� �� playerFollowCam.transform.forward�� AddForce
                // rayHit.transform.gameObject.GetComponent<Rigidbody>().AddForce(playerFollowCam.transform.forward * 10f, ForceMode.Impulse);
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("FireHit", RpcTarget.AllBuffered, playerFollowCam.transform.forward * 10f);
            }
        }
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

    void CurSceneFind()
    {
        // Lobby Scene �÷��̾� �Ϻ� ��� Ȱ��ȭ 
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            Debug.Log("�κ� ����");
            LobbyPlayerActive();
            Unlock(); // �÷��̾� ȸ�� Ȱ��ȭ
            // gameSceneLogic.LobbyPos();
        }

        else if (SceneManager.GetActiveScene().name == "GunFight")
        {
            // Lobby������ Ȱ��ȭ�� isStart, attackDelay ��Ȱ��ȭ
            isStart = false;
            attackDelay = false;

            Invoke("Unlock", 3f); // 3�� �� �÷��̾� isStart, attackDelay
            
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

