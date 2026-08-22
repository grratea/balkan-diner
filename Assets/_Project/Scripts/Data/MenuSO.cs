using UnityEngine;

[CreateAssetMenu(fileName = "Menu", menuName = "Diner/Menu")]
public class MenuSO : ScriptableObject
{
    public DishSO[] availableDishes;

    public DishSO GetRandomDish()
    {
        if (availableDishes == null || availableDishes.Length == 0)
        {
            Debug.LogError("Menu is empty");
            return null;
        }
        return availableDishes[Random.Range(0, availableDishes.Length)];
    }
}
