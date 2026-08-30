using UnityEngine;
using System;

public class DayManager : MonoBehaviour
{
    public static DayManager instance { get; private set; }

    [SerializeField] private float dayDuration = 120f;
    [SerializeField] private int baseGoal = 200;
    [SerializeField] private int goalIncrement = 100; // jedino nez je li mi to treba

    private int currentDay = 1;
    private float timeLeft;
    private int earnedToday;
    private DayState state = DayState.Playing;

    public int CurrentDay { get { return currentDay; } }
    public float TimeLeft { get { return timeLeft; } }
    public int EarnedToday { get { return earnedToday; } }
    public DayState State { get { return state; } }

    public event Action<bool, int, int> OnDayEnded;
    public event Action<int, int> OnDayStarted;

    public int Goal
    {
        get { return baseGoal + (currentDay - 1) * goalIncrement; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnMoneyChanged -= HandleMoneyChanged;
        }
        Time.timeScale = 1f;
    }


    void Start()
    {
        GameManager.instance.OnMoneyChanged += HandleMoneyChanged;
        StartDay();
    }


    private void StartDay()
    {
        ResetWorld();

        earnedToday = 0;
        timeLeft = dayDuration;
        state = DayState.Playing;
        Time.timeScale = 1f; // KRECE SE

        OnDayStarted?.Invoke(currentDay, Goal);
    }

    private void ResetWorld()
    {
        Customer[] customers = FindObjectsByType<Customer>(FindObjectsSortMode.None);
        foreach(Customer c in customers)
        {
            Destroy(c.gameObject);
        }
        // on ih poziva
        // ne radi sam on jer ne zna stanja
        TableManager.instance.ResetForNewDay();
        OrderManager.instance.ResetForNewDay();
        StoveManager.instance.ResetForNewDay();

        CarryController carry = FindFirstObjectByType<CarryController>();
        if (carry != null)
        {
            carry.ForceDrop();
        }
    }

    private void HandleMoneyChanged(int amount)
    {
        earnedToday += amount; // za svaki gost
    }

    void Update()
    {
        if (state != DayState.Playing)
        {
            return;
        }

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f) 
        {
            timeLeft = 0f;
            EndDay();
        }
    }

    private void EndDay()
    {
        state = DayState.Ended;
        Time.timeScale = 0f; // stane igra, ali ne zaustavi Update
        // globalna var

        bool success = (earnedToday >= Goal);
        OnDayEnded?.Invoke(success, earnedToday, Goal);
    }

    public void NextDay()
    {
        currentDay++;
        StartDay();
    }

    public void RetryDay()
    {
        StartDay(); // isti cilj
    }
}
