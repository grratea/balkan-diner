using UnityEngine;
using UnityEngine.Rendering;

// zasad samo dva updatea
public enum UpgradeType
{
    CookSpeed, ExtraTable
}

[CreateAssetMenu(fileName = "Upgrade_", menuName = "Diner/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    [Header("DISPLAY")]
    public string displayName = "New upgrade";
    [TextArea] public string description = "Description";

    [Header("LOGIC")]
    public UpgradeType type;

    [Tooltip("COST PER LEVEL")]
    public int[] costPerLevel = { 150, 400 };

    [Tooltip("VALUE PER LEVEL")]
    public float[] valuePerLevel = { 0.8f, 0.6f };

    public int MaxLevel { get { return costPerLevel.Length; } }

    public int GetCost(int level)
    {
        int index = level - 1;
        if (index < 0 || index >= costPerLevel.Length)
        {
            return int.MaxValue;
        }
        return costPerLevel[index];
    }

    public float GetValue(int level)
    {
        if (level <= 0)
        {
            return 0f;
        }
        // jos pogledati
        int index = Mathf.Clamp(level - 1, 0, valuePerLevel.Length - 1);
        return valuePerLevel[index];
    }
}
