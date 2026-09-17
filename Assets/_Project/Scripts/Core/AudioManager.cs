using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {  get; private set; }
    
    [Header("SOURCES")]
    [SerializeField] private AudioSource sfxSource; // efekti
    [SerializeField] private AudioSource musicSource; // glazba

    [Header("VOLUME")]
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.05f;

    [Header("EFFECTS")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip doorBellClip;
    [SerializeField] private AudioClip dingClip;
    [SerializeField] private AudioClip cashClip;
    [SerializeField] private AudioClip angryClip;
    [SerializeField] private AudioClip purchaseClip;
    [SerializeField] private AudioClip daySuccessClip;
    [SerializeField] private AudioClip dayFailClip;

    [Header("MUSIC")]
    [SerializeField] private AudioClip musicClip;

    public void PlayClick() { PlayWithPitchVariation(clickClip, 0.1f); }
    public void PlayDoorBell() { PlaySFX(doorBellClip, 0.2f); }
    public void PlayDing() { PlaySFX(dingClip); }
    public void PlayCash() { PlayWithPitchVariation(cashClip, 0.08f); }
    public void PlayAngry() { PlaySFX(angryClip); }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }
        // da sam stavila Play - nesto bi moglo prekinuti zvuk
        sfxSource.PlayOneShot(clip, volumeScale * sfxVolume);
    }

    // varijacija visine tona jer isti zvuk postane dosadan
    // to rade prof igre
    private void PlayWithPitchVariation(AudioClip clip, float variation)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        float originalPitch = sfxSource.pitch;
        sfxSource.pitch = 1f + Random.Range(-variation, variation);
        sfxSource.PlayOneShot(clip, sfxVolume);
        sfxSource.pitch = originalPitch;
    }

    void Start()
    {
        SubscribeToEvents();
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (musicClip == null || musicSource == null)
        {
            return;
        }

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        // globalna var
        AudioListener.pause = false;
    }


    // primjer OCP-a
    private void SubscribeToEvents()
    {
        // AKO POSTOJE 
        if (GameManager.instance != null)
        {
            // na event prikaci svoju metodu
            GameManager.instance.OnMoneyChanged += HandleMoneyChanged;
        }

        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.OnUpgradePurchased += HandleUpgradePurchased;
        }

        if (DayManager.instance != null)
        {
            DayManager.instance.OnDayEnded += HandleDayEnded;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnMoneyChanged -= HandleMoneyChanged;
        }
        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.OnUpgradePurchased -= HandleUpgradePurchased;
        }
        if (DayManager.instance != null)
        {
            DayManager.instance.OnDayEnded -= HandleDayEnded;
        }
    }

    private void HandleMoneyChanged(int amount)
    {
        // pozitivan iznos - zarada, negativan - kupnja
        if (amount > 0)
        {
            PlayCash();
        }
    }

    private void HandleUpgradePurchased(UpgradeSO upgrade, int newLevel)
    {
        PlaySFX(purchaseClip);
    }

    private void HandleDayEnded(bool success, int earned, int goal)
    {
        PlaySFX(success ? daySuccessClip : dayFailClip);
    }

    void Update()
    {
        
    }
}
