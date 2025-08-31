using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform targetTransform;
    public Transform cameraPivot; // The object the camera uses to pivot (Look up and down)
    public Transform cameraTransform; // The transform of the actual camera object in the scene
    public LayerMask collisonLayers; // the layers we want our camera to collide with
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;
    InputManager inputManager;

    public float followSpeed = 0.2f;

    public float cameraCollisionOffSet = 0.2f;
    public float minCollisionOffSet = 0.2f;
    public float cameraCollisionRadius;
    public float lookAngle; //Camera look up and down
    public float pivotAngle; //Camera look left or right
    public float cameraSpeed = 2f;

    public void Awake()
    {
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        inputManager = FindObjectOfType<InputManager>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, followSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Quaternion targetRotation;
        Vector3 rotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraSpeed);

        pivotAngle = Mathf.Clamp(pivotAngle, -35f, 35f);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisonLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minCollisionOffSet)
        {
            targetPosition =- minCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
