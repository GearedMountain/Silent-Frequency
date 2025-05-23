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

    // Information and responses of this transmission 
    public string vesselId;
    public string incomingDirection;

    public void Transmit(string message){
        morseCodeTransmissionTextBox.text = "";
        StartCoroutine(TransmitMorse(message));
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
        StartCoroutine(ClearScreen());
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

    IEnumerator Dit()
    {
        inPause = false;
        incomingSignalAudioSource.Play();
        yield return new WaitForSeconds(ditTime);
        incomingSignalAudioSource.Stop();
    }

    IEnumerator Dash()
    {
        inPause = false;
        incomingSignalAudioSource.Play();
        yield return new WaitForSeconds(dashTime);
        incomingSignalAudioSource.Stop();
    }


}
