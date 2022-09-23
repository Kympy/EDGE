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
    private bool Used = false; // Has a first collision
    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        BulletSound = GetComponents<AudioSource>();
    }
    private void Start()
    {
        _Rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse); // Give a force to bullet
        _Rigidbody.AddTorque(transform.forward * power, ForceMode.Impulse); // Give a rotation force
    }
    private void Update()
    {
        /*
        if(Used == false)
        {
            _Rigidbody.AddForce(Vector3.up * 5.8f, ForceMode.Acceleration);
        }
        */
        //Debug.Log(_Rigidbody.velocity.magnitude);
    }
    private void OnCollisionEnter(Collision collision) // When Bullet gets a collision with an object
    {
        if (photonView.IsMine == false) return;
        Used = true;
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
            FixBullet();
            // Else Instantiate blood because bullet hit player
            PhotonNetwork.Instantiate("Impacts/BodyImpact", collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

            if (collision.collider.CompareTag("PlayerHead"))
            {
                collision.transform.root.gameObject.GetPhotonView().RPC("GetDamage", RpcTarget.OthersBuffered, 100f);
                SniperGameManager.Instance.GetUI.UpdateIndicator("Head", 2);
                SniperGameManager.Instance.GetUI.gameObject.GetPhotonView().RPC("UpdateIndicator", RpcTarget.OthersBuffered, "Head", 1);
            }
            else if (collision.collider.CompareTag("PlayerBody"))
            {
                collision.transform.root.gameObject.GetPhotonView().RPC("GetDamage", RpcTarget.OthersBuffered, Mathf.Round(Random.Range(70f, 99f)));
                SniperGameManager.Instance.GetUI.UpdateIndicator("Body", 2);
                SniperGameManager.Instance.GetUI.gameObject.GetPhotonView().RPC("UpdateIndicator", RpcTarget.OthersBuffered, "Body", 1);
            }
            else if (collision.collider.CompareTag("PlayerArmL"))
            {
                collision.transform.root.gameObject.GetPhotonView().RPC("GetDamage", RpcTarget.OthersBuffered, Mathf.Round(Random.Range(10f, 50f)));
                SniperGameManager.Instance.GetUI.UpdateIndicator("ArmL", 2);
                SniperGameManager.Instance.GetUI.gameObject.GetPhotonView().RPC("UpdateIndicator", RpcTarget.OthersBuffered, "ArmL", 1);
            }
            else if (collision.collider.CompareTag("PlayerArmR"))
            {
                collision.transform.root.gameObject.GetPhotonView().RPC("GetDamage", RpcTarget.OthersBuffered, Mathf.Round(Random.Range(10f, 50f)));
                SniperGameManager.Instance.GetUI.UpdateIndicator("ArmR", 2);
                SniperGameManager.Instance.GetUI.gameObject.GetPhotonView().RPC("UpdateIndicator", RpcTarget.OthersBuffered, "ArmR", 1);
            }
            else if (collision.collider.CompareTag("PlayerLegL"))
            {
                collision.transform.root.gameObject.GetPhotonView().RPC("GetDamage", RpcTarget.OthersBuffered, Mathf.Round(Random.Range(20f, 60f)));
                SniperGameManager.Instance.GetUI.UpdateIndicator("LegL", 2);
                SniperGameManager.Instance.GetUI.gameObject.GetPhotonView().RPC("UpdateIndicator", RpcTarget.OthersBuffered, "LegL", 1);
            }
            else if (collision.collider.CompareTag("PlayerLegR"))
            {
                collision.transform.root.gameObject.GetPhotonView().RPC("GetDamage", RpcTarget.OthersBuffered, Mathf.Round(Random.Range(20f, 60f)));
                SniperGameManager.Instance.GetUI.UpdateIndicator("LegR", 2);
                SniperGameManager.Instance.GetUI.gameObject.GetPhotonView().RPC("UpdateIndicator", RpcTarget.OthersBuffered, "LegR", 1);
            }
            transform.SetParent(collision.transform.root, false);
        }
        
    }
    private void FixBullet() 
    {
        GetComponent<Collider>().enabled = false; // Disable Collider;
        _Rigidbody.velocity = Vector3.zero; // Stop
        _Rigidbody.isKinematic = true;
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
