using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera dollyCamera = null;
    [SerializeField] private CinemachineVirtualCamera dollyCamera2 = null;
    [SerializeField] private CinemachineDollyCart dollyCart = null;
    [SerializeField] private CinemachineDollyCart dollyCart2 = null;

    private Coroutine CutCoroutine = null;

    private int CameraIndex;

    private void Awake()
    {
        dollyCart.m_Speed = 2.5f;
        dollyCamera2.enabled = false;
        CutCoroutine = StartCoroutine(Cut1());
    }
    private IEnumerator Cut1()
    {
        while(true)
        {
            dollyCart.m_Speed += Time.deltaTime;
            if(dollyCart.m_Position >= dollyCart.m_Path.PathLength)
            {
                dollyCamera2.enabled = true;
                dollyCamera.enabled = false;
                dollyCart2.m_Speed = 0.5f;
                yield break;
            }
            yield return null;
        }      
    }
}
