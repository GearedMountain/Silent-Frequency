using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class morseCodeTranslator : MonoBehaviour
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

    private Dictionary<string, string> letterToMorseCode;

    void Awake()
    {
        letterToMorseCode = morseCodeToLetter.ToDictionary(pair => pair.Value, pair => pair.Key);
    }

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

    public string TranslateLetterToMorse(string letter)
    {
        if (letterToMorseCode.TryGetValue(letter.ToUpper(), out string morse))
        {
            return morse;
        }
        else
        {
            return "?";
        }
    }
}
