using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TMPro;
public class incomingMorseCodeSignal : MonoBehaviour
{
    public AudioSource incomingSignalAudioSource;
    public bool morseCodeOnCooldown = false;

    public float ditTime;
    public float dashTime;

    public GameObject receivingLightGameobject;
    public Material[] receivingLightMaterials;

    public TMP_Text morseCodeTransmissionTextBox;
    public TMP_Text incomingTransmissionFrequency;

    public Dictionary<string, string> classificationFrequencies = new Dictionary<string, string>()
    {
        { "SUBMARINE", "10\nkHz" },
        { "DESTROYER", "35\nkHz" },
        { "CARRIER", "50\nkHz" },
        { "PATROL", "85\nkHz" },
        { "CIVILLIAN", "100\nkHz" },
        { "DISTRESS", "500\nkHz" }
    };

    public morseCodeTranslator morseCodeTranslator;

    private transmissionInteraction currentCommunication;
    void Start(){
        // CREATE FIRST TRANSMISSION RESPONSE *TEMPORARY*
        transmissionInteraction introVessel = new transmissionInteraction(this);
        currentCommunication = introVessel;
        introVessel.StartInteraction();
        receivingLightGameobject.GetComponent<Renderer>().material = receivingLightMaterials[1];
    }

    public void Transmit(string message){
        incomingTransmissionFrequency.text = classificationFrequencies[currentCommunication.frequencyClass];
        morseCodeOnCooldown = true;

        receivingLightGameobject.GetComponent<Renderer>().material = receivingLightMaterials[0];
        morseCodeTransmissionTextBox.text = "";
        StartCoroutine(TransmitMorse(message));
    }

    public void TransmitResponseWithDelay(string response, float averageWaitTime)
    {
        morseCodeOnCooldown = true;
        StartCoroutine(TransmitResponseWithDelayCoroutine(response,averageWaitTime));

    }

    public IEnumerator TransmitResponseWithDelayCoroutine(string response, float averageWaitTime)
    {
        Debug.Log("received response");
        yield return new WaitForSeconds(averageWaitTime);
        Transmit(response);

    }

    public void PlayerTransmitMessage(string message){
        currentCommunication.ReadTransmission(message);
    }

    private IEnumerator TransmitMorse(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            string morse = morseCodeTranslator.TranslateLetterToMorse(c.ToString());
            yield return StartCoroutine(TransmitCharacter(morse));
            morseCodeTransmissionTextBox.text += c;
            yield return new WaitForSeconds(dashTime); // wait after letter
            
        }
        receivingLightGameobject.GetComponent<Renderer>().material = receivingLightMaterials[1];
        morseCodeOnCooldown = false;

        //StartCoroutine(ClearScreen());
    }

    private IEnumerator TransmitCharacter(string morse)
    {
        foreach (char symbol in morse)
        {
            if (symbol == '.')
            {
                yield return StartCoroutine(Dit());
            }
            else if (symbol == '-')
            {
                yield return StartCoroutine(Dash());
            }
            
            yield return new WaitForSeconds(ditTime); // pause between symbols
        }
    }

    private IEnumerator ClearScreen()
    {
        yield return new WaitForSeconds(5f);
        morseCodeTransmissionTextBox.text = "";
    }

    private float fadeTime = 0.01f;

    IEnumerator Dit()
    {

        incomingSignalAudioSource.volume = 0f;
        incomingSignalAudioSource.Play();

        // Fade in
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            incomingSignalAudioSource.volume = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        yield return new WaitForSeconds(ditTime - 2 * fadeTime);

        // Fade out
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            incomingSignalAudioSource.volume = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        incomingSignalAudioSource.Stop();
    }

    IEnumerator Dash()
    {

        incomingSignalAudioSource.volume = 0f;
        incomingSignalAudioSource.Play();

        // Fade in
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            incomingSignalAudioSource.volume = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        yield return new WaitForSeconds(dashTime - 2 * fadeTime);

        // Fade out
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            incomingSignalAudioSource.volume = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        incomingSignalAudioSource.Stop();
    }

}

// Information and responses of this transmission 

public class transmissionInteraction : MonoBehaviour
{
    public string vesselId;
    public string incomingDirection;

    public string entryMessage = "INC";

    public string IDR = "IL5336";
    public string REQ = "DOCK";
    public string LCT = "NORTH";
    public string CLS = "PATROL";

    public string frequencyClass = "PATROL";

    public float averageWaitTime = 8;
    private incomingMorseCodeSignal mainClass;
    public transmissionInteraction(incomingMorseCodeSignal mainClassRef)
    {
        mainClass = mainClassRef;
    }

    public void StartInteraction()
    {
        mainClass.Transmit(entryMessage);
    }

    public void ReadTransmission(string receivedMessage)
    {

        switch (receivedMessage)
        {
            case "IDR":
                SendResponse(IDR);
                return;
            case "REQ":
                SendResponse(REQ);
                return;  
            case "LCT":
                SendResponse(LCT);
                return;    
            case "CLS":
                SendResponse(CLS);
                return;  
            default:
                SendResponse("RRQ");
                return;
        }
    }

    public void SendResponse(string response)
    {
       mainClass.TransmitResponseWithDelay(response, averageWaitTime);
    }
}