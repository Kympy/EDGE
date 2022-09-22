using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade")
        {
            Debug.Log(collision.gameObject.name);
            
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log("¹ÚÇû´ç");
        }

        else
            Debug.Log("ÃÄ³Â´ç");
    }
}
