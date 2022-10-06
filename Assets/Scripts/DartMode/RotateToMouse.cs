using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RotateToMouse : MonoBehaviourPun
{
    [SerializeField] private PlayerController playerController;
    
    [SerializeField]
    private float rotCamXAxisSpeed = 5;

    [SerializeField]
    private float rotCamYAxisSpeed = 20f;

    private float limitMinX = -80;
    private float limitMaxX = 50;
    private float eulerAngleX;
    private float eulerAngleY;

    private float CameraEulerAngleX = 0f;

    private float ServerEulerAngleX = 0f;

    private float mouseX = 0f;
    private float mouseY = 0f;

    [SerializeField] private Transform MyUpper = null;

    GameObject myCamera;

    private void Start()
    {
        myCamera = playerController.cam.gameObject;
    }

    private void Update()
    {
        if (photonView.IsMine == false) return;
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        eulerAngleX = mouseY * rotCamYAxisSpeed * Time.deltaTime;
        eulerAngleY = mouseX * rotCamXAxisSpeed * Time.deltaTime;

        CameraEulerAngleX = -eulerAngleX + myCamera.transform.eulerAngles.x;

        CameraEulerAngleX = CameraEulerAngleX > 180f ? CameraEulerAngleX - 360f : CameraEulerAngleX;
        CameraEulerAngleX = Mathf.Clamp(CameraEulerAngleX, -43f, 130f);

        myCamera.transform.eulerAngles = new Vector3(CameraEulerAngleX, myCamera.transform.eulerAngles.y, 0f);
        myCamera.transform.eulerAngles += new Vector3(0f, eulerAngleY, 0f);
        transform.eulerAngles += new Vector3(0f, eulerAngleY, 0f);

        photonView.RPC("UpdateServerBone", RpcTarget.All, -eulerAngleX);
    }

    private void LateUpdate()
    {
        if (photonView.IsMine == false)
        {
            ServerEulerAngleX = ServerEulerAngleX > 180f ? ServerEulerAngleX - 360f : ServerEulerAngleX;
            ServerEulerAngleX = Mathf.Clamp(ServerEulerAngleX, -43f, 130f);
            MyUpper.eulerAngles = new Vector3(MyUpper.eulerAngles.x, MyUpper.eulerAngles.y, MyUpper.eulerAngles.z + ServerEulerAngleX);
        }
    }

    [PunRPC]
    public void UpdateServerBone(float rotation)
    {
        ServerEulerAngleX += rotation;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
