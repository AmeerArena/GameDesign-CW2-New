using UnityEngine;
using TMPro;

public class DayCounter : MonoBehaviour
{
    private TMP_Text text;   // works for TextMeshProUGUI and TextMeshPro

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        if (text == null)
        {
            Debug.LogError("DayCounter: No TMP_Text found on this GameObject.");
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.dayCounter = this;
            UpdateDayText(GameManager.Instance.currentDay);
        }
        else
        {
            Debug.LogWarning("DayCounter: No GameManager instance found in scene.");
        }
    }

    public void UpdateDayText(int day)
    {
        if (text == null)
        {
            Debug.LogError("DayCounter: text reference is null, cannot update day text.");
            return;
        }

        text.text = "Day " + day;
    }
}
