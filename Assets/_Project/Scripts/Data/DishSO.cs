using UnityEngine;

[CreateAssetMenu(fileName = "Dish_", menuName = "Diner/Dish")]
public class DishSO : ScriptableObject
{
    [Header("BASIC")]
    public string displayName = "New Dish";

    [Header("COOKING")]
    [Min(0.5f)] // kao minimum
    public float cookTime = 5f;

    // TODO: price
    // TODO: icon
}
