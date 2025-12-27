using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/NOT")]
public class NotCondition : DialogueCondition
{
    public DialogueCondition condition;

    public override bool IsMet()
    {
        return condition != null && !condition.IsMet();
    }
}

