using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    private Camera PC;
    public float distance = 100f;
    RaycastHit rcHit;
    
    

    // Start is called before the first frame update
    void Start()
    {
        PC = Camera.main;
        Vector3 rayOrigin = PC.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 rayDir = PC.transform.forward;

        if (Input.GetMouseButton(0))
        {
            Ray ray = new Ray(rayOrigin, rayDir);

            

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 5f);

        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FireRay()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        

        Physics.Raycast(ray, out rcHit);
        /*Vector3 zzz = rcHit.point - Throwitem.transform.position; AudioReverbZone*/
    }
}
