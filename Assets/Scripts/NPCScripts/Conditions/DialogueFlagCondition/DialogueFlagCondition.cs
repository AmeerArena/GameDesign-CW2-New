using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueFlagCondition", menuName = "Dialogue/Conditions/Flag Condition")]
public class DialogueFlagCondition : DialogueCondition
{
    [Tooltip("The unique flag name that this condition checks.")]
    public string flagName;

    public override bool IsMet()
    {
        if (string.IsNullOrEmpty(flagName))
            return true;

        return GameState.Instance.IsDialogueFlagSet(flagName);
    }
}

