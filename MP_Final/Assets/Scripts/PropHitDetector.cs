using UnityEngine;

public class PropHitDetector : MonoBehaviour
{
    Rigidbody rb;
    ThrowableProp throwable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        throwable = GetComponent<ThrowableProp>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (throwable == null || !throwable.isThrown)
            return;

        IDamageable damageable =
            collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            float force = rb.linearVelocity.magnitude;
            damageable.TakeHit(force);
        }
    }
}