using UnityEngine;
using ScriptBoy.ProceduralBook;

public class handbookControl : MonoBehaviour
{
    public AudioSource bookAudioSource;
    [SerializeField] Book m_Book;
    [SerializeField] AutoTurnSettings m_AutoTurnSettings;
    public void Update()
    {
        
     
    }

    public void PageForward(){
        if (!m_Book.isAutoTurning)
        { 
            m_Book.StartAutoTurning(AutoTurnDirection.Next, m_AutoTurnSettings);
            bookAudioSource.Play();   
        }
    }

    public void PageBackward(){
        if (!m_Book.isAutoTurning)
        { 
            m_Book.StartAutoTurning(AutoTurnDirection.Back, m_AutoTurnSettings);
            bookAudioSource.Play();   
        }
    }
     //   if (Input.GetKeyDown(KeyCode.A))
    //    { 
    //       m_Book.StartAutoTurning(AutoTurnDirection.Back, m_AutoTurnSettings);
    //    }
    
}
