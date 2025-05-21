using UnityEngine;
using TMPro;

public class worldInteractionManager : MonoBehaviour
{
    public float rayDistance = 100f;
    public Color debugRayColor = Color.red;

    public TMP_Text currentActionTextbox;

    private bool lookingAtSomething = false;
    private Camera cam;
    private GameObject lastHitObject = null;

    private string objectName;
    // Gameobjects for various interactbles and their scripts
    public logWriter logWriter; 

    void Start()
    {
        // Get the Camera component from the current GameObject
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        FireRaycast();
        if (lookingAtSomething && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithObject(objectName);
        }
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
                    objectName = hitObject.name;
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
                lookingAtSomething = false;
            }
        }
    }

    string GetInteractionMessage(string objectName)
    {
        lookingAtSomething = true;
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

    void InteractWithObject(string objectName)
    {
        // Switch based on specific object names
        switch (objectName)
        {
            case "New Transmission Log":
                logWriter.Interact();
                return;
            case "Edit Transmission Log":

            case "Delete Transmission Log":

            case "Power Console":

            case "Morse Terminal":

            default:
                return;
        }
    }
}

