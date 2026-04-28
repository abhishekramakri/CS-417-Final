using UnityEngine;

// Attach to wood/metal scrap prefabs. Rigidbody handles physics drop.
// Adi's grab interactable goes on this same GameObject so it can be thrown.
public class Scrap : MonoBehaviour
{
    public enum ScrapType { Wood, Metal }
    public ScrapType scrapType;
}
