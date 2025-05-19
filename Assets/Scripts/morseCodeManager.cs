using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class morseCodeManager : MonoBehaviour
{
    public AudioSource morseCodeBeeperAudioSource;
    public TMP_Text morseCodeTransmissionTextBox;
    public TMP_Text morseCodeToTextTranslationTextBox;

    public GameObject[] ledObjects;
    public Material ledOnMaterial;
    public Material ledOffMaterial;

    [Header("Timing Settings")]
    public float wordDelayTime = 3f; // Total time for word timeout
    private float letterDelayTime => wordDelayTime * 0.4f; // 40% of word timeout

    private float pressStartTime;
    private float transmissionDelayTime;
    public int currentLetterCount = 1;

    private bool isSpacenarPressed = false;
    public string currentMorseCodeTransmission = "";

    private Coroutine waitingWordCoroutine;
    private Coroutine waitingLetterCoroutine;

    private MorseCodeTranslator morseCodeTranslator;

    void Start()
    {
        morseCodeTranslator = new MorseCodeTranslator();
        ResetLEDs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pressStartTime = Time.time;
            isSpacenarPressed = true;

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

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isSpacenarPressed = false;
            morseCodeBeeperAudioSource.Stop();

            float heldTime = Time.time - pressStartTime;
            string symbol = heldTime >= 0.25f ? "-" : ".";
            currentMorseCodeTransmission += symbol;
            morseCodeTransmissionTextBox.text += symbol;

            // Replace previous letter if still editing it
            string currentText = morseCodeToTextTranslationTextBox.text;
            if (currentText.Length == currentLetterCount)
            {
                morseCodeToTextTranslationTextBox.text = currentText.Substring(0, currentText.Length - 1);
            }

            morseCodeToTextTranslationTextBox.text += morseCodeTranslator.TranslateCurrentCodeToLetter(currentMorseCodeTransmission);

            transmissionDelayTime = Time.time;

            waitingWordCoroutine = StartCoroutine(WaitForWordComplete());
            waitingLetterCoroutine = StartCoroutine(WaitForLetterComplete());
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

        Debug.Log("Word completed.");

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


public class MorseCodeTranslator
{
    
    Dictionary<string, string> morseCodeToLetter = new Dictionary<string, string>()
    {
        { ".-", "A" },
        { "-...", "B" },
        { "-.-.", "C" },
        { "-..", "D" },
        { ".", "E" },
        { "..-.", "F" },
        { "--.", "G" },
        { "....", "H" },
        { "..", "I" },
        { ".---", "J" },
        { "-.-", "K" },
        { ".-..", "L" },
        { "--", "M" },
        { "-.", "N" },
        { "---", "O" },
        { ".--.", "P" },
        { "--.-", "Q" },
        { ".-.", "R" },
        { "...", "S" },
        { "-", "T" },
        { "..-", "U" },
        { "...-", "V" },
        { ".--", "W" },
        { "-..-", "X" },
        { "-.--", "Y" },
        { "--..", "Z" },
        
        // Numbers
        { "-----", "0" },
        { ".----", "1" },
        { "..---", "2" },
        { "...--", "3" },
        { "....-", "4" },
        { ".....", "5" },
        { "-....", "6" },
        { "--...", "7" },
        { "---..", "8" },
        { "----.", "9" }
    };

    public string TranslateCurrentCodeToLetter(string currentMorseCode)
    {
        if (morseCodeToLetter.TryGetValue(currentMorseCode, out string letter))
        {
            return letter;
        }
        else
        {
           return "?";
        }
    }

}