using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/NPC Is Alive")]
public class NPCAliveCondition : DialogueCondition
{
    public string npcId;

    public override bool IsMet()
    {
        return GameManager.Instance != null && GameManager.Instance.IsNPCAlive(npcId);
    }
}