using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        text.text = "Coins: " + CoinManager.coins;
    }
}