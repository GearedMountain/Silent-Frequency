using UnityEngine;

public class worldInteractionManager : MonoBehaviour
{
    public float rayDistance = 100f;
    public Color debugRayColor = Color.red;

    private Camera cam;

    void Start()
    {
        // Get the Camera component from the current GameObject
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        FireRaycast();
    }

    void FireRaycast()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // Draw debug ray
        Debug.DrawRay(origin, direction * rayDistance, debugRayColor, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Interactable"))
            {
                Debug.Log("Hit Interactable: " + hitObject.name);
            }
        }
    }
}
