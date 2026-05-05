using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static int coins = 0;

    public static void AddCoin(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }
}