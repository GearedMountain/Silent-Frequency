using UnityEngine;

public class logWriter : MonoBehaviour
{
    public GameObject animatedLogSheet;

    private Animator logSheetAnimator;
    
    public void Start()
    {
        logSheetAnimator = animatedLogSheet.GetComponent<Animator>();
    }
    
    public void Interact(){
        logSheetAnimator.SetTrigger ("NewCardGrabbed");
        Debug.Log("New Log Created");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
