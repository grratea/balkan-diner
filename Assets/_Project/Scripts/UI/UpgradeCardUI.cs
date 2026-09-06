using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Button buyButton;

    private UpgradeSO upgrade;
    // bolje nego konstruktor
    public void Setup(UpgradeSO upgrade)
    {
        this.upgrade = upgrade;
        buyButton.onClick.AddListener(OnBuyClicked);
        Refresh();
    }

    private void OnDestroy()
    {
        if (buyButton  != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
        }
    }

    private void OnBuyClicked()
    {
        UpgradeManager.instance.TryPurchase(upgrade);   
    }

    public void Refresh()
    {
        if (upgrade == null)
        {
            return;
        }

        nameLabel.text = upgrade.displayName;
        descriptionLabel.text = upgrade.description;

        int level = UpgradeManager.instance.GetLevel(upgrade);
        levelLabel.text = $"Level {level} / {upgrade.MaxLevel}";

        if (UpgradeManager.instance.IsMaxLevel(upgrade))
        {
            costLabel.text = "MAX";
            // puno bolje nego da sakrijem button
            // jer igrac ona zna da ne moze kupiti jer nema para
            buyButton.interactable = false;
            return;
        }

        int cost = UpgradeManager.instance.GetNextCost(upgrade);
        costLabel.text = $"{cost} €";
        buyButton.interactable = UpgradeManager.instance.CanAfford(upgrade);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
