using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleWeed : MonoBehaviour
{
    private Rigidbody _Rigidbody;
    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(AutoMove());
    }
    private IEnumerator AutoMove()
    {
        WaitForSeconds timer = new WaitForSeconds(3f);
        while(true)
        {
            //_Rigidbody.AddForce((Vector3.right * 1f + Vector3.up * 1f).normalized, ForceMode.Impulse);
            _Rigidbody.AddTorque(Vector3.forward * 0.2f, ForceMode.Impulse);
            if((int)Random.Range(0, 5) == 0)
            {
                _Rigidbody.AddForce(Vector3.up * 0.2f, ForceMode.Impulse);
            }
            yield return timer;
        }
    }
}
