using UnityEngine;
using ScriptBoy.ProceduralBook;

public class handbookControl : MonoBehaviour
{
    [SerializeField] Book m_Book;
    [SerializeField] AutoTurnSettings m_AutoTurnSettings;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        { 
           m_Book.StartAutoTurning(AutoTurnDirection.Next, m_AutoTurnSettings);
    }
    }
     //   if (Input.GetKeyDown(KeyCode.A))
    //    { 
    //       m_Book.StartAutoTurning(AutoTurnDirection.Back, m_AutoTurnSettings);
    //    }
    
}
