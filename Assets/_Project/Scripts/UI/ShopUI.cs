using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer; // roditelj
    [SerializeField] private UpgradeCardUI cardPrefab;
    [SerializeField] private TextMeshProUGUI moneyLabel;
    [SerializeField] private Button continueButton;

    private List<UpgradeCardUI> cards = new List<UpgradeCardUI>();

    void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        UpgradeManager.instance.OnUpgradePurchased += HandlePurchase;

        BuildCards();
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        // sve obrnuto od Starta
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
        }

        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.OnUpgradePurchased -= HandlePurchase;
        }
    }

    // samo jednom se poziva
    private void BuildCards()
    {
        foreach(UpgradeSO u in UpgradeManager.instance.AvailableUpgrades)
        {
            UpgradeCardUI card = Instantiate(cardPrefab, cardContainer);
            card.Setup(u);
            cards.Add(card);
        }
    }

    public void Open()
    {
        panel.SetActive(true);
        RefreshAll();
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void HandlePurchase(UpgradeSO upgrade, int newLevel)
    {
        RefreshAll();
    }

    // prikazuje sve kartice odjednom
    private void RefreshAll()
    {
        foreach(UpgradeCardUI card in cards) 
        {
            card.Refresh();
        }
        if (moneyLabel != null)
        {
            moneyLabel.text = $"{GameManager.instance.Money} €";
        }
    }

    private void OnContinueClicked()
    {
        Close();
        DayManager.instance.NextDay();
    }

    void Update()
    {
        
    }
}
