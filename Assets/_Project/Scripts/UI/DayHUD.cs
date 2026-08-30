using TMPro;
using UnityEngine;

public class DayHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayLabel;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI goalLabel;

    void Start()
    {
        
    }
    void Update()
    {
        if (DayManager.instance == null)
        {
            return;
        }

        if (dayLabel != null)
        {
            dayLabel.text = $"DAY {DayManager.instance.CurrentDay}";
        }

        if (timeLabel != null)
        {
            float t = DayManager.instance.TimeLeft;
            int min = Mathf.FloorToInt(t / 60f);
            int sec = Mathf.FloorToInt(t % 60f);
            timeLabel.text = $"{min}:{sec:00}"; 
        }

        if (goalLabel != null)
        {
            goalLabel.text = $"GOAL: {DayManager.instance.EarnedToday} / {DayManager.instance.Goal}";
        }
    }
}
