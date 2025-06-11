using UnityEngine;

public class randomVesselGenerator : MonoBehaviour
{

    public void CreateRandomFriendly(transmissionInteraction v) {
        
        if(Random.Range(0,2) == 1)
        {
            v.IDR = "IL" + RandomVesselNumber().ToString();
        } else {
            v.IDR = "CV" + RandomVesselNumber().ToString();
        }
        
    }

    private int RandomVesselNumber()
    {
        return Random.Range(1000, 10000);
    }
}
