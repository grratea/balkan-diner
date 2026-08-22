using UnityEngine;

[CreateAssetMenu(fileName = "Dish_", menuName = "Diner/Dish")]
public class DishSO : ScriptableObject
{
    [Header("BASIC")]
    public string displayName = "New Dish";

    // TODO: cooktime
    // TODO: price
    // TODO: icon
}
