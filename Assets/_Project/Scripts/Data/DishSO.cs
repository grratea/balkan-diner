using UnityEngine;

[CreateAssetMenu(fileName = "Dish_", menuName = "Diner/Dish")]
public class DishSO : ScriptableObject
{
    [Header("BASIC")]
    public string displayName = "New Dish";

    [Header("COOKING")]
    public float cookTime = 5f;

    [Header("PRICING")]
    public int price = 40;

    // TODO: icon
    [Header("VISUAL")]
    public Sprite icon;
}
