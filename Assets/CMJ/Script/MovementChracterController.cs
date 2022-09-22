using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class MovementChracterController : MonoBehaviourPun
{
    [SerializeField]
    private float moveSpeed;
    private Vector3 moveForce;

    private CharacterController characterController;


    private void Awake()
    {
        if (photonView.IsMine == false) return;
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false) return;
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }
}
