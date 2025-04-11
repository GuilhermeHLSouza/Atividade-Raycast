using UnityEngine;
using UnityEngine.Windows;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;
    float jumpForce = 7f;
    float gravity = -20f;

    public float playerHeight = 2f;
    public LayerMask WhatIsGround;
    bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 velocity;

    public Transform orientation;

    CharacterController characterController;
    Animator animator;

    public bool freeze; 

    private void Start()
    {
        characterController = GetComponentInChildren<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (freeze) return;

        GroundCheck();
        MyInput();
        HandleJump();
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = UnityEngine.Input.GetAxisRaw("Horizontal");
        verticalInput = UnityEngine.Input.GetAxisRaw("Vertical");
    }

    private void GroundCheck()
    {
        Vector3 rayOrigin = characterController.bounds.center;
        float rayLength = (characterController.height / 2f) + 0.1f;
        grounded = Physics.Raycast(rayOrigin, Vector3.down, rayLength, WhatIsGround);
    }

    private void HandleJump()
    {
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (grounded && UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 move = moveDirection.normalized * moveSpeed;

        characterController.Move((move + new Vector3(0, velocity.y, 0)) * Time.deltaTime);

        Vector3 lookDirection = orientation.forward;
        lookDirection.y = 0f;
        lookDirection.Normalize();

        if (lookDirection != Vector3.zero)
        {
            transform.forward = lookDirection;
        }
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        StopAllCoroutines();
        StartCoroutine(JumpToPositionRoutine(targetPosition, trajectoryHeight));
    }

    private IEnumerator JumpToPositionRoutine(Vector3 target, float height)
    {
        Vector3 startPos = transform.position;
        float g = -gravity;

        Vector3 displacementXZ = new Vector3(target.x - startPos.x, 0, target.z - startPos.z);
        float displacementY = target.y - startPos.y;

        float time = Mathf.Sqrt(2 * height / g) + Mathf.Sqrt(2 * Mathf.Max(0, displacementY - height) / g);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * g * height);
        Vector3 velocityXZ = displacementXZ / time;

        float elapsedTime = 0;

        freeze = true;

        while (elapsedTime < time)
        {
            characterController.Move((velocityXZ + velocityY) * Time.deltaTime);
            velocityY += Vector3.up * gravity * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        freeze = false;
        velocity = Vector3.zero;
    }
}
