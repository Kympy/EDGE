using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController MyPlayerControll = null;

    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float rotateSpeed = 500f;
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float gravity = 20f;

    private bool isMove { get; set; }

    private void Awake()
    {
        MyPlayerControll = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Start()
    {
        CameraMove cam = FindObjectOfType<CameraMove>();
        cam.FollowTarget(this.transform);
    }
    // Update is called once per frame
    void Update()
    {
        //if (isMove == true)
        //{
        //    Move();
        //}
        Move();

    }
    void Move()
    {
        // 이동 입력
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");

        // 이동
        Vector3 moveDir = (xAxis * transform.right + zAxis * transform.forward) * moveSpeed * Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moveDir.y = jumpForce;
        }
        moveDir.y -=gravity*Time.deltaTime;
        MyPlayerControll.Move(moveDir);

        // 회전 
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        this.transform.Rotate(0f, mouseX * Time.deltaTime * rotateSpeed, 0f);
    }
}
