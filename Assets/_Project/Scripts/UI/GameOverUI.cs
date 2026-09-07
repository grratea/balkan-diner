using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI resultLabel;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitButton;
 
    void Start()
    {
        if (DayManager.instance == null)
        {
            Debug.LogError("DayManager ne postoji", this);
            return;
        }

        DayManager.instance.OnGameWon += HandleGameWon;

        playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        quitButton.onClick.AddListener(MainMenuUI.QuitGame);

        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (DayManager.instance != null)
        {
            DayManager.instance.OnGameWon -= HandleGameWon;
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
        }   

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(MainMenuUI.QuitGame);
        }
    }

    private void HandleGameWon()
    {
        panel.SetActive(true);

        titleLabel.text = "CONGRATS";
        titleLabel.color = Color.yellow;

        resultLabel.text = $"Total earned: {GameManager.instance.Money} €";
    }

    private void OnPlayAgainClicked()
    {
        panel.SetActive(false);
        DayManager.instance.StartNewGame();
    }

    void Update()
    {
        
    }
}
