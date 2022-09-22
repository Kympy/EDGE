using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeCollisionEvent : MonoBehaviour
{
    private Rigidbody rb;
    private bool Hit;
    int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.tag == "Score1")
        {
            score += 1;
        }

        if (collision.gameObject.tag == "Score2")
        {
            score += 2;
        }

        if (collision.gameObject.tag == "Score3")
        {
            score += 3;
        }

        if (collision.gameObject.tag == "Score4")
        {
            score += 4;
        }

        if (collision.gameObject.tag == "Score5")
        {
            score += 5;
        }

        if (collision.gameObject.tag == "Score8")
        {
            score += 8;
        }

        if (collision.gameObject.tag == "Score9")
        {
            score += 9;
        }

        Debug.Log(score);



    }
}

