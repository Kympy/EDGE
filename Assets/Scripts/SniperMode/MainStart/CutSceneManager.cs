using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    #region Variables
    // Light
    [SerializeField] private GameObject Thunder = null;
    // Actor : Characters and effects on cut scene
    [SerializeField] private GameObject Player = null;
    [SerializeField] private GameObject Actor2 = null;
    [SerializeField] private Transform Actor2Origin = null;
    [SerializeField] private GameObject Smoke = null;
    [SerializeField] private GameObject Muzzle = null;
    // Props : Things on cut scene
    [SerializeField] private GameObject Bullet = null;
    [SerializeField] private GameObject FirePos = null;
    [SerializeField] private Image LightOutPanel = null;
    // Camera Order : Cut order
    [SerializeField] private CinemachineVirtualCamera dollyCamera = null;
    [SerializeField] private CinemachineVirtualCamera dollyCamera2 = null;
    [SerializeField] private CinemachineVirtualCamera dollyCamera3 = null;
    // Original Position of dolly camera 3
    [SerializeField] private Transform dollyCamera3Pos = null;
    // Cart Order : Dolly cart movement order
    [SerializeField] private CinemachineDollyCart dollyCart = null;
    [SerializeField] private CinemachineDollyCart dollyCart2 = null;

    private Coroutine CutCoroutine = null;
    private GameObject actorBullet = null;
    #endregion

    private void Awake() // When start game
    {
        Actor2Origin = Actor2.transform; // An actor who will die 's original position
        dollyCart.m_Speed = 2.5f; // First dolly speed

        dollyCamera2.enabled = false; // Wait for order
        dollyCamera3.enabled = false; // Wait for order

        Smoke.SetActive(false); // Hide effect
        Muzzle.SetActive(false); // Hide effect

        CutCoroutine = StartCoroutine(Cut1()); // Start first cut scenen coroutine
        StartCoroutine(ThunderCo());
    }
    private IEnumerator Cut1() // Move close to the building and rotate
    {
        dollyCamera.enabled = true; // First dolly camera enable and start taking
        dollyCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0f; // Path initialize
        bool once = false; // To restrict coroutine call execute only one time
        while(true)
        {
            dollyCart.m_Speed += Time.deltaTime; // Increase dolly cart 1's speed gradually
            if(dollyCart.m_Position >= dollyCart.m_Path.PathLength * 0.85f && once == false) // When dolly cart 1 almost arrived at destination,
            {
                once = true; // Toggle on boolean
                StartCoroutine(LightOut()); // Start screen light out
            }
            if(dollyCart.m_Position >= dollyCart.m_Path.PathLength) // When dolly cart 1 arrived at destination,
            {
                CutCoroutine = StartCoroutine(Cut2()); // Start play cut scene 2
                yield break; // End coroutine
            }
            yield return null;
        }      
    }
    private IEnumerator Cut2() // Left to right show
    {
        dollyCamera.enabled = false;
        dollyCamera2.enabled = true;
        dollyCamera2.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0f;

        dollyCart2.m_Speed = 0.1f;
        bool once = false;
        while(true)
        {
            dollyCart2.m_Speed += Time.deltaTime * 0.1f;

            if (dollyCart2.m_Position >= dollyCart2.m_Path.PathLength * 0.7f && once == false)
            {
                once = true;
                StartCoroutine(LightOut());
            }

            if (dollyCart2.m_Position >= dollyCart2.m_Path.PathLength)
            {
                CutCoroutine = StartCoroutine(Cut3());
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator Cut3()
    {
        dollyCamera2.enabled = false;
        dollyCamera3.enabled = true;
        dollyCamera3.transform.position = dollyCamera3Pos.position;

        yield return new WaitForSecondsRealtime(2f);
        Smoke.SetActive(true);
        Muzzle.SetActive(true);
        Player.GetComponent<Animator>().SetTrigger("Fire");
        GameObject target = Instantiate(Bullet, FirePos.transform.position, FirePos.transform.rotation);
        actorBullet = target;
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.001f;


        dollyCamera3.Follow = target.transform;

        //yield return new WaitForSecondsRealtime(1f);
        while(true)
        {
            Time.timeScale += Time.deltaTime;
            if(Time.timeScale > 1f)
            {
                Time.timeScale = 1f;
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator ResetScene()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        StartCoroutine(LightOut());
        yield return new WaitForSecondsRealtime(1f);
        Actor2.GetComponentInChildren<ActorJohn>().KinematicInit();
        Actor2.GetComponent<Animator>().enabled = true;
        Actor2.transform.position = Actor2Origin.position;
        Actor2.transform.rotation = Actor2Origin.rotation;
        //Actor2.GetComponent<Rigidbody>().velocity = Vector3.zero;

        dollyCamera3.Follow = null;

        dollyCart2.m_Position = 0f;
        dollyCart2.m_Speed = 0f;
        dollyCart.m_Position = 0f;
        dollyCart.m_Speed = 2.5f;

        Destroy(actorBullet);
        CutCoroutine = StartCoroutine(Cut1());
    }
    private IEnumerator LightOut()
    {
        while(true)
        {
            LightOutPanel.color += new Color(0f, 0f, 0f, Time.deltaTime); // Go Black
            if(LightOutPanel.color.a >= 1f)
            {
                LightOutPanel.color = new Color(0f, 0f, 0f, 1f);
                StartCoroutine(LightIn());
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator LightIn()
    {
        while (true)
        {
            LightOutPanel.color -= new Color(0f, 0f, 0f, Time.deltaTime); // Go Black
            if (LightOutPanel.color.a <= 0f)
            {
                LightOutPanel.color = new Color(0f, 0f, 0f, 0f);
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator ThunderCo()
    {
        WaitForSecondsRealtime sec = new WaitForSecondsRealtime(4f);
        WaitForSecondsRealtime toggle = new WaitForSecondsRealtime(0.1f);
        while(true)
        {
            yield return sec;
            sec = new WaitForSecondsRealtime(Random.Range(3f, 7f));
            Thunder.SetActive(true);
            yield return toggle;
            Thunder.SetActive(false);
            yield return toggle;
            Thunder.SetActive(true);
            yield return toggle;
            Thunder.SetActive(false);
        }
    }
}
