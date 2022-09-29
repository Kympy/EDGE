using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class ObjectRayHit : MonoBehaviourPun
{
    Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [PunRPC]
    void FireHit(Vector3 vec3)
    {
        rb.AddForce(vec3, ForceMode.Impulse);
    }
}
