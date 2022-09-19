using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 940f;
    [SerializeField] private float power = 50f;
    private Rigidbody _Rigidbody;

    private AudioSource[] BulletSound = null;
    //private float timer = 0f;
    //private float origin = 0f;
    //private bool CheckOnce = true;
    //public GameObject hit;

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        BulletSound = GetComponents<AudioSource>();
    }
    private void Start()
    {
        _Rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse); // Give a force to bullet
        _Rigidbody.AddTorque(transform.forward * power, ForceMode.Impulse); // Give a rotation force
        //Debug.Log("Starting Y Position is " + transform.position.y + " m");
        //origin = transform.position.y;
    }
    private void OnCollisionEnter(Collision collision) // When Bullet gets a collision with an object
    {
        //PhotonNetwork.Instantiate("BulletHole", collision.contacts[0].point + collision.contacts[0].normal * 0.1f, Quaternion.LookRotation(collision.contacts[0].normal));
        if (collision.transform.CompareTag("Building")) // Hit building
        {
            PhotonNetwork.Instantiate("SandImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            FixBullet();
            return;
        }
        else if (collision.transform.CompareTag("Dirt")) // Hit dirt
        {
            PhotonNetwork.Instantiate("DirtImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            FixBullet();
            return;
        }
        else
        {
            FixBullet();
            // Else Instantiate blood because bullet hit player
            PhotonNetwork.Instantiate("BodyImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

            if (collision.transform.CompareTag("PlayerHead"))
            {
                Debug.Log("Head");
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, 100);
            }
            else if (collision.transform.CompareTag("PlayerBody"))
            {
                Debug.Log("Body");
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, 100);
            }
            else if (collision.transform.CompareTag("PlayerArmL"))
            {
                Debug.Log("Arm");
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, 100);
            }
            else if (collision.transform.CompareTag("PlayerArmR"))
            {
                Debug.Log("Arm");
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, 100);
            }
            else if (collision.transform.CompareTag("PlayerLegL"))
            {
                Debug.Log("Leg");
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, 100);
            }
            else if (collision.transform.CompareTag("PlayerLegR"))
            {
                Debug.Log("Leg");
                collision.gameObject.GetComponentInParent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, 100);
            }
        }
        
    }
    private void FixBullet()
    {
        _Rigidbody.velocity = Vector3.zero; // Stop
        _Rigidbody.isKinematic = true;
        GetComponent<Collider>().enabled = false; // Disable Collider;
        BulletSound[1].Play();
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
