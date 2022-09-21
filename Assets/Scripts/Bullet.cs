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
    private void Update()
    {
        //Debug.Log(_Rigidbody.velocity.magnitude);
    }
    private void OnCollisionEnter(Collision collision) // When Bullet gets a collision with an object
    {
        if (photonView.IsMine == false) return;
        //PhotonNetwork.Instantiate("BulletHole", collision.contacts[0].point + collision.contacts[0].normal * 0.1f, Quaternion.LookRotation(collision.contacts[0].normal));
        if (collision.transform.CompareTag("Building")) // Hit building
        {
            PhotonNetwork.Instantiate("Impacts/SandImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            FixBullet();
            return;
        }
        else if (collision.transform.CompareTag("Dirt")) // Hit dirt
        {
            PhotonNetwork.Instantiate("Impacts/DirtImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            FixBullet();
            return;
        }
        else
        {
            Debug.Log(collision.collider.tag);
            FixBullet();
            // Else Instantiate blood because bullet hit player
            PhotonNetwork.Instantiate("Impacts/BodyImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

            if (collision.collider.CompareTag("PlayerHead"))
            {
                Debug.Log(collision.gameObject.transform.root.GetComponent<SniperControl>().ToString());
                collision.transform.root.gameObject.GetPhotonView().RPC("GetDamage", RpcTarget.AllBuffered, 100f);
                SniperGameManager.Instance.GetUI.UpdateIndicator("Head", 2);
                if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine == false)
                {

                }
            }
            else if (collision.collider.CompareTag("PlayerBody"))
            {
                if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine == false)
                {
                    Debug.Log("Body Indi");
                    SniperGameManager.Instance.GetUI.UpdateIndicator("Body", 2);
                }
            }
            else if (collision.collider.CompareTag("PlayerArmL"))
            {
                if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine == false)
                {
                    SniperGameManager.Instance.GetUI.UpdateIndicator("ArmL", 2);
                }        
            }
            else if (collision.collider.CompareTag("PlayerArmR"))
            {
                if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine == false)
                {
                    SniperGameManager.Instance.GetUI.UpdateIndicator("ArmR", 2);
                }
            }
            else if (collision.collider.CompareTag("PlayerLegL"))
            {
                if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine == false)
                {
                    SniperGameManager.Instance.GetUI.UpdateIndicator("LegL", 2);
                }
            }
            else if (collision.collider.CompareTag("PlayerLegR"))
            {
                if (collision.gameObject.GetComponentInParent<PhotonView>().IsMine == false)
                {
                    SniperGameManager.Instance.GetUI.UpdateIndicator("LegR", 2);
                }
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
