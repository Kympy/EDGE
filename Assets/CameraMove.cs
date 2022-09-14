using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // µû¶ó´Ù´Ò Transform;
    Transform Target = null;
    
    public void FollowTarget(Transform target)
    {
        Target = target;
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position = Target.position+Vector3.up;
        this.transform.rotation = Target.rotation;
    }
}
