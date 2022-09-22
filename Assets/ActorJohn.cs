using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorJohn : MonoBehaviour
{
    [SerializeField] private Animator _Animator = null;
    [SerializeField] private Rigidbody _Rigidbody = null;
    [SerializeField] private GameObject _Blood = null;

    [SerializeField] private CutSceneManager _CutSceneManager = null;

    private void Awake()
    {
        _Blood.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ActorBullet")
        {
            collision.gameObject.GetComponent<Collider>().enabled = false;
            _Blood.SetActive(true);
            _Animator.enabled = false;
            _Rigidbody.AddForce(-transform.forward * 5f + transform.up * 2f, ForceMode.Impulse);
            Time.timeScale = 0.1f;
            StartCoroutine(_CutSceneManager.ResetScene());
        }
    }
}
