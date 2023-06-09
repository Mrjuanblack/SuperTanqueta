using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float bodyRotationSpeed = 2.5f;
    public float turretRotationSpeed = 10f;

    private Transform mainTurretTransform;
    private Transform mainTurretTargetTransform;
    private Transform smallTurretTransform;
    private Transform smallTurretTargetTransform;

    public Transform smallTurretSpawnTransform;
    public Transform mainTurretSpawnTransform;

    public float mainTurretCooldown = 1f;
    private float mainTurretCurrentCooldown = 0f;
    public float smallTurretCooldown = 0.2f;
    private float smallTurretCurrentCooldown = 0f;

    public GameObject smallBulletRef;
    public GameObject bigBulletRef;

    private bool isMainTurretActive = true;
    private bool isShooting = false;

    private Vector2 moveInput;
    private Vector2 aimInput;

    private Transform tankBodyTrans;
    private Rigidbody tankBodyRigidbody;
    private Quaternion initialRotformTankBody;

    private AudioSource smallTurretAudio;
    private AudioSource mainTurretAudio;

    private Animator mainBodyAnim;
    private Animator mainTurretAnim;
    private Animator leftTrackAnim;
    private Animator rightTrackAnim;

    private void OnEnable()
    {
        // Enable the input actions
        Controls controls = new Controls();
        controls.Enable();

        // Subscribe to the input actions
        controls.gameplay.move.performed += OnMovePerformed;
        controls.gameplay.move.canceled += OnMoveCanceled;
        controls.gameplay.aim.performed += OnAimPerformed;
        controls.gameplay.aim.canceled += OnAimCanceled;
        controls.gameplay.swapWeapon.performed += OnSwapPerformed;
        controls.gameplay.shoot.performed += OnShootPerformed;
        controls.gameplay.shoot.canceled += OnShootCanceled;
    }

    private void OnDisable()
    {
        // Disable the input actions
        Controls controls = new Controls();
        controls.Disable();

        // Unsubscribe from the input actions
        controls.gameplay.move.performed -= OnMovePerformed;
        controls.gameplay.move.canceled -= OnMoveCanceled;
        controls.gameplay.aim.performed -= OnAimPerformed;
        controls.gameplay.aim.canceled -= OnAimCanceled;
        controls.gameplay.swapWeapon.performed -= OnSwapPerformed;
        controls.gameplay.shoot.performed -= OnShootPerformed;
        controls.gameplay.shoot.canceled -= OnShootCanceled;
    }

    private void Awake()
    {
        tankBodyTrans = gameObject.transform.Find("SuperTanqueta_MainBody");

        GameObject mainTurretGO = Instantiate(new GameObject("MainTurretTarger"), tankBodyTrans);
        GameObject smallTurretGO = Instantiate(new GameObject("SmallTurretTarger"), tankBodyTrans);

        mainTurretTransform = gameObject.transform.Find("MainTurretPivot");
        
        mainTurretGO.transform.position = mainTurretTransform.position;
        mainTurretGO.transform.rotation = mainTurretTransform.rotation;
        mainTurretTargetTransform = mainTurretGO.transform;

        smallTurretTransform = gameObject.transform.Find("SmallTurretPivot");
        smallTurretGO.transform.position = smallTurretTransform.position;
        smallTurretGO.transform.rotation = smallTurretTransform.rotation;
        smallTurretTargetTransform = smallTurretGO.transform;
        initialRotformTankBody = new Quaternion(tankBodyTrans.rotation.x, tankBodyTrans.rotation.y, tankBodyTrans.rotation.z, tankBodyTrans.rotation.w);


        tankBodyRigidbody = gameObject.transform.Find("SuperTanqueta_MainBody").gameObject.AddComponent<Rigidbody>();
        tankBodyRigidbody.useGravity = false;

        smallTurretAudio = smallTurretTransform.gameObject.GetComponent<AudioSource>();
        mainTurretAudio = mainTurretTransform.gameObject.GetComponent<AudioSource>();
        mainBodyAnim = tankBodyTrans.gameObject.GetComponent<Animator>();
        mainTurretAnim = mainTurretTransform.Find("SuperTanqueta_MainTurret").GetComponent<Animator>();
        leftTrackAnim = tankBodyTrans.Find("SuperTanqueta_TackLeft").gameObject.GetComponent<Animator>();
        rightTrackAnim = tankBodyTrans.Find("SuperTanqueta_TackRight").gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if(isShooting){
            if(isMainTurretActive){
                if (mainTurretCurrentCooldown >= mainTurretCooldown) {
                    Shoot(true);
                    mainTurretCurrentCooldown = 0;
                }
            } else {
                if (smallTurretCurrentCooldown >= smallTurretCooldown) {
                    Shoot(false);
                    smallTurretCurrentCooldown = 0;
                }
            }
        }
        ManageCooldowns(Time.deltaTime);
    }

    private void ManageCooldowns(float deltaTime) {
        if(mainTurretCurrentCooldown < mainTurretCooldown){
            mainTurretCurrentCooldown += deltaTime;
        }
        if(smallTurretCurrentCooldown < smallTurretCooldown){
            smallTurretCurrentCooldown += deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // Tank Movement
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        tankBodyRigidbody.velocity = movement;
        if (movement != Vector3.zero)
        {
            Quaternion bodyRotation = Quaternion.LookRotation(movement) * initialRotformTankBody;
            tankBodyRigidbody.rotation = Quaternion.Lerp(tankBodyRigidbody.rotation, bodyRotation, bodyRotationSpeed * Time.deltaTime);
            mainTurretTransform.position = mainTurretTargetTransform.position;
            smallTurretTransform.position = smallTurretTargetTransform.position;
            mainBodyAnim.SetFloat("Speed", moveInput.x);
            mainBodyAnim.SetFloat("Tilt", moveInput.y);
            mainTurretAnim.SetFloat("Speed", moveInput.x);
            mainTurretAnim.SetFloat("Tilt", moveInput.y);
            leftTrackAnim.speed = movement.magnitude;
            rightTrackAnim.speed = movement.magnitude;
        }else{
            leftTrackAnim.speed = 0;
            rightTrackAnim.speed = 0;
        }

        // Turret Aiming
        Vector3 aimDirection = new Vector3(-aimInput.y, 0f, aimInput.x);
        if (aimDirection != Vector3.zero)
        {
            Quaternion turretRotation = Quaternion.LookRotation(aimDirection);
            if (isMainTurretActive) {
                mainTurretTransform.rotation = Quaternion.Slerp(mainTurretTransform.rotation, turretRotation, turretRotationSpeed * Time.deltaTime);
            }
            else
            {
                smallTurretTransform.rotation = Quaternion.Slerp(smallTurretTransform.rotation, turretRotation, turretRotationSpeed * Time.deltaTime);
            }
            
        }
        if (isMainTurretActive) {
            smallTurretTransform.rotation = Quaternion.Slerp(smallTurretTransform.rotation, smallTurretTargetTransform.rotation, turretRotationSpeed * Time.deltaTime);
        } else {
            mainTurretTransform.rotation = Quaternion.Slerp(mainTurretTransform.rotation, mainTurretTargetTransform.rotation, turretRotationSpeed * Time.deltaTime);
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        aimInput = Vector2.zero;
    }

    private void OnSwapPerformed(InputAction.CallbackContext context)
    {
        SwapTurret();
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        isShooting = true;
    }
    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        isShooting = false;
    }

    private void SwapTurret()
    {
        isMainTurretActive = !isMainTurretActive;
    }

    private void Shoot(bool isMainTurret)
    {
        if(isMainTurret)
        {
            Instantiate(bigBulletRef, mainTurretSpawnTransform.position, mainTurretSpawnTransform.rotation);
            mainTurretAudio.pitch = Random.Range(0.90f, 1f);
            mainTurretAudio.Play();
            mainTurretAnim.SetTrigger("Shoot");
        } else {
            Instantiate(smallBulletRef, smallTurretSpawnTransform.position, smallTurretSpawnTransform.rotation);
            smallTurretAudio.pitch = Random.Range(0.95f, 1f);
            smallTurretAudio.Play();
        }
    }
}
