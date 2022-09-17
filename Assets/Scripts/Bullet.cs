using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 940f;
    [SerializeField] private float power = 50f;
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
        _Rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse); // Give a force to bullet
        _Rigidbody.AddTorque(transform.forward * power, ForceMode.Impulse); // Give a rotation force
        //Debug.Log("Starting Y Position is " + transform.position.y + " m");
        //origin = transform.position.y;
    }
    private void OnCollisionEnter(Collision collision) // When Bullet gets a collision with an object
    {
        _Rigidbody.velocity = Vector3.zero; // Stop
        _Rigidbody.isKinematic = true;
        GetComponent<Collider>().enabled = false; // Disable Collider;

        if (collision.transform.CompareTag("Building")) // Hit building
        {
            PhotonNetwork.Instantiate("SandImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            return;
        }
        else if (collision.transform.CompareTag("Dirt")) // Hit dirt
        {
            PhotonNetwork.Instantiate("DirtImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            return;
        }
        // Else Instantiate blood because bullet hit player
        PhotonNetwork.Instantiate("BodyImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

        if (collision.transform.CompareTag("PlayerHead"))
        {
            
        }
        else if (collision.transform.CompareTag("PlayerBody"))
        {
            
        }
        else if (collision.transform.CompareTag("PlayerArmL"))
        {
            
        }
        else if (collision.transform.CompareTag("PlayerArmR"))
        {
            
        }
        else if (collision.transform.CompareTag("PlayerLegL"))
        {

        }
        else if (collision.transform.CompareTag("PlayerLegR"))
        {

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
