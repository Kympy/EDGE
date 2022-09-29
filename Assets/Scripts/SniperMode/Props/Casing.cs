using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    private Rigidbody _Rigidbody = null;
    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _Rigidbody.AddForce((transform.right + transform.forward) * 1f, ForceMode.Impulse);
    }
}
