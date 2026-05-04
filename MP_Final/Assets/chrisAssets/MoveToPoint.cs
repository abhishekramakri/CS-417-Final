using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    private Vector2 target = new Vector2(-1f, -10f);
    private float duration = 3f;

    private Vector2 startPos;
    private float timer = 0f;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = timer / duration;

        // Smooth interpolation (linear movement)
        transform.position = Vector2.Lerp(startPos, target, t);

        // Optional: destroy when finished
        if (t >= 1f)
        {
            transform.position = target;
            Destroy(gameObject); // remove if you don't want them piling up
        }
    }
}