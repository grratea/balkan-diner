using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Mover))] 
public class Customer : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] private TextMeshPro debugLabel;
    [SerializeField] private CustomerState state = CustomerState.Entering;

    private Mover mover;
    private Table assignedTable;
    private Vector2 exitPosition;

    private void Awake()
    {
        mover = GetComponent<Mover>(); 
    }

    public void Init(Vector2 exit)
    {
        exitPosition = exit;
    }

    void Start()
    {
        RefreshLabel();
        TryFindSpot();
    }

    private void TryFindSpot()
    {
        Table table = TableManager.instance.GetFreeTable();
        if (table != null)
        {
            GoToTable(table);
        }
        else
        {
            // TO DO !!!!!!!!!!!!!!!!!!!!!!
            // RED CEKANJA QueueManager.TryJoin
            Leave();
        }
    }

    // public zbog eventualnog QueueManagera
    public void GoToTable(Table table)
    {
        assignedTable = table;

        table.SetState(TableState.Reserved);

        SetState(CustomerState.WalkingToTable);
        mover.MoveTo(table.SeatPosition, OnArrivedAtTable); // callback? ubiti lanac
    }

    private void OnArrivedAtTable()
    {
        SetState(CustomerState.Seated);
        assignedTable.SetState(TableState.Seated);

        // SUBSCRIBE
        assignedTable.OnStateChanged += HandleTableStateChanged;
    }

    private void HandleTableStateChanged(Table table, TableState newState)
    {
        // tko mijenja stanje stola, gost sam kad ode nakon sto pojede i plati
        // UBITI SAMO TESTIRANJE
        if (newState == TableState.Dirty) 
        {
            Leave();
        }
    }

    public void Leave()
    {
        Unsubscribe();

        assignedTable = null;
        SetState(CustomerState.Leaving);

        mover.MoveTo(exitPosition, () => Destroy(gameObject));
    }

    // poziv moze biti dvaput
    private void Unsubscribe()
    {
        if (assignedTable != null) 
        {
            assignedTable.OnStateChanged -= HandleTableStateChanged;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    void Update()
    {
        
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
            CustomerState.Entering => Color.white,
            CustomerState.InQueue => Color.grey,
            CustomerState.WalkingToTable => Color.yellow,
            CustomerState.Seated => Color.green,
            CustomerState.Leaving => Color.red,
            _ => Color.white,
        };
    }

    private void SetState(CustomerState newState)
    {
        if (state == newState)
        {
            return;
        }
        state = newState;
        RefreshLabel();
    }
}
