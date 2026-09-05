using UnityEngine;

[CreateAssetMenu(fileName = "BalanceConfig", menuName = "Diner/Balance Config")]
public class BalanceConfig : ScriptableObject
{
    [Header("DAY")]
    public int totalDays = 5;
    public float dayDuration = 120f;

    [Header("CUSTOMERS")]
    public float spawnInterval = 4f;
    public float customerPatience = 25f;
    public float customerEatDuration = 4f;

    [Header("START STATE")]
    public int startingTables = 3;
    public int startingStoves = 2;

    [Tooltip("GOAL EARNINGS")]
    public int[] dailyGoals = { 200, 350, 550, 800, 1100 };

    public int GetGoalForDay(int day)
    {
        if (dailyGoals == null || dailyGoals.Length == 0)
        {
            Debug.LogError("dailyGoals je prazan?");
            return 99999;
        }
        // vise kao safety izvan raspona, mozda maknuti
        int index = Mathf.Clamp(day - 1, 0, dailyGoals.Length - 1);
        return dailyGoals[index];
    }
}
