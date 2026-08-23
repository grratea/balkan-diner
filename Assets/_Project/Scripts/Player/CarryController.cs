using TMPro;
using UnityEngine;

public class CarryController : MonoBehaviour
{
    [SerializeField] private TextMeshPro carryLabel;

    private Order carriedOrder;

    public Order CarriedOrder {  get { return carriedOrder; } }
    public bool IsCarrying { get { return carriedOrder != null; } }
    public bool IsEmpty { get { return carriedOrder == null; }}

    void Start()
    {
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (carryLabel == null)
        {
            return;
        }

        if (IsCarrying)
        {
            carryLabel.text = $"[{carriedOrder.Dish.displayName}]";
            carryLabel.color = Color.yellow;
        }

        else
        {
            carryLabel.text = "";
        }
    }

    // PROMIJENITI NAZIV FJE
    public bool PickUp(Order order)
    {
        // ne mozes pokupit ako vec nesto imas u rukama
        if (IsCarrying || order == null)
        {
            return false;
        }

        carriedOrder = order;
        RefreshLabel();
        return true;
    }

    public Order Drop()
    {
        Order order = carriedOrder;
        carriedOrder = null;
        RefreshLabel();
        return order;
    }

    void Update()
    {
        
    }
}
