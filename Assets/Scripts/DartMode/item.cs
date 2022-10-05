using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ãß°¡ 
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class item : MonoBehaviourPun
{
    private Rigidbody rb;
    private BoxCollider box;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponentInChildren<BoxCollider>();
    }
    [PunRPC]
    public void ObjReset()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        box.enabled = false;
    }
}



