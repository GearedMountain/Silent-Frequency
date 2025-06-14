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
    public morseCodeManager morseCodeManager;
    public handbookControl handbookControl;
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
                        if(!logWriter.beingLookedAt && objectName=="Morse Code Log") {logWriter.beingLookedAt = true;}
                        currentActionTextbox.text = GetInteractionMessage(objectName);
                    }
                }
                return;
            }
        }


         if(logWriter.beingLookedAt) {logWriter.beingLookedAt = false;}

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

            case "Trash Log":
                return "Trash Transmission Log";

            case "Submit Log":
                return "Submit Transmission Log";

            case "Morse Paddle":
                return "Transmit Morse Code";

            case "Morse Paddle INUSE":
                return "Stop Transmitting";
                
            case "Morse Code Log":
                return "Punch Hole in Log";

            case "Alert Command Button":
                return "Alert High Command";

            case "Forward Page":
                return "Turn Page Forward";

            case "Backward Page":
                return "Turn Page Backward";

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
            case "Trash Log":
                logWriter.Trash();
                return;
            case "Submit Log":
                logWriter.Submit();
                return;
            case "Power Console":
            case "Morse Paddle":
                morseCodeManager.Interact();
                return;
            case "Morse Paddle INUSE":
                morseCodeManager.Exit();
                return;

            case "Morse Code Log":
                logWriter.HandlePunchInput();
                return;

            case "Forward Page":
                handbookControl.PageForward();
                return;

            case "Backward Page":
                handbookControl.PageBackward();
                return;

            default:
                return;
        }
    }
}

