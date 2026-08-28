using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Table : MonoBehaviour
{
    [Header("SETTINGS")]
    // transform.position se ne moze koristiti za oboje
    [SerializeField] private Transform seatPoint; // tocka za gosta 
    [SerializeField] private Transform servePoint; // tocka za konobara
    [SerializeField] private TextMeshPro debugLabel;

    [Header("ONLY FOR DEBUG")]
    [SerializeField] private TableState state = TableState.Free;

    private Customer currentCustomer;
    private int pendingPayment;

    public Customer CurrentCustomer { get {  return currentCustomer; } }
    public bool HasPayment { get { return pendingPayment > 0; } }
    public int PendingPayment { get { return pendingPayment; } }

    public TableState State { get { return state; } }

    public event Action<Table, TableState> OnStateChanged;
    // fja prima stol (da zna koji) i novo stanje, ne vraca nista

    public Vector2 SeatPosition
    {
        get
        {
            if (seatPoint != null)
            {
                return (Vector2)seatPoint.position;
            }
            return (Vector2)transform.position;
        }
    }

    public Vector2 ServePosition
    {
        get
        {
            if (servePoint != null)
            {
                return (Vector2)servePoint.position;
            }
            return (Vector2)transform.position;
        }
    }

    public bool IsFree
    {
        get { return state == TableState.Free; }
    }

    void Start()
    {
        RefreshLabel(); // da pokaze pocetno stanje odmah, inace prazno do prve promjene
    }

    public void SetState(TableState newState)
    {
        // da se ne ponovi zvuk ili efekt
        if (state == newState)
        {
            return;
        }

        this.state = newState;
        RefreshLabel(); // promjeni vizualno stol
        OnStateChanged?.Invoke(this, state); // okida na promjenu stanja te salje stol i state
    }
    
    private void RefreshLabel()
    {
        if (debugLabel == null)
        {
            return;
        }

        if (state == TableState.Dirty && pendingPayment > 0)
        {
            debugLabel.text = $"Dirty\n{pendingPayment} €";
            debugLabel.color = Color.yellow;
            return;
        }

        debugLabel.text = state.ToString(); 
        debugLabel.color = state switch
        {
            TableState.Free => Color.green,
            TableState.Seated => Color.yellow,
            TableState.Ordered => new Color(1f, 0.5f, 0f),
            TableState.Served => Color.cyan,
            TableState.Dirty => Color.red,
            TableState.Reserved => Color.grey,
            _ => Color.white,                         // default
        };
    }

    public void AssignCustomer(Customer customer)
    {
        currentCustomer = customer;
    }

    public void ClearCustomer()
    {
        currentCustomer = null;
    }

    // ZA GOSTA
    public void LeavePayment(int amount)
    {
        pendingPayment = amount;
        RefreshLabel();
    }

    // ZA PLAYERA
    // ubiti isti kao Stove.TakeDish da se ne pokupi isti novac dvaput
    public int CollectPayment()
    {
        int amount = pendingPayment;
        pendingPayment = 0;
        return amount;
    }

    // uvijek se crta u Scene View
    private void OnDrawGizmos()
    {
        if (seatPoint != null)
        {
            Gizmos.color = Color.coral;
            Gizmos.DrawWireSphere(seatPoint.position, 0.2f);
        }

        if (servePoint != null)
        {
            Gizmos.color = Color.cadetBlue;
            Gizmos.DrawWireSphere(servePoint.position, 0.2f);
        }
    }

    void Update()
    {
        
    }
}
