using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamCompo : MonoBehaviour
{
    public GameObject RealMuzzle;
    public GameObject RealSmoke;
    public GameObject ArmCasing;
    public Camera ScopeCamera;

    private void Awake()
    {
        RealMuzzle.SetActive(false);
        RealSmoke.SetActive(false);
        ArmCasing.SetActive(false);
    }
}
