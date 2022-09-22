using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutSceneManager : MonoBehaviour
{
    // Actor
    [SerializeField] private GameObject Player = null;
    [SerializeField] private GameObject Smoke = null;
    [SerializeField] private GameObject Muzzle = null;
    // Props
    [SerializeField] private GameObject Bullet = null;
    [SerializeField] private Transform FirePos = null;
    // Camera Order
    [SerializeField] private CinemachineVirtualCamera dollyCamera = null;
    [SerializeField] private CinemachineVirtualCamera dollyCamera2 = null;
    [SerializeField] private CinemachineVirtualCamera dollyCamera3 = null;
    // Cart Order
    [SerializeField] private CinemachineDollyCart dollyCart = null;
    [SerializeField] private CinemachineDollyCart dollyCart2 = null;

    private Coroutine CutCoroutine = null;

    private int CameraIndex;

    private void Awake()
    {
        dollyCart.m_Speed = 2.5f;

        dollyCamera2.enabled = false;
        dollyCamera3.enabled = false;

        Smoke.SetActive(false);
        Muzzle.SetActive(false);

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
                dollyCart2.m_Speed = 0.1f;
                CutCoroutine = StartCoroutine(Cut2());
                yield break;
            }
            yield return null;
        }      
    }
    private IEnumerator Cut2()
    {
        while(true)
        {
            dollyCart2.m_Speed += Time.deltaTime * 0.1f;
            if(dollyCart2.m_Position >= dollyCart2.m_Path.PathLength)
            {
                dollyCamera3.enabled = true;
                dollyCamera2.enabled = false;
                CutCoroutine = StartCoroutine(Cut3());
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator Cut3()
    {
        yield return new WaitForSecondsRealtime(2f);
        Player.GetComponent<Animator>().SetTrigger("Fire");
        Smoke.SetActive(true);
        Muzzle.SetActive(true);

        GameObject target = Instantiate(Bullet, FirePos.position, FirePos.rotation);
        dollyCamera3.Follow = target.transform;
        dollyCamera3.LookAt = target.transform;
        Time.timeScale = 0.1f;
        while (true)
        {

            yield return null;
        }
    }
}
