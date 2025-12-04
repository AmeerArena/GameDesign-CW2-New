using UnityEngine;
using TMPro;

public class DayCounter : MonoBehaviour
{
    private TextMeshPro text;   // no public field anymore

    private void Awake()
    {
        // try to get the TextMeshPro on THIS object
        text = GetComponent<TextMeshPro>();

        if (text == null)
        {
            Debug.LogError("DayCounter: No TextMeshPro found on this GameObject.");
        }
    }

    private void Start()
    {
        // optional initial update if GameManager exists
        if (GameManager.Instance != null)
        {
            GameManager.Instance.dayCounter = this;
            UpdateDayText(GameManager.Instance.currentDay);
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
