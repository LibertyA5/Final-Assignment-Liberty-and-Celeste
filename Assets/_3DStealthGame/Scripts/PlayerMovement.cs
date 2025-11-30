using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;

    [Header("Input Actions")]
    public InputAction MoveAction;
    public InputAction SprintAction;

    [Header("Movement Speeds")]
    public float walkSpeed = 1.0f;
    public float sprintSpeed = 1.75f; // adjust as needed
    public float turnSpeed = 20f;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        MoveAction.Enable();
        SprintAction.Enable();
    }

    void FixedUpdate()
    {
        // Read movement input
        Vector2 pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        m_Animator.SetBool("IsWalking", isWalking);

        // HOLD-TO-SPRINT logic
        bool isSprinting = SprintAction.ReadValue<float>() > 0.5f;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Rotation
        Vector3 desiredForward = Vector3.RotateTowards(
            transform.forward,
            m_Movement,
            turnSpeed * Time.deltaTime,
            0f
        );
        m_Rotation = Quaternion.LookRotation(desiredForward);

        // Apply rotation + movement
        m_Rigidbody.MoveRotation(m_Rotation);
        m_Rigidbody.MovePosition(
            m_Rigidbody.position +
            m_Movement * currentSpeed * Time.deltaTime
        );
    }

    private void OnDisable()
    {
        MoveAction.Disable();
        SprintAction.Disable();
    }
}
