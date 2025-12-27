using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/Talked To NPC")]
public class TalkedToNPCCondition : DialogueCondition
{
    public string npcId;

    public override bool IsMet()
    {
        return GameState.Instance != null && GameState.Instance.HasTalkedToNPC(npcId);
    }
}

