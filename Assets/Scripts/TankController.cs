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
    private Quaternion mainTurretDefRot;
    private Transform smallTurretTransform;
    private Quaternion smallTurretDefRot;

    // public Transform smallTurretSpawnTransform;
    // public Transform mainTurretSpawnTransform;

    private float mainTurretCooldown = 1f;
    private float mainTurretCurrentCooldown = 0f;
    public float smallTurretCooldown = 0.2f;
    private float smallTurretCurrentCooldown = 0f;

    // public GameObject bulletRef;

    private bool isMainTurretActive = true;
    private bool isShooting = false;

    private Vector2 moveInput;
    private Vector2 aimInput;

    private Transform tankBodyRigTrans;
    private Rigidbody tankBodyRigidbody;
    private Quaternion initialRotformTankBody;

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
        tankBodyRigTrans = gameObject.transform.Find("SuperTanqueta_MainBody").transform.Find("TankBodyRig");
        mainTurretTransform = gameObject.transform.Find("SuperTanqueta_MainTurret").transform.Find("MainTurretRig").Find("Base");
        smallTurretTransform = gameObject.transform.Find("SuperTanqueta_SmallTurret").transform.Find("SmallTurretRig").Find("Base");
        initialRotformTankBody = tankBodyRigTrans.rotation;


        tankBodyRigidbody = gameObject.transform.Find("SuperTanqueta_MainBody").transform.Find("TankBodyRig").gameObject.AddComponent<Rigidbody>();
        tankBodyRigidbody.useGravity = false;
        mainTurretDefRot = mainTurretTransform.localRotation;
        smallTurretDefRot = smallTurretTransform.localRotation;
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
            Debug.Log("a");
            Debug.Log(tankBodyRigidbody);
            Quaternion bodyRotation = Quaternion.LookRotation(movement) * initialRotformTankBody;
            tankBodyRigidbody.rotation = Quaternion.Lerp(tankBodyRigidbody.rotation, bodyRotation, bodyRotationSpeed * Time.deltaTime);
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
            smallTurretTransform.localRotation = Quaternion.Slerp(smallTurretTransform.localRotation, smallTurretDefRot, turretRotationSpeed * Time.deltaTime);
        } else {
            mainTurretTransform.localRotation = Quaternion.Slerp(mainTurretTransform.localRotation, mainTurretDefRot, turretRotationSpeed * Time.deltaTime);
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
        // if(isMainTurretActive){
        //     smallTurretTransform.localRotation = smallTurretDefRot;
        // } else {
        //     mainTurretTransform.localRotation = mainTurretDefRot;
        // }
    }

    private void Shoot(bool isMainTurret)
    {
        // if(isMainTurret)
        // {
        //     Debug.Log("asdasdasd");
        //     Projectile bulletScript = bulletRef.GetComponent<Projectile>();
        //     bulletScript.maxPiercingCount = 0;
        //     bulletScript.explodeAtEnd = true;
        //     bulletScript.explosionRadius = 5;
        //     Instantiate(bulletRef, mainTurretSpawnTransform.position, mainTurretSpawnTransform.rotation);
        // }
    }
}
