using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class item : MonoBehaviour
{
    public enum Type { Axe, Knife };
    public Type type;
    public int value;
    public float itemSpeed;
    

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (gameObject.tag == "Axe")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed*1.5f + transform.up * itemSpeed * 0.8f);
            GetComponent<Rigidbody>().AddTorque(transform.right * itemSpeed * 100000f);
        }

        if (gameObject.tag == "Knife")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed * 1.5f);
        }
    }

  

    private void Update()
    {

        





    }
}



