using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {  get; private set; }

    [SerializeField] private int money = 0;

    public int Money { get { return money; } }

    // dodano za UI, kad se kasnije implementira
    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            return;
        }
        this.money += amount;
        OnMoneyChanged?.Invoke(amount);
    }

    // TO DO !!!
    // potrosi novac

    void Start()
    {

    }

    void Update()
    {
        
    }
}
