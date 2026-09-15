using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Mover))] 
public class Customer : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] private TextMeshPro debugLabel;
    [SerializeField] private CustomerState state = CustomerState.Entering;
    [SerializeField] private bool showDebugLabel = false;

    [Header("ORDER")]
    [SerializeField] private MenuSO menu;

    [Header("EATING")]
    [SerializeField] private float eatDuration = 4f;


    [Header("PATIENCE")]
    [SerializeField] private float maxPatience = 25f;
    [SerializeField] private Transform patienceBarRoot;
    [SerializeField] private SpriteRenderer patienceBarFill;

    [Header("VISUAL")]
    [SerializeField] private SpriteRenderer visualRenderer;
    [SerializeField] private Sprite[] customerSprites;
    [SerializeField] private SpriteRenderer orderIcon;

    private float patience;
    private bool patienceActive;

    private Coroutine eatingRoutine;

    private Mover mover;
    private Table assignedTable;
    private Vector2 exitPosition;

    private DishSO wantedDish;
    private Order currentOrder;

    public CustomerState State { get { return state; } }
    public Table AssignedTable { get { return assignedTable; } }
    public DishSO WantedDish { get { return wantedDish; } }

    private void Awake()
    {
        mover = GetComponent<Mover>();
        PickRandomLook();
    }


    public void Init(Vector2 exit)
    {
        exitPosition = exit;
    }

    private void Start()
    {
        maxPatience = GameConfig.Balance.customerPatience;
        eatDuration = GameConfig.Balance.customerEatDuration;
        menu = GameConfig.Menu;

        RefreshLabel();
        StopPatience(); // skriven jer se akt tek kada sjedne
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

    // PROVJERI
    private void OnArrivedAtTable()
    {
        SetState(CustomerState.Seated);
        assignedTable.SetState(TableState.Seated);
        assignedTable.AssignCustomer(this);

        // SUBSCRIBE
        assignedTable.OnStateChanged += HandleTableStateChanged;


        wantedDish = menu.GetRandomDish();
        RefreshLabel();

        ShowOrderIcon(wantedDish);
        StartPatience();

    }

    private void StartPatience()
    {
        patience = maxPatience;
        patienceActive = true;

        if (patienceBarRoot != null)
        {
            patienceBarRoot.gameObject.SetActive(true); // prikazi
        }

        RefreshPatienceBar();
    }

    private void RefreshPatienceBar()
    {
        if (patienceBarRoot == null || patienceBarFill == null)
        {
            return;
        }

        float ratio = Mathf.Clamp01(patience / maxPatience);

        Vector3 scale = patienceBarRoot.localScale;
        scale.x = ratio;
        patienceBarRoot.localScale = scale;

        if (ratio > 0.5f)
        {
            patienceBarFill.color = Color.green;
        }
        else if (ratio > 0.25f)
        {
            patienceBarFill.color = Color.yellow;
        }
        else
        {
            patienceBarFill.color = Color.red;
        }

    }

    private void StopPatience()
    {
        patienceActive = false;

        if (patienceBarRoot != null)
        {
            patienceBarRoot.gameObject.SetActive(false); // sakrij
        }
    }

    public Order PlaceOrder()
    {
        if (state != CustomerState.Seated || wantedDish == null)
        {
            return null;
        }

        // SPRIJECAVA DVOSTRUKU NARUDZBU, ocekuje se da bude null jer nikad dosad nije koristen
        // ako je koristen, onda NIJE NULL
        if (currentOrder != null)
        {
            return null;
        }

        currentOrder = new Order(wantedDish, assignedTable, this);
        assignedTable.SetState(TableState.Ordered);
        RefreshLabel();

        return currentOrder;    
    }

    private void HandleTableStateChanged(Table table, TableState newState)
    {
        // tko mijenja stanje stola, gost sam kad ode nakon sto pojede i plati
        // UBITI SAMO TESTIRANJE
        if (newState == TableState.Dirty && state != CustomerState.Leaving) 
        {
            Leave();
        }
    }

    public void Leave()
    {
        HideOrderIcon();
        StopPatience();
        StopEating();
        Unsubscribe();
        if (assignedTable != null)
        {
            assignedTable.ClearCustomer();
        }

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
        if (!patienceActive)
        {
            return;
        }

        patience -= Time.deltaTime;
        RefreshPatienceBar();

        if (patience <= 0f)
        {
            patience = 0f;
            LeaveAngry();
        }
    }

    private void LeaveAngry()
    {
        StopPatience();

        // otkine se narudzba i zasteka stednjak
        if (currentOrder != null)
        {
            OrderManager.instance.CancelOrder(currentOrder);
            currentOrder = null;
        }

        if (assignedTable != null)
        {
            assignedTable.SetState(TableState.Dirty); // samo bez novaca
        }

        Leave();
    }


    private void RefreshLabel()
    {
        if (debugLabel == null)
        {
            return;
        }

        if (!showDebugLabel)
        {
            debugLabel.gameObject.SetActive(false);
            return;
        }
        debugLabel.gameObject.SetActive(true);

        string text = state.ToString();

        if (state == CustomerState.Seated && wantedDish != null) 
        {
            if (currentOrder == null)
            {
                text = $"{wantedDish.displayName}?";
            }
            else
            {
                text = $"{wantedDish.displayName}... ";
            }
        }

        debugLabel.text = text;
        debugLabel.color = state switch
        {
            CustomerState.Entering => Color.white,
            CustomerState.InQueue => Color.grey,
            CustomerState.WalkingToTable => Color.yellow,
            CustomerState.Seated => Color.green,
            CustomerState.Eating => Color.cyan,
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


    public void ReceiveDish(Order order)
    {
        if (state != CustomerState.Seated)
        {
            return;
        }

        HideOrderIcon();
        StopPatience(); // buduci da je dobio hranu, NE CEKA VISE


        SetState(CustomerState.Eating);
        eatingRoutine = StartCoroutine(EatRoutine(order));
    }

    private IEnumerator EatRoutine(Order order)
    {
        float timer = eatDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            RefreshEatingLabel(timer);
            yield return null; // ide na sljedeci frame
        }
        FinishEating(order);
    }

    private void FinishEating(Order order)
    {
        eatingRoutine = null;
        int payment = order.Dish.price;
        assignedTable.LeavePayment(payment);
        assignedTable.SetState(TableState.Dirty);

        Leave();
    }

    // kada Customer postane nestrpljiv i onda ustane usred jela
    private void StopEating()
    {
        if (eatingRoutine != null)
        {
            StopCoroutine(eatingRoutine);
            eatingRoutine = null;
        }
    }
    private void RefreshEatingLabel(float timeLeft)
    {
        if (debugLabel == null)
        {
            return;
        }
        debugLabel.text = $"eating... {timeLeft:F1}s";
        debugLabel.color = Color.cyan;
    }

    private void PickRandomLook()
    {
        if (visualRenderer == null || customerSprites == null || customerSprites.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, customerSprites.Length);
        visualRenderer.sprite = customerSprites[index];

    }

    private void ShowOrderIcon(DishSO dish)
    {
        if (orderIcon == null)
        {
            return;
        }

        // null sprite = renderer ne crta nista
        orderIcon.sprite = (dish != null) ? dish.icon : null;
    }

    private void HideOrderIcon()
    {
        if (orderIcon != null)
        {
            orderIcon.sprite = null;
        }
    }

}
