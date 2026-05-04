using UnityEngine;

public class ThrowableProp : MonoBehaviour
{
    public bool isThrown;

    public void OnReleased()
    {
        isThrown = true;
    }
}