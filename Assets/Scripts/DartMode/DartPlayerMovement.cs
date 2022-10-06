using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartPlayerMovement : MonoBehaviour
{
    // 리지드바디
    private Rigidbody _Rigidbody = null;

    // 이동 키 값을 저장할 변수
    private float horizontal = 0f;
    private float vertical = 0f;

    // 해당 방향으로 키 입력이 있는지
    private bool hasHorizontalInput = false;
    private bool hasVerticalInput = false;

    // 이동 중인지
    private bool isMove = false;

    public bool IsMove { get { return isMove; } }

    // 이동할 벡터
    private Vector3 moveVector = Vector3.zero;

    // 이동 속도
    [SerializeField, Range(0f, 100f)]private float moveSpeed = 0f;

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (isMove)
        {
            _Rigidbody.position += moveSpeed * Time.deltaTime * moveVector;
        }
    }

    private void Update()
    {
        // 키 입력을 받음 -1, 0, 1
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        isMove = hasHorizontalInput || hasVerticalInput;

        moveVector = (transform.forward * vertical + transform.right * horizontal).normalized;
    }
}
