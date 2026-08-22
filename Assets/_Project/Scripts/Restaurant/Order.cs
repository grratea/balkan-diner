using UnityEngine;

// obicna klasa jer ne treba kao objekt, a ni na scenu
public class Order
{
    public DishSO Dish { get; private set; }
    public Table Table { get; private set; } 
    public Customer Customer { get; private set; }

    public bool IsReady { get; set; } // ZA KUHINJU

    public Order (DishSO dish, Table table, Customer customer)
    {
        this.Dish = dish;
        this.Table = table;
        this.Customer = customer;
    }

}
