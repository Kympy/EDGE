using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartPlayerMovement : MonoBehaviour
{
    // ������ٵ�
    private Rigidbody _Rigidbody = null;

    // �̵� Ű ���� ������ ����
    private float horizontal = 0f;
    private float vertical = 0f;

    // �ش� �������� Ű �Է��� �ִ���
    private bool hasHorizontalInput = false;
    private bool hasVerticalInput = false;

    // �̵� ������
    private bool isMove = false;

    public bool IsMove { get { return isMove; } }

    // �̵��� ����
    private Vector3 moveVector = Vector3.zero;

    // �̵� �ӵ�
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
        // Ű �Է��� ���� -1, 0, 1
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        isMove = hasHorizontalInput || hasVerticalInput;

        moveVector = (transform.forward * vertical + transform.right * horizontal).normalized;
    }
}
