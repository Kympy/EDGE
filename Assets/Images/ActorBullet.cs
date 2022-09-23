using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBullet : MonoBehaviour
{
    private Rigidbody _Rigidbody = null;
    [SerializeField] private GameObject _Blood = null;
    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _Blood.SetActive(false);
    }
    private void Start()
    {
        _Rigidbody.AddForce(transform.forward * 20f, ForceMode.Impulse);
        _Rigidbody.AddTorque(transform.forward * 5f, ForceMode.Impulse);
    }
    private void Update()
    {
        if (_Blood.activeSelf)
        {
            _Blood.transform.rotation = Quaternion.Euler(-180f, 0f, 0f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Target")
        {
            _Blood.SetActive(true);
            GetComponent<Collider>().enabled = false;
        }
    }
}
