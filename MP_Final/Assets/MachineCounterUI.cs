using TMPro;
using UnityEngine;

public class MachineCounterUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        text.text = "Machines Destroyed: " + MachineStats.machinesDestroyed;
    }
}