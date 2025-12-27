using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/AND")]
public class AndCondition : DialogueCondition
{
    public DialogueCondition[] conditions;

    public override bool IsMet()
    {
        if (conditions == null || conditions.Length == 0)
            return true;

        foreach (var condition in conditions)
        {
            if (condition == null || !condition.IsMet())
                return false;
        }

        return true;
    }
}

