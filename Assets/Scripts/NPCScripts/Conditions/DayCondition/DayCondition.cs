using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/Day Condition")]
public class DayCondition : DialogueCondition
{
    public enum DayComparison
    {
        Equal,
        GreaterThan,
        GreaterOrEqual,
        LessThan,
        LessOrEqual,
        Between
    }

    public DayComparison comparison;

    public int day;         
    // used for Between
    public int dayMin;       
    public int dayMax;

    public override bool IsMet()
    {
        if (GameManager.Instance == null)
            return false;

        int currentDay = GameManager.Instance.currentDay;

        switch (comparison)
        {
            case DayComparison.Equal:
                return currentDay == day;

            case DayComparison.GreaterThan:
                return currentDay > day;

            case DayComparison.GreaterOrEqual:
                return currentDay >= day;

            case DayComparison.LessThan:
                return currentDay < day;

            case DayComparison.LessOrEqual:
                return currentDay <= day;

            case DayComparison.Between:
                return currentDay >= dayMin && currentDay <= dayMax;
        }

        return false;
    }
}

