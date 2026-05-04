using UnityEngine;

public class Machine : MonoBehaviour, IDamageable
{
    public bool disabled = false;

    public float moveSpeed = 2f;

    public Vector3 moveDirection = Vector3.right;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    [Header("Movement (X-Z plane)")]
    public Vector3 targetPosition = new Vector3(-1f,4f, -10f);
    public float moveDuration = 10f;

    private Vector3 startPosition;
    private float timer;
    private bool isMoving = false;

    void OnEnable()
    {
        startPosition = transform.position;
        startPosition.y = 4f;
        transform.position = startPosition;

        timer = 0f;
        isMoving = true;
        // transform.Rotate(0f, 90f, 0f);
    }
    void Update()
    {
        if (disabled || !isMoving) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / moveDuration);

        Vector3 pos = Vector3.Lerp(startPosition, targetPosition, t);
        pos.y = 4f;

        transform.position = pos;

        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0f;
        float rotationSpeed = 2f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Add 90° offset
            targetRotation *= Quaternion.Euler(0f, 270f, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        if (t >= 1f)
        {
            transform.position = new Vector3(targetPosition.x, 4f, targetPosition.z);

            isMoving = false;

          
            Destroy(gameObject);
        }
    }

    public void TakeHit(float force)
    {
        if (!disabled)
        {
            MoveMachine();
        }
    }

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (force > 1f)
            DisableMachine();
    }

    void DisableMachine()
    {
        disabled = true;
        isMoving = false;

        Destroy(gameObject);
    }
}
// using UnityEngine;

// public class Machine : MonoBehaviour, IDamageable
// {
//     public bool disabled;

//     public AudioSource audioSource;
//     public AudioClip hitSound;

//     public void TakeHit(float force)
//     {
//         if (disabled) return;

//         Debug.Log("Machine hit with force: " + force);

//         audioSource.PlayOneShot(hitSound);

//         if (force > 1f)
//         {
//             DisableMachine();
//         }
//     }

//     void DisableMachine()
//     {
//         disabled = true;
//         Debug.Log("Machine disabled!");
//     }
// }