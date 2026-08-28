using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager-a nema u sceni", this);
            return;
        }
        GameManager.instance.OnMoneyChanged += HandleMoneyChanged;
        Refresh();
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnMoneyChanged -= HandleMoneyChanged;
        }
    }

    private void HandleMoneyChanged(int amount)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (label == null)
        {
            return;
        }
        label.text = $"{GameManager.instance.Money} €";
    }

    void Update()
    {
        
    }
}
