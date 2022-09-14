using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    private Rigidbody _Rigidbody;
    private float timer = 0f;
    private float origin = 0f;
    private bool CheckOnce = true;
    public GameObject hit;

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Destroy(this.gameObject, 60f);
        _Rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
        Debug.Log("Starting Y Position is " + transform.position.y + " m");
        origin = transform.position.y;
    }

    private void Update()
    {
        Debug.LogWarning("Height : " + transform.position.y + " m");
        if(CheckOnce)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                Debug.LogError("After 1 Second, Current Height is " + transform.position.y + " m" + " Distance Between : " + Mathf.Abs(transform.position.y - origin));
                timer = 0f;
                CheckOnce = false;
            }
            
        }

        Debug.Log(_Rigidbody.velocity.magnitude + " m/s");
        
        //transform.Translate(speed * Time.deltaTime * Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogError("Hit " + collision.transform.name);
        if (collision.transform.CompareTag("Target"))
        {

            Instantiate(hit, collision.transform.position, collision.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
