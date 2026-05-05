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

    private Renderer[] renderers;

    public GameObject validHitObject;
    private GameObject lastHitObject;

    private bool canTakeDamage = false;
    public float spawnImmunityTime = 3f;

    void OnEnable()
    {
        startPosition = transform.position;
        startPosition.y = 4f;
        transform.position = startPosition;

        timer = 0f;
        isMoving = true;

        renderers = GetComponentsInChildren<Renderer>();

        canTakeDamage = false;
        StartCoroutine(SpawnImmunity());
        // transform.Rotate(0f, 90f, 0f);
    }

    System.Collections.IEnumerator SpawnImmunity()
    {
        yield return new WaitForSeconds(spawnImmunityTime);
        canTakeDamage = true;
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
        
         if (disabled) return;

         if (!canTakeDamage) return;

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (force > 0f)
            DisableMachine();
    }

    System.Collections.IEnumerator FallOver()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(90f, startRot.eulerAngles.y, startRot.eulerAngles.z);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }

    System.Collections.IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        lastHitObject = collision.gameObject;

        Debug.Log("Machine collided with: " + lastHitObject.name);
    }

    void DisableMachine()
    {
        disabled = true;
        isMoving = false;

        MachineStats.RegisterDestroyed();

        // turn red
        foreach (Renderer r in renderers)
        {
            if (r.material != null)
            {
                r.material.SetColor("_BaseColor", Color.red);
            }
        }

        // flip over
        StartCoroutine(FallOver());

        // destroy after delay
        StartCoroutine(DestroyAfterDelay(2f));
    }
}