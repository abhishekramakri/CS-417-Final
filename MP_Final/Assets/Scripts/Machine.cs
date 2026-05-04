using UnityEngine;

public class Machine : MonoBehaviour, IDamageable
{
    public bool disabled = false;

    public float moveSpeed = 2f;

    public Vector3 moveDirection = Vector3.right;

    public AudioSource audioSource;
    public AudioClip hitSound;

    void Update()
    {
        if (!disabled)
        {
            MoveMachine();
        }
    }

    void MoveMachine()
    {
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }

    public void TakeHit(float force)
    {
        if (disabled) return;

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (force > 5f)
        {
            disabled = true;
        }
    }
}