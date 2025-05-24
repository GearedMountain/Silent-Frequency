using UnityEngine;
using System.Collections;
using TMPro;
public class incomingMorseCodeSignal : MonoBehaviour
{
    public AudioSource incomingSignalAudioSource;
    private bool inPause = false;

    public float ditTime;
    public float dashTime;

    public GameObject receivingLightGameobject;
    public Material[] receivingLightMaterials;

    public TMP_Text morseCodeTransmissionTextBox;


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
        receivingLightGameobject.GetComponent<Renderer>().material = receivingLightMaterials[0];
        morseCodeTransmissionTextBox.text = "";
        StartCoroutine(TransmitMorse(message));
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
        inPause = false;
       

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
        inPause = false;
       

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
            default:
                SendResponse("RRQ");
                return;
        }
    }

    IEnumerator SendResponse(string response)
    {
        yield return new WaitForSeconds(averageWaitTime);
        mainClass.Transmit(response);
    }
}