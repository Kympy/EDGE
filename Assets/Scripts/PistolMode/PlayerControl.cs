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

    // �ڽ� ������Ʈ�� �ִ� rigidBody
    // = ragdoll 
    Rigidbody[] rbChild = new Rigidbody[13];

    BullCount bulletUI = null;

    // �¸� UI
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

    // Lobby Scene�� ��� ��� Ȱ��ȭ�� ���� ���� ���� �� �ʱ�ȭ
    bool playerLobbyActive = false;

    bool isAlive = true;

    // Particle ȣ��
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
        // photonView�� IsMine �϶��� ���� 

        // GunFight Scene : other Client Player error -> photonView Other isKinematic == false 
        photonView.RPC("PhyPlayer", RpcTarget.OthersBuffered);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerFollowCam = GameObject.Find("PlayerFollowCam");
        // Find : ���� 1ȸ ����� �������� Update�� ���� ���������� ����
        // followCameraPos = transform.GetChild(2).gameObject; 

        // �ڽ� ������Ʈ�� �پ��ִ� rigidBody
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

        // Player�� photonView�� IsMine�϶� PlayerChest�� BoneTransform �ֱ�
        if (anim && photonView.IsMine)
        {
            PlayerChest = anim.GetBoneTransform(HumanBodyBones.Chest);
        }
        /*
                // Player�� photonView�� IsMine==false (������ �������� ���� Player Clone)�϶� BoneTransform �ֱ�
                if (anim && photonView.IsMine == false)
                {
                    OtherPlayerChest = anim.GetBoneTransform(HumanBodyBones.Chest);
                }
        */
        if (photonView.IsMine == false)
        {
            return;
        }
        // photonView�� IsMine �϶��� ���� 


        Invoke("Unlock", 3f); // ���� 3�ʵ� ���콺 ȸ�� Ȱ��ȭ

        // Debug.Log("player     " + transform.position);



        // ragdoll�� isKinematic : true
        // Player Death ���� isKinematic false�� ��ȯ �ʿ�
        for (int i = 0; i < rbChild.Length; i++)
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

        // trans to PUNRpc : MouseY
        // InPutMouseY();
    }

    // ���� ���� : Update -> animation -> LateUpdate
    // chest ȸ�� �� animation�� ���� �ʱ�ȭ�Ǵ� ��츦 �����ϱ� ���� LaUpdate ��� 
    private void LateUpdate()
    {
        // Player PhotonView == IsMine
        if (isStart && isAlive && photonView.IsMine)
        {
            PlayerRotate();
        }

        // PRC target.ohter�� ���濡�� �������� PlayerClone�� Mouse Y�� ����
        // ������ ������ mouseY ���� �� Client�� ���� Player Clone�� �ݿ� 
        // else if (isStart && isAlive && photonView.IsMine == false)
        else if (photonView.IsMine == false)
        {
            // ������ ������ ������ mouseY ���� ������ �Լ� ȣ�� �ʿ�
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

        // ȸ���� ����
        mouseY = Mathf.Clamp(mouseY, limitMinMouseY, limitMaxMouseY);

        // Debug.Log(mouseY);

        // ���ѵ� mouseY�� �Է¹޾� Chest ȸ��
        PlayerChest.transform.localEulerAngles = new Vector3(0, 0, -mouseY);

        transform.rotation = Quaternion.Euler(0, mouseX, 0);

        photonView.RPC("PlayerRotateY", RpcTarget.OthersBuffered, inputMouseY);
    }

    void InPutMouseY()
    {
        if (punMouseY < -360) punMouseY += 360;
        if (punMouseY > 360) punMouseY -= 360;

        // ȸ���� ����
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
        // ���콺 �̵��� ���� ȸ��
        playerFollowCam.transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

        // ī�޶��� ��ġ�� Ư����ġ(followCameraPos)�� ��ȯ (���� ����)
        playerFollowCam.transform.position = followCameraPos.transform.position;
    }

    void PlayerAttack()
    {

        if (Input.GetButtonDown("Fire1") && playerLobbyActive)
        {
            photonView.RPC("FireAnim", RpcTarget.AllBuffered);

            attackDelay = false;
            Invoke("AttackDelay", 0.5f); // ������ �ð� 0.5��
            Debug.Log("[�κ�] �߻�");

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

            // Reload UI ȣ��
            StartCoroutine(gunFightSceneUI.Reload());

            Invoke("AttackDelay", 2f); // ������ �ð� 2��
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
                Debug.Log("����");

                // �ǰݹ��� �÷��̾� LOSE UI ȣ��
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("IsHit", RpcTarget.All);

                // Win UI ȣ��
                gunFightSceneUI.ResultWin();
                rayHit.transform.gameObject.GetComponent<PhotonView>().RPC("AnimControl", RpcTarget.AllBuffered);
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

        //Debug.Log("Chest ������ : " + inputMouseY);
    }

    [PunRPC]
    void PhyPlayer()
    {
        rb.isKinematic = true;
    }

}

