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
        GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed*1.5f + transform.up * itemSpeed*0.8f);
        /*GetComponent<Rigidbody>().AddTorque(transform.right * itemSpeed*100000f);*/
    }

  

    private void Update()
    {

        





    }
 



    /*GetComponent<Rigidbody>().AddForce(transform.forward * itemSpeed + transform.up * 300f);
    GetComponent<Rigidbody>().AddTorque(transform.right * 7f);*/

}



