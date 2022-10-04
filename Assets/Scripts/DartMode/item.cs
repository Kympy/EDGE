using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 추가 
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class item : MonoBehaviourPun
{
    public enum Type { Axe, Knife };
    public Type type;
    public int value;
    public float itemSpeed = 0;
    
    

    private void Awake()
    {
        
    }

    private void Start()
    {
/*
        if (this.gameObject.tag == "Axe")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed * 1.5f + transform.up * itemSpeed * 0.8f);
            GetComponent<Rigidbody>().AddTorque(transform.right * itemSpeed * 100000f);
        }
        else if (this.gameObject.tag == "Knife")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed * 1.5f);
        }
*/
    }

    [PunRPC]
    // 추가
    void ThrowWeapon(float inputSpeed)
    {
        if (this.gameObject.tag == "Axe")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * inputSpeed * 1.5f + transform.up * inputSpeed * 0.8f);
            GetComponent<Rigidbody>().AddTorque(transform.right * inputSpeed * 100000f);

            Debug.Log($"{itemSpeed}");
        }
        else if (this.gameObject.tag == "Knife")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * inputSpeed * 1.5f);
        }
    }


    private void Update()
    {

        





    }
}



