using UnityEngine;

public class cameraControl : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public float smoothSpeed = 10f;

    public float minVerticalAngle = -70f;
    public float maxVerticalAngle = 70f;

    public float minHorizontalAngle = -120f;
    public float maxHorizontalAngle = 120f;

    public float normalFOV = 60f;
    public float zoomFOV = 30f;
    public float zoomSpeed = 10f;

    float xRotation = 0f;
    float yRotation = 0f;

    Quaternion targetRotation;
    Camera cam;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector3 initialAngles = transform.eulerAngles;
        xRotation = initialAngles.x;
        yRotation = initialAngles.y;
        targetRotation = transform.rotation;

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraControl script must be attached to a Camera component.");
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        yRotation = Mathf.Clamp(yRotation, minHorizontalAngle, maxHorizontalAngle);

        targetRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

        // Zoom with right-click
        if (cam != null)
        {
            float targetFOV = Input.GetMouseButton(1) ? zoomFOV : normalFOV;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
        }
    }
}
