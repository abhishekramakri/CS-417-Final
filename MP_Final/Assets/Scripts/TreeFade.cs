using UnityEngine;

public class TreeFade : MonoBehaviour
{
    public float duration = 5f;
    public Vector3 targetScale = Vector3.zero; // shrink to nothing

    private Transform[] treeTransforms;
    private Vector3[] originalScales;
    private float elapsedTime = 0f;

    void Start()
    {
        treeTransforms = GetComponentsInChildren<Transform>();

        originalScales = new Vector3[treeTransforms.Length];

        for (int i = 0; i < treeTransforms.Length; i++)
        {
            originalScales[i] = treeTransforms[i].localScale;
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        for (int i = 0; i < treeTransforms.Length; i++)
        {
            treeTransforms[i].localScale = Vector3.Lerp(originalScales[i], targetScale, t);
        }
    }
}