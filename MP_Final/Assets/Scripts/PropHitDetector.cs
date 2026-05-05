using UnityEngine;

public class PropHitDetector : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable =
            collision.gameObject.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            float force = rb.linearVelocity.magnitude;

            damageable.TakeHit(force);
        }
    }
}