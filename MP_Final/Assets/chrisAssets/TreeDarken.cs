using UnityEngine;

public class TreeDarken : MonoBehaviour
{
    public float duration = 5f;

    private Renderer[] renderers;
    private Material[][] materials;
    private Color[][] originalColors;

    private float elapsedTime = 0f;

    void Start()
    {
        // Get all renderers in children (includes nested)
        renderers = GetComponentsInChildren<Renderer>();

        materials = new Material[renderers.Length][];
        originalColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;
            originalColors[i] = new Color[materials[i].Length];

            for (int j = 0; j < materials[i].Length; j++)
            {
                originalColors[i][j] = materials[i][j].color;
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                materials[i][j].color = Color.Lerp(originalColors[i][j], Color.black, t);
            }
        }
    }
}