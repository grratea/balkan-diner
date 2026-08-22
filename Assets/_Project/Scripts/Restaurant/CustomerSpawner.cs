using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private Transform doorPoint;

    [Header("SETTINGS")]
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private bool spawningEnabled = true;

    private float timer;

    void Start()
    {
        
    }

    void Update()
    {
        if (!spawningEnabled)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnCustomer();
        }
    }
    private void SpawnCustomer()
    {
        if (!TableManager.instance.HasFreeTable)
        {
            return;
        }

        Customer customer = Instantiate(customerPrefab, doorPoint.position, Quaternion.identity);
        customer.Init(doorPoint.position);
    }

    private void OnDrawGizmos()
    {
        if (doorPoint == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(doorPoint.position, Vector3.one * 0.5f);
    }
}
