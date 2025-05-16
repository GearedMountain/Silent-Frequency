using UnityEngine;

public class cameraControl : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public float smoothSpeed = 10f;

    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    public float minHorizontalAngle = -70f; // can't turn too far left
    public float maxHorizontalAngle = 70f;  // can't turn too far right

    float xRotation = 0f;
    float yRotation = 0f;

    Quaternion targetRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector3 initialAngles = transform.eulerAngles;
        xRotation = initialAngles.x;
        yRotation = initialAngles.y;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        // Clamp both axes
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        yRotation = Mathf.Clamp(yRotation, minHorizontalAngle, maxHorizontalAngle);

        // Update target and interpolate
        targetRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}
