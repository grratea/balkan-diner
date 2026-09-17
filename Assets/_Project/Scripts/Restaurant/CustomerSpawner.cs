using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private Transform doorPoint;

    [Header("SETTINGS")]
    [SerializeField] private bool spawningEnabled = true;

    private float timer;

    private void Update()
    {
        if (!spawningEnabled)
        {
            return;
        }

        // ne stvaraj goste kad dan nije u tijeku
        if (DayManager.instance == null ||
            DayManager.instance.State != DayState.Playing)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= GameConfig.Balance.spawnInterval)
        {
            timer = 0f;
            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        // nema stola -> ne stvaraj gosta uopce
        if (!TableManager.instance.HasFreeTable)
        {
            return;
        }

        Customer customer = Instantiate(
            customerPrefab,
            doorPoint.position,
            Quaternion.identity);

        customer.Init(doorPoint.position);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayDoorBell();
        }
    }

    private void OnDrawGizmos()
    {
        if (doorPoint == null)
        {
            return;
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(doorPoint.position, Vector3.one * 0.5f);
    }
}