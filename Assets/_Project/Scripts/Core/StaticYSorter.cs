using UnityEngine;

public class StaticYSorter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private int precision = 100;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (targetRenderer != null)
        {
            targetRenderer.sortingOrder = -(int)(transform.position.y * precision);
        }
    }

#if UNITY_EDITOR
    // osvjezi order dok pomices objekt u editoru
    private void OnValidate()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (targetRenderer != null)
        {
            targetRenderer.sortingOrder = -(int)(transform.position.y * precision);
        }
    }
#endif
}