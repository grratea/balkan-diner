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
        if (order == null)
        {
            return;
        }

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

    public void ResetForNewDay()
    {
        activeOrders.Clear();
        OnOrdersChanged?.Invoke();
    }

    public void CancelOrder(Order order)
    {
        if (order == null)
        {
            return;
        }

        order.IsCancelled = true;

        // ako je na stednjaku, pusti da se kuha, ali ju player mora baciti
        if (IsOrderOnStove(order))
        {
            OnOrdersChanged?.Invoke();

        }
        // ako nije na stednjaku, sam ju makni
        else
        {   
            RemoveOrder(order);
        }
    }

    private bool IsOrderOnStove(Order order)
    {
        foreach(Stove s in StoveManager.instance.Stoves)
        {
            if (s.CurrentOrder == order)
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
