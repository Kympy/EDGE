using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 940f;
    private Rigidbody _Rigidbody;
    //private float timer = 0f;
    //private float origin = 0f;
    //private bool CheckOnce = true;
    //public GameObject hit;

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        _Rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
        //Debug.Log("Starting Y Position is " + transform.position.y + " m");
        //origin = transform.position.y;
    }
    private void OnCollisionEnter(Collision collision)
    {
        _Rigidbody.velocity = Vector3.zero;
        _Rigidbody.isKinematic = true;

        if (collision.transform.CompareTag("Player"))
        {
            PhotonNetwork.Instantiate("BodyImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
        else if(collision.transform.CompareTag("Building"))
        {
            PhotonNetwork.Instantiate("SandImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
        else if(collision.transform.CompareTag("Dirt"))
        {
            PhotonNetwork.Instantiate("DirtImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
    }

#if UNITY_EDITOR
    /*
    private void CheckHeight()
    {
        Debug.LogWarning("Height : " + transform.position.y + " m");
        if (CheckOnce)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                Debug.LogError("After 1 Second, Current Height is " + transform.position.y + " m" + " Distance Between : " + Mathf.Abs(transform.position.y - origin));
                timer = 0f;
                CheckOnce = false;
            }
        }

        Debug.Log(_Rigidbody.velocity.magnitude + " m/s");
    }
    */
#endif
}
