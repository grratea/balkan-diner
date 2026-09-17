using UnityEngine;
using System;

public class DayManager : MonoBehaviour
{
    public static DayManager instance { get; private set; }

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
    public event Action OnGameWon;
    public int Goal
    {
        get { return GameConfig.Balance.GetGoalForDay(currentDay); }
    }

    public bool IsLastDay
    {
        get { return currentDay >= GameConfig.Balance.totalDays; }
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

        AudioListener.pause = false;
    }


    void Start()
    {
        GameManager.instance.OnMoneyChanged += HandleMoneyChanged;
        // StartDay();
        // sad se ceka da igrac pokrene 
        state = DayState.Ended;
        Time.timeScale = 0f;
    }


    private void StartDay()
    {
        ResetWorld();

        earnedToday = 0;
        timeLeft = GameConfig.Balance.dayDuration;
        state = DayState.Playing;
        Time.timeScale = 1f;

        AudioListener.pause = false;

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

        // TU POZVATI
        TableManager.instance.ApplyUpgrades();
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
        Time.timeScale = 0f;

        //AudioListener.pause = true;

        bool success = (earnedToday >= Goal);
        // bool isLastDay = (currentDay >= GameConfig.Balance.totalDays);

        if (success && IsLastDay)
        {
            OnGameWon?.Invoke();
            return; // da se ne okine ovaj event dolje
        }

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

    public void StartNewGame()
    {
        currentDay = 1; // dan
        GameManager.instance.ResetMoney(); // novac
        UpgradeManager.instance.ResetAll(); // upgrade

        // gosti, stol, stednjak itd i zapocne novi dan
        StartDay(); 
    }

    public void StopGame()
    {
        state = DayState.Ended;
        Time.timeScale = 0f;
        ResetWorld();
    }
}
