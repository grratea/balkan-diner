using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class OrderManager : MonoBehaviour
{
    public static OrderManager instance { get; private set; }
    private List<Order> activeOrders = new List<Order>();
    public IReadOnlyList<Order> ActiveOrders {  get { return activeOrders; } }
    public int Count {  get { return activeOrders.Count; } }
    public event Action OnOrdersChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void AddOrder(Order order)
    {
        activeOrders.Add(order);
        OnOrdersChanged?.Invoke();
    }

    public void RemoveOrder(Order order) 
    { 
        if (activeOrders.Remove(order))
        {
            OnOrdersChanged?.Invoke();
        }
    }

    public Order GetOrderForTable(Table table)
    {
        return activeOrders.FirstOrDefault(o => o.Table == table);
    }

    // zove Stove kada zavrsi s kuhanjem
    public void NotifyOrdersChanged()
    {
        OnOrdersChanged?.Invoke();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
