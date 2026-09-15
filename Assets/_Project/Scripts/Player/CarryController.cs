using TMPro;
using UnityEngine;

public class CarryController : MonoBehaviour
{
    //[SerializeField] private TextMeshPro carryLabel;
    [SerializeField] private SpriteRenderer carryIcon;

    private Order carriedOrder;

    public Order CarriedOrder {  get { return carriedOrder; } }
    public bool IsCarrying { get { return carriedOrder != null; } }
    public bool IsEmpty { get { return carriedOrder == null; }}

    void Start()
    {
        //RefreshLabel();
        RefreshIcon();
    }

    //private void RefreshLabel()
    //{
    //    if (carryLabel == null)
    //    {
    //        return;
    //    }

    //    if (IsCarrying)
    //    {
    //        carryLabel.text = $"[{carriedOrder.Dish.displayName}]";
    //        carryLabel.color = Color.yellow;
    //    }

    //    else
    //    {
    //        carryLabel.text = "";
    //    }
    //}

    // PROMIJENITI NAZIV FJE
    public bool PickUp(Order order)
    {
        // ne mozes pokupit ako vec nesto imas u rukama
        if (IsCarrying || order == null)
        {
            return false;
        }

        carriedOrder = order;
        // ShowCarryIcon(order.Dish);
        RefreshIcon();
        // RefreshLabel();
        return true;
    }

    public Order Drop()
    {
        Order order = carriedOrder;
        carriedOrder = null;
        // HideCarryIcon();
        RefreshIcon();
        // RefreshLabel();
        return order;
    }

    public void ForceDrop()
    {
        carriedOrder = null;
        RefreshIcon();
        // RefreshLabel();
    }

    private void RefreshIcon()
    {
        if (carryIcon == null) {
            return;
        }

        if (carriedOrder != null && carriedOrder.IsReady)
        {
            carryIcon.sprite = carriedOrder.Dish.icon;
        }

        else
        {
            carryIcon.sprite = null;
        }
    }

    private void ShowCarryIcon(DishSO dish)
    {
        if (carryIcon == null)
        {
            return;
        }

        if (dish == null || dish.icon == null)
        {
            carryIcon.gameObject.SetActive(false);
            return;
        }

        carryIcon.sprite = dish.icon;
        carryIcon.gameObject.SetActive(true);
    }

    private void HideCarryIcon()
    {
        if (carryIcon != null)
        {
            carryIcon.gameObject.SetActive(false);
        }
    }
}
