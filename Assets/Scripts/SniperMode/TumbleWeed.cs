using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleWeed : MonoBehaviour
{
    private Rigidbody _Rigidbody;
    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();

    }
    private void Start()
    {
        _Rigidbody.AddForce(new Vector3(Random.Range(-1f, 0f), Random.Range(0f, 1f), Random.Range(-1f, 1f)), ForceMode.Impulse);
        _Rigidbody.AddTorque(Vector3.forward, ForceMode.Impulse);
        StartCoroutine(AutoMove());
    }
    private IEnumerator AutoMove()
    {
        float destroyTimer = 0f;
        WaitForSeconds timer = new WaitForSeconds(3f);
        while(true)
        {
            if(destroyTimer > 2)
            {
                TumbleWeedSpawner.Instance.ReturnTumble(this.gameObject);
                yield break;
            }
            destroyTimer += 1;
            _Rigidbody.AddTorque(Vector3.forward * 0.3f, ForceMode.Impulse);
            if((int)Random.Range(0, 5) == 0)
            {
                _Rigidbody.AddForce(Vector3.up * 0.2f, ForceMode.Impulse);
            }
            yield return timer;
        }
    }
}
