using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class morseCodeManager : MonoBehaviour
{
    public bool allowedToTransmit = true;

    public AudioSource morseCodeBeeperAudioSource;
    public AudioSource incomingSignalAudioSource;

    public TMP_Text morseCodeTransmissionTextBox;
    public TMP_Text morseCodeToTextTranslationTextBox;
    public TMP_Text liveKeypressTextBox; // <- NEW FIELD

    public GameObject[] ledObjects;
    public GameObject morseCodeTranslationHover;
    public GameObject morseCodePaddle;
    public GameObject morseCodePaddleInUse;

    private Animator morseCodeTranslationAnimator;
    private Animator morseCodePaddleAnimator;

    public Material ledOnMaterial;
    public Material ledOffMaterial;

    [Header("Timing Settings")]
    public float wordDelayTime = 3f;
    private float letterDelayTime => wordDelayTime * 0.4f;

    private float pressStartTime;
    private float transmissionDelayTime;
    public int currentLetterCount = 1;

    private bool isSpacenarPressed = false;
    private bool hasSwitchedToDash = false;
    private bool middleOfSendingTransmission = false;
    public string currentMorseCodeTransmission = "";

    private Coroutine waitingWordCoroutine;
    private Coroutine waitingLetterCoroutine;

    public morseCodeTranslator morseCodeTranslator;
    public incomingMorseCodeSignal incomingMorseCodeSignal;
    private bool currentlySendingTransmission = false;

    public cameraControl cameraControl;

    void Start()
    {
        morseCodeTranslationAnimator = morseCodeTranslationHover.GetComponent<Animator>();
        morseCodePaddleAnimator = morseCodePaddle.GetComponent<Animator>();
        //morseCodeTranslator = new MorseCodeTranslator();
        ResetLEDs();
        liveKeypressTextBox.text = "";
    }

    public void Interact()
    {
        morseCodeTranslationAnimator.SetBool("TranslatorActive", true);
        currentlySendingTransmission = true;
        cameraControl.isCameraLocked = true;
        morseCodePaddleInUse.SetActive(true);
    }
    
    //public void CreateIncomingTransmission(){
       // incomingMorseCodeSignal.Transmit("HELPQT0");
    //}

    public void Exit(){
        morseCodeTranslationAnimator.SetBool("TranslatorActive", false);
        currentlySendingTransmission = false;
        cameraControl.isCameraLocked = false;
        morseCodePaddleInUse.SetActive(false);
        StopTransmittingMorse(true);

    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && currentlySendingTransmission && !incomingMorseCodeSignal.morseCodeOnCooldown)
        {
            if (!middleOfSendingTransmission){
                middleOfSendingTransmission = true;
                morseCodeToTextTranslationTextBox.text = "";
                morseCodeTransmissionTextBox.text = "";
            }
            pressStartTime = Time.time;
            isSpacenarPressed = true;
            hasSwitchedToDash = false;
            liveKeypressTextBox.text = ".";

            if (!morseCodeBeeperAudioSource.isPlaying)
                morseCodeBeeperAudioSource.Play();

            if (waitingWordCoroutine != null)
            {
                StopCoroutine(waitingWordCoroutine);
                waitingWordCoroutine = null;
            }

            if (waitingLetterCoroutine != null)
            {
                StopCoroutine(waitingLetterCoroutine);
                waitingLetterCoroutine = null;
            }

            ResetLEDs();
        }

        if (isSpacenarPressed)
        {
            morseCodePaddleAnimator.SetBool("PaddleDown", true);
            float elapsed = Time.time - pressStartTime;

            if (!hasSwitchedToDash && elapsed >= 0.25f)
            {
                liveKeypressTextBox.text = "-";
                hasSwitchedToDash = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && currentlySendingTransmission && !incomingMorseCodeSignal.morseCodeOnCooldown)
        {
            StopTransmittingMorse(false);
        }
    }

    private void StopTransmittingMorse(bool forced)
    {
        morseCodePaddleAnimator.SetBool("PaddleDown", false);
        isSpacenarPressed = false;
        morseCodeBeeperAudioSource.Stop();

        float heldTime = Time.time - pressStartTime;
        string symbol = heldTime >= 0.25f ? "-" : ".";

        liveKeypressTextBox.text = "";

        if(!forced)
        {
            

            currentMorseCodeTransmission += symbol;
            morseCodeTransmissionTextBox.text += symbol;


            string currentText = morseCodeToTextTranslationTextBox.text;
            if (currentText.Length == currentLetterCount)
            {
                morseCodeToTextTranslationTextBox.text = currentText.Substring(0, currentText.Length - 1);
            }

            morseCodeToTextTranslationTextBox.text += morseCodeTranslator.TranslateCurrentCodeToLetter(currentMorseCodeTransmission);
            transmissionDelayTime = Time.time;

            waitingWordCoroutine = StartCoroutine(WaitForWordComplete());
            waitingLetterCoroutine = StartCoroutine(WaitForLetterComplete());
        } else{
            ForceTransmit();
            StopCoroutine(waitingWordCoroutine);
            StopCoroutine(waitingLetterCoroutine);
        }
    }

    public void ForceTransmit(){
        if (!incomingMorseCodeSignal.morseCodeOnCooldown && morseCodeTransmissionTextBox.text != ""){
            incomingMorseCodeSignal.PlayerTransmitMessage(morseCodeToTextTranslationTextBox.text);
            currentMorseCodeTransmission = "";
            morseCodeTransmissionTextBox.text += " ";
            currentLetterCount = 1;
            morseCodeToTextTranslationTextBox.text = "";
            morseCodeTransmissionTextBox.text = "";
            ResetLEDs();
        }
    }

    IEnumerator WaitForLetterComplete()
    {
        yield return new WaitForSeconds(letterDelayTime);
        Debug.Log("Letter completed.");
        currentLetterCount++;
        currentMorseCodeTransmission = "";
        morseCodeTransmissionTextBox.text += " ";
    }

    IEnumerator WaitForWordComplete()
    {
        float stepTime = wordDelayTime / ledObjects.Length;

        for (int i = 0; i < ledObjects.Length; i++)
        {
            yield return new WaitForSeconds(stepTime);
            SetLEDState(i, true);
        }

        Debug.Log($"Word completed : {morseCodeToTextTranslationTextBox.text}");
        incomingMorseCodeSignal.PlayerTransmitMessage(morseCodeToTextTranslationTextBox.text);
        middleOfSendingTransmission = false;
        currentLetterCount = 1;
        morseCodeToTextTranslationTextBox.text = "";
        morseCodeTransmissionTextBox.text = "";
        ResetLEDs();
    }

    void SetLEDState(int index, bool isOn)
    {
        if (index >= 0 && index < ledObjects.Length)
        {
            Renderer renderer = ledObjects[index].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = isOn ? ledOnMaterial : ledOffMaterial;
            }
        }
    }

    void ResetLEDs()
    {
        foreach (GameObject led in ledObjects)
        {
            Renderer renderer = led.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = ledOffMaterial;
            }
        }
    }
}


