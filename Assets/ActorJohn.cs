using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorJohn : MonoBehaviour
{
    [SerializeField] private Animator _Animator = null;
    [SerializeField] private Rigidbody _Rigidbody = null;
    [SerializeField] private GameObject _Blood = null;
    [SerializeField] private Rigidbody _Spine = null;
    [SerializeField] private Rigidbody _Hip = null;

    [SerializeField] private CutSceneManager _CutSceneManager = null;

    private void Awake()
    {
        _Blood.SetActive(false);
        KinematicInit();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ActorBullet")
        {
            Time.timeScale = 0.1f;
            _Animator.enabled = false;
            _Spine.isKinematic = false;
            _Hip.isKinematic = false;
            _Blood.SetActive(true);
            //_Rigidbody.AddForce(collision.gameObject.transform.forward * Random.Range(50f, 100f), ForceMode.Impulse);
            StartCoroutine(_CutSceneManager.ResetScene());
        }
    }
    public void KinematicInit()
    {
        _Spine.isKinematic = true;
        _Hip.isKinematic = true;
    }
}
