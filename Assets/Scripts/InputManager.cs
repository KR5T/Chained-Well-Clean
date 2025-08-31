using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerController playerControls;
    AnimatorManager animatorManager;
    PlayerLocomotion playerLocomotion;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;
    public float cameraInputX;
    public float cameraInputY;
    public bool jumpInput;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerController();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
        }
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        //HandleJumpInput
        //HandleActionInput
    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveAmount *= 0.5f;
            verticalInput *= 0.5f;
            horizontalInput *= 0.5f;
            playerLocomotion.movementSpeed = 3;
        }
        else if (playerLocomotion.isOnStair)
        {
            playerLocomotion.movementSpeed = 5;
        }
        else
        {
            playerLocomotion.movementSpeed = 7;
        }

        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }
}