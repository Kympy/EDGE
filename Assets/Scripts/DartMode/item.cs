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
        if (this.gameObject.tag == "Axe")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed * 1.5f + transform.up * itemSpeed * 0.8f);
            GetComponent<Rigidbody>().AddTorque(transform.right * itemSpeed * 100000f);
        }
        else if (this.gameObject.tag == "Knife")
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed * 3f);
        }
    }

  

    private void Update()
    {

        





    }
}



