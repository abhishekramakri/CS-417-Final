using UnityEngine;
using Unity.XR.CoreUtils;

public class Coin : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<XROrigin>() != null)
        {
            CoinManager.AddCoin(value);
            Destroy(gameObject);
        }

    }

    void Update()
    {
        transform.Rotate(0f, 100f * Time.deltaTime, 0f);
    }
}