using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance { get; private set; }

    [SerializeField] private List<UpgradeSO> availableUpgrades = new List<UpgradeSO>();

    private Dictionary<UpgradeSO, int> levels = new Dictionary<UpgradeSO, int>();

    public IReadOnlyList<UpgradeSO> AvailableUpgrades { get { return availableUpgrades; } }

    public event Action<UpgradeSO, int> OnUpgradePurchased;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach (UpgradeSO u in availableUpgrades)
        {
            levels[u] = 0;
        }
    }



    public int GetLevel(UpgradeSO upgrade)
    {
        if (upgrade == null)
        {
            return 0;
        }
        return levels.TryGetValue(upgrade, out int level) ? level : 0;
    }

    public bool IsMaxLevel(UpgradeSO upgrade)
    {
        return GetLevel(upgrade) >= upgrade.MaxLevel;
    }

    public int GetNextCost(UpgradeSO upgrade)
    {
        return upgrade.GetCost(GetLevel(upgrade) + 1);
    }

    public bool CanAfford(UpgradeSO upgrade)
    {
        if (IsMaxLevel(upgrade))
        {
            return false;
        }
        return GameManager.instance.Money >= GetNextCost(upgrade);
    }


    public bool TryPurchase(UpgradeSO upgrade)
    {
        if (upgrade == null || IsMaxLevel(upgrade))
        {
            return false;
        }

        int cost = GetNextCost(upgrade);

        if (!GameManager.instance.SpendMoney(cost))
        {
            return false;
        }

        int newLevel = GetLevel(upgrade) + 1;
        levels[upgrade] = newLevel;

        OnUpgradePurchased?.Invoke(upgrade, newLevel);
        return true;
    }





    public float GetCookTimeMultiplier()
    {
        UpgradeSO u = FindByType(UpgradeType.CookSpeed);
        if (u == null)
        {
            return 1f;
        }

        int level = GetLevel(u);
        if (level == 0)
        {
            return 1f;
        }

        return u.GetValue(level);
    }

    public int GetExtraTableCount()
    {
        UpgradeSO u = FindByType(UpgradeType.ExtraTable);
        if (u == null)
        {
            return 0;
        }

        int level = GetLevel(u);
        if (level == 0)
        {
            return 0;
        }

        return Mathf.RoundToInt(u.GetValue(level));
    }

    private UpgradeSO FindByType(UpgradeType type)
    {
        foreach (UpgradeSO u in availableUpgrades)
        {
            if (u.type == type)
            {
                return u;
            }
        }
        return null;
    }

    public void ResetAll()
    {
        foreach (UpgradeSO u in availableUpgrades)
        {
            levels[u] = 0;
        }
    }
}