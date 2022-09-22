using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEvent8 : MonoBehaviour
{
    int score = 0;
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
        if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade")
        {
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            /*Debug.Log("¹ÚÇû´ç");*/

            if (collision.gameObject.GetComponentInChildren<BoxCollider>().gameObject.tag == "Blade")
            {
                score += 8;
            }
            Debug.Log(score);
        }

        else
            Debug.Log("ÃÄ³Â´ç");
    }
}
