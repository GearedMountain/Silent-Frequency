using UnityEngine;
using TMPro;

public class worldInteractionManager : MonoBehaviour
{
    public float rayDistance = 100f;
    public Color debugRayColor = Color.red;

    public TMP_Text currentActionTextbox;
    private Camera cam;

    private GameObject lastHitObject = null;

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

        Debug.DrawRay(origin, direction * rayDistance, debugRayColor, 0.1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Interactable"))
            {
                if (hitObject != lastHitObject)
                {
                    lastHitObject = hitObject;
                    string objectName = hitObject.name;
                    if (currentActionTextbox != null)
                    {
                        currentActionTextbox.text = GetInteractionMessage(objectName);
                    }
                }
                return;
            }
        }

        // If we get here, either no hit or hit non-interactable
        if (lastHitObject != null)
        {
            lastHitObject = null;
            if (currentActionTextbox != null)
            {
                currentActionTextbox.text = "";
            }
        }
    }

    string GetInteractionMessage(string objectName)
    {
        // Switch based on specific object names
        switch (objectName)
        {
            case "New Transmission Log":
                return "Open New Transmission Log";

            case "Edit Transmission Log":
                return "Edit Existing Transmission Log";

            case "Delete Transmission Log":
                return "Delete This Transmission Log";

            case "Power Console":
                return "Access Power Console";

            case "Morse Terminal":
                return "Use Morse Terminal";

            default:
                return $"Interact with {objectName}";
        }
    }
}

