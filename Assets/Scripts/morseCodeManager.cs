using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
public class morseCodeManager : MonoBehaviour
{
    public AudioSource morseCodeBeeperAudioSource;
    public TMP_Text morseCodeTransmissionTextBox; // Raw morse code youre sending
    public TMP_Text morseCodeToTextTranslationTextBox; // Alphabet characters translated from it

    public Slider dashTimingSlider;
    public Slider completionTimingsSlider;

    private float pressStartTime;
    private float transmissionDelayTime;
    public int currentLetterCount = 1;

    private bool isSpacenarPressed = false;

    public string currentMorseCodeTransmission = "";
    
    private Coroutine waitingThreeSecondsCoroutine;
    private Coroutine waitingOneSecondCoroutine;

    private MorseCodeTranslator morseCodeTranslator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        morseCodeTranslator = new MorseCodeTranslator();
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(KeyCode.Space))
        {
            pressStartTime = Time.time;
            isSpacenarPressed = true;
            if (!morseCodeBeeperAudioSource.isPlaying)
            {
                morseCodeBeeperAudioSource.Play();
            }

            // Make sure to reset the 3 second timer so it doesnt transmit too early
            if (waitingThreeSecondsCoroutine != null)
            {
                StopCoroutine(waitingThreeSecondsCoroutine);
                waitingThreeSecondsCoroutine = null;
            }

            // Make sure to reset the 1 second timer so it doesnt stop too early
            if (waitingOneSecondCoroutine != null)
            {
                StopCoroutine(waitingOneSecondCoroutine);
                waitingOneSecondCoroutine = null;
            }
        }

        // Fill dash slider while spacebar is held
        if (isSpacenarPressed)
        {
            float elapsedMs = (Time.time) - pressStartTime;
            dashTimingSlider.value = Mathf.Clamp(elapsedMs, 0f, .25f);
        }

        // Fill delay timer until transmission is sent
        if (waitingThreeSecondsCoroutine != null)
        {
            float elapsedMs = (Time.time) - transmissionDelayTime;
            completionTimingsSlider.value = Mathf.Clamp(elapsedMs, 0f, 3f);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isSpacenarPressed = false;            
            morseCodeBeeperAudioSource.Stop();
            float heldTime = Time.time - pressStartTime;
            if (heldTime >= 0.25f)
            {
                // Dash was transmitted
                morseCodeTransmissionTextBox.text += "-";
                currentMorseCodeTransmission += "-";
            }
            else
            {
                // Dit was transmitted
                morseCodeTransmissionTextBox.text += ".";
                currentMorseCodeTransmission += ".";
            }

            string currentTranslationLetter = morseCodeToTextTranslationTextBox.text;
            if (currentTranslationLetter.Length == currentLetterCount)
            {
                Debug.Log("Replace");
                morseCodeToTextTranslationTextBox.text = currentTranslationLetter.Substring(0, currentTranslationLetter.Length - 1);
            }
            morseCodeToTextTranslationTextBox.text += morseCodeTranslator.TranslateCurrentCodeToLetter(currentMorseCodeTransmission);

            transmissionDelayTime = Time.time;

            // Start the coroutine to wait 3 seconds for complete transmission
            waitingThreeSecondsCoroutine = StartCoroutine(WaitThreeForTransmissionComplete());

            // Start the coroutine to wait 1 second for letter completion
            waitingOneSecondCoroutine = StartCoroutine(WaitOneSecondForLetterComplete());

        }
    }

    IEnumerator WaitOneSecondForLetterComplete()
    {
        // See if they havent pressed anything in 3 seconds, if so, complete the transmission
        yield return new WaitForSeconds(1f);

        Debug.Log("Letter was complete: ");
        currentLetterCount++;
        currentMorseCodeTransmission = "";
        morseCodeTransmissionTextBox.text += " ";
    }

    IEnumerator WaitThreeForTransmissionComplete()
    {
        // See if they havent pressed anything in 3 seconds, if so, complete the transmission
        yield return new WaitForSeconds(3f);

        Debug.Log("Transmission was complete: " + morseCodeTransmissionTextBox.text);

        // Make sure to reset transmission translation counter
        currentLetterCount = 1;
        morseCodeToTextTranslationTextBox.text = "";
        morseCodeTransmissionTextBox.text = "";
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