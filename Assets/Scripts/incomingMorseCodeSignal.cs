using UnityEngine;
using System.Collections;

public class incomingMorseCodeSignal : MonoBehaviour
{
    public AudioSource incomingSignalAudioSource;
    private bool inPause = false;

    public float ditTime;
    public float dashTime;

    public GameObject receivingLightGameobject;
    public Material[] receivingLightMaterials;

    public morseCodeTranslator morseCodeTranslator;

    // Information and responses of this transmission 
    public string vesselId;
    public string incomingDirection;

    public void Transmit(string message){
        StartCoroutine(Dit());
    }

    IEnumerator Dit()
    {
        inPause = false;
        incomingSignalAudioSource.Play();
        yield return new WaitForSeconds(ditTime);
        incomingSignalAudioSource.Stop();
    }

    IEnumerator MorseSymbolPause()
    {
        inPause = true;
        yield return new WaitForSeconds(ditTime);

    }
}
