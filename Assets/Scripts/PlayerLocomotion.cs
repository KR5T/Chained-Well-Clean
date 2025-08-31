using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody rigby;

    [Header("Falling")]
    public bool isGrounded;
    public float fallingSpeed;
    public float leapingVelocity;
    public LayerMask groundLayer;
    public float inAirTimer;
    public float raycastHeightOffSet = 0.5f;
    public float sphereRadius = 0.6f;
    public float maxDistance = 1.2f;

    [Header("Movement")]
    public float movementSpeed = 7;
    public float rotationSpeed = 15;

    [Header("Stair")]
    public bool isOnStair = false;

    [Header("Jumping")]
    public float jumpForce = 5f;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        rigby = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        if (isOnStair)
        {
            HandleStairMovement();
        }

        HandleFallingAndLanding();

        if (playerManager.isInteracting && !isGrounded)
            return;
        HandleJumping();
        HandleMovement();
        HandleRotation();
        
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection = moveDirection * movementSpeed;

        Vector3 movementVelocity = moveDirection;

        // Y hızını sıfırlama → yer çekimi / stair kuvveti çalışsın
        rigby.velocity = new Vector3(movementVelocity.x, rigby.velocity.y, movementVelocity.z);
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + raycastHeightOffSet;

        if (Physics.SphereCast(rayCastOrigin, sphereRadius, Vector3.down, out hit, maxDistance, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            }
            inAirTimer = 0f;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;

            if (!playerManager.isInteracting && !inputManager.jumpInput)
                animatorManager.PlayTargetAnimation("Falling", true);

            inAirTimer += Time.deltaTime;
            rigby.AddForce(Vector3.down * fallingSpeed * inAirTimer, ForceMode.Acceleration);
        }
    }

    private void HandleStairMovement()
    {

        // stair'deyken karakteri zemine yapıştır
        rigby.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Stair"))
        {
            isOnStair = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Stair"))
        {
            isOnStair = false;
        }
    }

    private void HandleJumping()
    {
        if (isGrounded && inputManager.jumpInput)
        {
            animatorManager.PlayTargetAnimation("Jumping", true);

            Vector3 velocity = rigby.velocity;
            velocity.y = 0;
            rigby.velocity += Vector3.up * jumpForce;

            isGrounded = false;
        }
        inputManager.jumpInput = false;
    }    
}
