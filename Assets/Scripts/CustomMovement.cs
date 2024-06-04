using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2.0f; // Movement speed
    public float rotationSpeed = 45.0f; // Rotation speed in degrees per second

    public Transform cameraRigTransform;
    public Transform centerEyeAnchor;

    void Start()
    {
        // Find the OVRCameraRig and its CenterEyeAnchor
        /*cameraRigTransform = GetComponent<OVRCameraRig>().transform;
        centerEyeAnchor = cameraRigTransform.Find("TrackingSpace/CenterEyeAnchor");*/
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Get input from the left joystick (Oculus Touch)
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // Calculate movement direction relative to the CenterEyeAnchor's forward direction
        Vector3 moveDirection = centerEyeAnchor.forward * primaryAxis.y + centerEyeAnchor.right * primaryAxis.x;
        moveDirection.y = 0; // Keep movement horizontal

        // Move the camera rig
        cameraRigTransform.position += moveDirection * speed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        // Get input from the right joystick (Oculus Touch)
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // Apply rotation based on right joystick horizontal axis
        if (Mathf.Abs(secondaryAxis.x) > 0.1f)
        {
            float rotation = secondaryAxis.x * rotationSpeed * Time.deltaTime;
            cameraRigTransform.Rotate(0, rotation, 0);
        }
    }
}
