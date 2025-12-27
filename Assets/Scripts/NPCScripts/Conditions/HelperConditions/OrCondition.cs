using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/OR")]
public class OrCondition : DialogueCondition
{
    public DialogueCondition[] conditions;

    public override bool IsMet()
    {
        if (conditions == null || conditions.Length == 0)
            return false;

        foreach (var condition in conditions)
        {
            if (condition != null && condition.IsMet())
                return true;
        }

        return false;
    }
}

