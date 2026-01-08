using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 15f;

    [Header("Platform Calibration")]
    [Tooltip("Offset for PC (Mouse)")]
    public float pcRotationOffset = -90f; 

    [Tooltip("Offset for Mobile (Gyro). Try 0, 90, 180, or -90")]
    public float mobileRotationOffset = -180f; 

    // Internal trackers
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        // 1. Initialize PC rotation using the PC SPECIFIC offset
        yRotation = pcRotationOffset;

        #if UNITY_STANDALONE || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        #endif

        // 2. Enable Sensors for Mobile
        if (AttitudeSensor.current != null)
            InputSystem.EnableDevice(AttitudeSensor.current);
    }

    void Update()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
            // --- PC MOUSE LOGIC ---
            if (Mouse.current == null) return;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            
            float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
            float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

            yRotation += mouseX;
            xRotation -= mouseY; 

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        #elif UNITY_IOS || UNITY_ANDROID
            // --- MOBILE GYRO LOGIC ---
            if (AttitudeSensor.current != null)
            {
                Quaternion q = AttitudeSensor.current.attitude.ReadValue();
                
                // 1. Standard Remap for Landscape
                Quaternion gyroRot = Quaternion.Euler(90, 0, 0) * new Quaternion(q.x, q.y, -q.z, -q.w);

                // 2. Apply MOBILE SPECIFIC Offset
                transform.localRotation = Quaternion.Euler(0, mobileRotationOffset, 0) * gyroRot;
            }
        #endif
    }
}