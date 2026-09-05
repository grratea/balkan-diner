using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager instance { get; private set; }

    [SerializeField] private List<Table> tables = new List<Table>();

    [Header("UPGRADEs")]
    [Tooltip("Extra tables that unlock")]
    [SerializeField] private List<Table> extraTables = new List<Table>();

    public IReadOnlyList<Table> Tables { get { return tables; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        RebuildTableList();
    }

    public void ApplyUpgrades()
    {
        int extra = 0;
        if (UpgradeManager.instance != null)
        {
            extra = UpgradeManager.instance.GetExtraTableCount();
        }

        for (int i = 0; i < extraTables.Count; i++)
        {
            extraTables[i].gameObject.SetActive(i < extra);
        }

        RebuildTableList();
    }

    private void RebuildTableList()
    {
        tables = FindObjectsByType<Table>(FindObjectsSortMode.None)
            .Where(t => t.gameObject.activeInHierarchy)
            .ToList();
    }

    public Table GetFreeTable()
    {
        return tables.FirstOrDefault(t => t.IsFree);
    }

    public bool HasFreeTable
    {
        get { return GetFreeTable() != null; }
    }

    public void ResetForNewDay()
    {
        foreach (Table t in tables)
        {
            t.ResetForNewDay();
        }
    }
}