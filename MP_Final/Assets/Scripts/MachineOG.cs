using UnityEngine;

public class MachineOG : MonoBehaviour, IDamageable
{
    public bool disabled;

    public AudioSource audioSource;
    public AudioClip hitSound;

    public void TakeHit(float force)
    {
        if (disabled) return;

        Debug.Log("Machine hit with force: " + force);

        audioSource.PlayOneShot(hitSound);

        if (force > 1f)
        {
            DisableMachine();
        }
    }

    void DisableMachine()
    {
        disabled = true;
        Debug.Log("Machine disabled!");
    }
}