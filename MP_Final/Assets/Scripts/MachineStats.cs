using UnityEngine;

public class MachineStats : MonoBehaviour
{
    public static int machinesDestroyed = 0;

    public static void RegisterDestroyed()
    {
        machinesDestroyed++;
        Debug.Log("Machines destroyed: " + machinesDestroyed);
    }
}