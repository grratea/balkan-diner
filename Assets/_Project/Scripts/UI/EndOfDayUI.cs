using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndOfDayUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI resultLabel;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonLabel;
    [SerializeField] private ShopUI shopUI;

    private bool lastDaySuccess;

    private void Start()
    {
        if (DayManager.instance == null)
        {
            Debug.LogError("DayManager nije u sceni", this);
            return;
        }

        DayManager.instance.OnDayEnded += HandleDayEnded;
        actionButton.onClick.AddListener(OnActionClicked);

        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (DayManager.instance != null)
        {
            DayManager.instance.OnDayEnded -= HandleDayEnded;
        }
        if (actionButton != null)
        {
            actionButton.onClick.RemoveListener(OnActionClicked);
        }
    }

    private void HandleDayEnded(bool success, int earned, int goal)
    {
        panel.SetActive(true);
        lastDaySuccess = success;

        if (success)
        {
            titleLabel.text = "DAY FINISHED!";
            titleLabel.color = Color.green;
            actionButtonLabel.text = "CONTINUE";
        }
        else
        {
            titleLabel.text = "YOU DID NOT MEET THE GOAL";
            titleLabel.color = Color.red;
            actionButtonLabel.text = "TRY AGAIN";
        }

        resultLabel.text = $"Earnings: {earned} €\nGoal: {goal} €";
    }

    private void OnActionClicked()
    {
        panel.SetActive(false);

        if (lastDaySuccess)
        {
            // on samo zapocne sljedeci dan
            // ima event kada se stigne continue
            // on pozove fje za sljedeci dan
            shopUI.Open();
        }
        else
        {
            DayManager.instance.RetryDay();
        }
    }
}