using UnityEngine;

public class YSorter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private int precision = 100;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // zove se NAKON sto se objekt pomaknuo
    // u updateom bi sortiranje kasnilo jedan frame
    private void Update()
    {
        if (targetRenderer == null)
        {
            return;
        }

        // minus jer manji Y (nize na ekranu) treba VECI order.
        targetRenderer.sortingOrder = -(int)(transform.position.y * precision);
    }
}