using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float bodyRotationSpeed = 2.5f;
    public float turretRotationSpeed = 10f;

    public Transform turretTransform;

    private Rigidbody tankRigidbody;
    private Transform tankTransform;
    private Vector2 moveInput;
    private Vector2 aimInput;

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
    }

    private void Awake()
    {
        tankRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Tank Movement
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        tankRigidbody.velocity = movement;
        if (movement != Vector3.zero)
        {
            Quaternion bodyRotation = Quaternion.LookRotation(movement);
            tankRigidbody.rotation = Quaternion.Lerp(tankRigidbody.rotation, bodyRotation, bodyRotationSpeed * Time.deltaTime);
        }

        // Turret Aiming
        Vector3 aimDirection = new Vector3(-aimInput.y, 0f, aimInput.x);
        if (aimDirection != Vector3.zero)
        {
            Quaternion turretRotation = Quaternion.LookRotation(aimDirection);
            turretTransform.rotation = Quaternion.Slerp(turretTransform.rotation, turretRotation, turretRotationSpeed * Time.deltaTime);
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
}
