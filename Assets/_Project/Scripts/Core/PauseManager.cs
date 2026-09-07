using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mainMenuPanel;

    private bool isPaused;

    public bool IsPaused {  get { return isPaused; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        if (pausePanel != null) 
        { 
            // jer zas bi bio upaljen, znaci ugasi ga
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (DayManager.instance != null && DayManager.instance.State != DayState.Playing)
        {
            return;
        }
        // ako je vec pauzirano
        if (isPaused)
        {
            Resume();
        }    
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    public void QuitToMenu()
    {
        Resume(); // da se makne panel i nije vise isPaused
        Time.timeScale = 0f; // ali se onda zaustavlja igra

        if (DayManager.instance != null)
        {
            DayManager.instance.StopGame();
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }
}
