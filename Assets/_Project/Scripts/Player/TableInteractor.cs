using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Mover))]
[RequireComponent(typeof(CarryController))]
// MOZDA PROMIJENITI IME KLASE U INTERACTOR, LOGIKA
public class TableInteractor : MonoBehaviour
{
    [SerializeField] private LayerMask interactableMask;
    private Camera cam;
    private Mover mover;
    private CarryController carry;

    private void Awake()
    {
        cam = Camera.main;
        mover = GetComponent<Mover>();
        carry = GetComponent<CarryController>();
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

        Collider2D hit = GetColliderUnderMouse();
        if (hit == null)
        {
            return;
        }
        // ako je hit izmedu stola i klika, idi do stola
        if (hit.TryGetComponent(out Table table))
        {
            TryGoToTable(table);
        }
        // ako je hit izmedu stovea i klika, idi do stovea
        else if (hit.TryGetComponent(out Stove stove))
        {
            TryGoToStove(stove);
        }

        // dohvacanje stola na koji je player kliknuo
        //Table table = GetTableUnderMouse(); 
        //if (table == null)
        //{
        //    return;
        //}

        //if (!HasWorkAt(table))
        //{
        //    return;
        //}

        //mover.MoveTo(table.ServePosition, () => Interact(table));
    }

    private void TryGoToTable(Table table)
    {
        // nemoj ici ako nije u stanju za konobara
        if (!HasWorkAtTable(table))
        {
            return;
        }

        mover.MoveTo(table.ServePosition, () => InteractWithTable(table));
    }

    // ne samo je li ima posla, nego sto ima i u ruci
    private bool HasWorkAtTable(Table table)
    {
        // za svaku dopustenu radnju nad ODREDENIM STANJIMA STOLA ima uvjet
        switch (table.State) 
        { 
            case TableState.Seated:
                return carry.IsEmpty && StoveManager.instance.HasEmptyStove; // player nema nista na tanjuru kad prima narudzbu

            case TableState.Ordered:
                return CanServeTable(table);

            case TableState.Dirty:
                return carry.IsEmpty; // cisti stol praznih ruku

            default:
                return false;
        }
    }

    private bool CanServeTable(Table table)
    {
        // moze posluziti ako nosi hranu i ako je ta hrana spremna ofc i ako je to taj stol
        return carry.IsCarrying && carry.CarriedOrder.IsReady && carry.CarriedOrder.Table == table;
    }

    private void InteractWithTable(Table table)
    {
        if (!HasWorkAtTable(table))
        {
            return;
        }

        switch (table.State) 
        {
            case TableState.Seated:
                TakeOrder(table);
                break;
            case TableState.Ordered:
                ServeDish(table);
                break;
            case TableState.Dirty:
                table.SetState(TableState.Free);
                break;
        }
    }

    private void ServeDish(Table table)
    {
        Order order = carry.Drop();

        table.SetState(TableState.Served);
        OrderManager.instance.RemoveOrder(order);

        // NEDOSTAJE TIMER ZA JEDENJE I PLACANJE
        table.SetState(TableState.Dirty);
    }

    private void TryGoToStove(Stove stove)
    {
        if (!HasWorkAtStove(stove))
        {
            return;
        }
        mover.MoveTo(stove.InteractPosition, () => InteractWithStove(stove));
    }

    private bool HasWorkAtStove(Stove stove)
    {
        // da, ako je stove prazan i ako player nesto nosi i ako hrana nije gotova)
        // STAVLJAM SIROVU NARUDBU NA PRAZAN STEDNJAK ?!
        if (stove.IsEmpty && carry.IsCarrying && !carry.CarriedOrder.IsReady)
        {
            return true;
        }

        // da, ako je hrana spremna i player nema nista u rukama
        if (stove.IsReady && carry.IsEmpty)
        {
            return true;
        }

        return false;
    }

    private void InteractWithStove(Stove stove)
    {
        if (!HasWorkAtStove(stove))
        {
            return;
        }
        // ako je stove prazan, kreni kuhat
        if (stove.IsEmpty)
        {
            Order order = carry.Drop();
            bool started = stove.StartCooking(order);

            // ZBOG IZGUBLJENOG RESURSA
            // npr. ako je stednjak pun i ne moze kuhat i ovim se izbjegava da se izgubi narudzba
            if (!started)
            {
                carry.PickUp(order);
            }
        }
        // ako nije prazan, uzmi hranu
        else if (stove.IsReady)
        {
            Order order = stove.TakeDish();
            carry.PickUp(order);
        }
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
        if (order == null) 
        {
            return;
        }

        OrderManager.instance.AddOrder(order);
        carry.PickUp(order); // player uzima narudzbu osigurava se da ne moze uzeti dvije narudzbe odjednom
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
    
    private Collider2D GetColliderUnderMouse()
    {
        Vector2 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        return Physics2D.OverlapPoint(world, interactableMask);
    }
}
