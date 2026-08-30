using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// SINGLETON
public class TableManager : MonoBehaviour
{
    // jedina instanca
    public static TableManager instance {  get; private set; }

    [SerializeField] private List<Table> tables = new List<Table>();

    public IReadOnlyList<Table> Tables => tables;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (tables.Count == 0)
        {
            // spremi sve stolove u listu
            tables = FindObjectsByType<Table>(FindObjectsSortMode.None).ToList();
        }
    }

    public Table GetFreeTable()
    {
        return tables.FirstOrDefault(t => t.IsFree);
    }

    public bool HasFreeTable => GetFreeTable() != null;

    public void ResetForNewDay()
    {
        foreach(Table t in tables)
        {
            t.ResetForNewDay();
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
