using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        panel.SetActive(true); // on je prvi
    }

    private void OnDestroy()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }

    private void OnPlayClicked()
    {
        panel.SetActive(false);
        DayManager.instance.StartNewGame();
    }

    private void OnQuitClicked()
    {
        QuitGame();
    }
    // u editoru - gasi play mode, a u buildu gasi app
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    void Update()
    {
        
    }
}
