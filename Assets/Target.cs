using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Bullet"))
        {
            StartCoroutine(Back());

        }
    }
    public IEnumerator Back()
    {
        while(true)
        {
            transform.eulerAngles += new Vector3(0.5f, 0f, 0f);
            if(transform.eulerAngles.x > 90f)
            {
                StartCoroutine(Front());
                StopCoroutine("Back");
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator Front()
    {
        yield return new WaitForSecondsRealtime(3f);
        while (true)
        {
            transform.eulerAngles -= new Vector3(0.5f, 0f, 0f);
            if (transform.eulerAngles.x < 0f)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                StopCoroutine("Front");
                yield break;
            }
            yield return null;
        }
    }
}
