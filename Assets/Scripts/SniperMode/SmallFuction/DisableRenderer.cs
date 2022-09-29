using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRenderer : MonoBehaviour
{
    private void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("MyServerPlayer");
    }
}
