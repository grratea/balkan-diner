using UnityEngine;
using UnityEngine.UI;

// Stavi na BILO KOJI gumb - sam nade Button komponentu i doda zvuk.
// Nema rucnog povezivanja u Inspectoru.
//
// Zasebna komponenta umjesto poziva u svakoj UI skripti:
// pet mjesta koja se moraju sjetiti vs jedna komponenta koju
// dodam i zaboravim. Isti obrazac kao YSorter iz Dana 12.
[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(PlaySound);
    }

    private void OnDestroy()
    {
        // za svaki AddListener mora postojati RemoveListener
        if (button != null)
        {
            button.onClick.RemoveListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayClick();
        }
    }
}