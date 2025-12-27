using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/NPC Has Trait")]
public class NPCHasTraitCondition : DialogueCondition
{
    public NPCTrait trait;
    public bool mustHaveTrait = true;

    public override bool IsMet()
    {
        if (DialogueController.Instance == null)
            return false;

        NPCController npc = DialogueController.Instance.CurrentSpeaker;
        if (npc == null)
            return false;

        bool hasTrait = npc.HasTrait(trait);
        return mustHaveTrait ? hasTrait : !hasTrait;
    }
}

