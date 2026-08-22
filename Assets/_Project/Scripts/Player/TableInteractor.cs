using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Mover))]
public class TableInteractor : MonoBehaviour
{
    [SerializeField] private LayerMask interactableMask;
    private Camera cam;
    private Mover mover;

    private void Awake()
    {
        cam = Camera.main;
        mover = GetComponent<Mover>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }
        if (!Mouse.current.leftButton.wasPressedThisFrame) 
        {
            return;
        }
        // dohvacanje stola na koji je player kliknuo
        Table table = GetTableUnderMouse(); 
        if (table == null)
        {
            return;
        }

        if (!HasWorkAt(table))
        {
            return;
        }

        mover.MoveTo(table.ServePosition, () => Interact(table));
    }

    private Table GetTableUnderMouse()
    {
        Vector2 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D hit = Physics2D.OverlapPoint(world, interactableMask);
        return hit != null && hit.TryGetComponent(out Table table) ? table : null;
    }

    private bool HasWorkAt(Table table) 
    {
        // player dolazi na stol samo kad je prljav ili treba uzeti narudzbu
        return table.State == TableState.Seated || table.State == TableState.Dirty || table.State == TableState.Ordered;
        // PRIVREMENO ORDERED
    }

    // zove Mover sto znaci da se promijenilo samo tijekom hoda
    private void Interact(Table table)
    {
        // NE RADI BEZ OVOG
        if (!HasWorkAt(table))   // stanje se moglo promijeniti dok sam hodala
        {
            return;
        }

        switch (table.State) 
        {
            case TableState.Seated:
                TakeOrder(table);
                break;

            case TableState.Dirty:
                table.SetState(TableState.Free);
                break;

            // PRIVREMENO ORDERED
            case TableState.Ordered:
                table.SetState(TableState.Dirty);
                break;
        }
    }

    // JOS OVO 
    private void TakeOrder(Table table)
    {
        Customer customer = FindCustomerAt(table);
        if (customer == null)
        {
            return;
        }

        Order order = customer.PlaceOrder();
        if (order != null) 
        {
            OrderManager.instance.AddOrder(order);
        }
    }

    private Customer FindCustomerAt(Table table)
    {
        Customer[] all = FindObjectsByType<Customer>(FindObjectsSortMode.None);
        foreach (Customer c in all) 
        {
            if (c.AssignedTable == table)
            {
                return c;
            }
        }
        return null;
    }
}
