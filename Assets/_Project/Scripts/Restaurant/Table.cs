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

        debugLabel.text = state.ToString(); 
        debugLabel.color = state switch
        {
            TableState.Free => Color.green,
            TableState.Seated => Color.yellow,
            TableState.Ordered => new Color(1f, 0.5f, 0f),
            TableState.Served => Color.cyan,
            TableState.Dirty => Color.red,
            _ => Color.white,                         // default
        };
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
