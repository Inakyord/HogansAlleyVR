using UnityEngine;
using UnityEngine.InputSystem; // Required for New Input System

public class CameraController : MonoBehaviour
{
    [Header("PC Settings")]
    public float mouseSensitivity = 15f;

    // Variables for PC Mouse Look
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        // PC: Lock Cursor
        #if UNITY_STANDALONE || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        #endif

        // MOBILE: Enable the Gyroscope/Attitude Sensor
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
        }
    }

    void Update()
    {
        // -----------------------------
        // 1. PC CONTROLS (Mouse)
        // -----------------------------
        #if UNITY_STANDALONE || UNITY_EDITOR
            if (Mouse.current == null) return;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yRotation += mouseDelta.x * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // -----------------------------
        // 2. MOBILE CONTROLS (Gyroscope)
        // -----------------------------
        #elif UNITY_IOS || UNITY_ANDROID
            
            if (AttitudeSensor.current != null)
            {
                // Get the rotation from the phone's sensors
                Quaternion attitude = AttitudeSensor.current.attitude.ReadValue();

                // Unity's coordinate system is different from the phone's.
                // We perform this specific remapping to make it work like a "Magic Window"
                // (This rotates the camera exactly as you rotate the phone)
                transform.localRotation = Quaternion.Euler(90, 0, 0) * new Quaternion(attitude.x, attitude.y, -attitude.z, -attitude.w);
            }
        #endif
    }
}