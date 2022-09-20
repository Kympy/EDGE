using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHeader : MonoBehaviourPun
{
    // Sensitivity
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float mouseSpeed = 10f;
    [SerializeField] protected float zoomMouseSpeed = 0.1f;
    [SerializeField] protected float zoomSpeed = 50f;

    protected Rigidbody _Rigidbody = null;
    // Player Virtual Arm
    protected GameObject Arm = null;

    // Player HP
    protected float HP;
    protected float MaxHP = 100;

    // Player Upper Bone
    protected Transform UpperBody = null;

    // Keyboard Movement
    protected float horizontal;
    protected float vertical;
    protected Vector3 moveVector;
    // Mouse Movement
    protected float mouseX = 0f;
    protected float mouseY = 0f;
    // Mouse Upper Rotation
    protected float mouseYUpper = 90f;
    // Fire
    protected float shootRot; // Fire time's X Rotation
    protected float recoilPower; // Recoil X Rotation Value

    // Fire Position
    protected Transform shootPos;
    protected Transform ZoomShootPosition;

    // Player Control Booleans
    protected bool IsZoom = false;

    // Keyboard Input Checker
    protected bool hasHorizontalInput;
    protected bool hasVerticalInput;
    protected bool IsMove = false;
    protected bool IsFire = false;

    // Camera
    protected Camera PlayerCamera; // Player Following Camera
    protected Camera ScopeCamera; // Sniper rifle scope
    protected float ClampedX = 0f; // Fixed Camera X rotation
    // Zoom Coroutine
    protected Coroutine ZoomCoroutine = null;
    // Recoil Coroutine
    protected Coroutine ReCoilCoroutine = null;
    // Zoom In / Out Arm Position
    protected Transform ZoomOutPos;
    protected Transform ZoomInPos;
    // Player Following Camera Position
    protected Transform PlayerCameraPos;
    // Casing Position
    protected Transform CasingPos;
    protected GameObject ArmCasing = null;
    // Animator
    protected Animator _ArmAnimator;
    protected Animator _PlayerAnimator;
    // Effect
    protected GameObject RealSmoke = null; // Player Arm smoke
    [SerializeField] protected GameObject FakeSmoke = null; // Player body smoke
    protected GameObject RealMuzzle = null; // Player Arm muzzle
    [SerializeField] protected GameObject FakeMuzzle = null; // Player body muzzle

    protected PlayerAudio _PlayerAudio = null;

    public float CurrentHP { get { return HP; } }
    public float Max_HP { get { return MaxHP; } }
}
