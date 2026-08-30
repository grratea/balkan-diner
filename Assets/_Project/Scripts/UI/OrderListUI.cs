using TMPro;
using UnityEngine;

public class OrderListUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    void Start()
    {
        OrderManager.instance.OnOrdersChanged += Refresh;
        Refresh();
    }
    
    private void Refresh()
    {
        if (label == null)
        {
            return;
        }

        var orders = OrderManager.instance.ActiveOrders;

        if (orders.Count == 0) 
        {
            label.text = "ORDERS: \n (none)";
            return;
        }

        string text = "ORDERS:\n";
        foreach (Order order in orders) 
        {
            string status;
            if (order.IsCancelled)
            {
                status = "[CANCELLED]";
            }
            else if (order.IsReady)
            {
                status = "[DONE]";
            }
            else
            {
                status = "[COOKING]";
            }

            text += $"{order.Dish.displayName} {status}\n";
        }
        label.text = text;
    }

    private void OnDestroy()
    {
        if (OrderManager.instance != null)
        {
            OrderManager.instance.OnOrdersChanged -= Refresh;
        }
    }

    void Update()
    {
        
    }
}
