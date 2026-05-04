using UnityEngine;
using TMPro;

public class ForestIntegrityUI : MonoBehaviour
{
    public PerimeterSpawner spawner;
    public TextMeshProUGUI integrityText;

    void Update()
    {
        if (spawner == null || integrityText == null) return;

        int value = Mathf.RoundToInt(spawner.forestIntegrity);

        // Change color based on health
        if (value > 70)
            integrityText.color = Color.green;
        else if (value > 30)
            integrityText.color = Color.yellow;
        else
            integrityText.color = Color.red;

        integrityText.text = $"Forest Integrity: {value}%";
    }
}

// using UnityEngine;
// using TMPro;

// public class ForestIntegrityUI : MonoBehaviour
// {
//     public PerimeterSpawner spawner;
//     public TextMeshProUGUI integrityText;

//     void Update()
//     {
//         if (spawner == null || integrityText == null) return;

//         integrityText.text =
//             "Forest Integrity: " +
//             Mathf.RoundToInt(spawner.forestIntegrity) + "%";
//     }
// }