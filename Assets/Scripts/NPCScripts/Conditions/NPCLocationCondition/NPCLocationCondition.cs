using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/NPC At Location")]
public class NPCLocationCondition : DialogueCondition
{
    public string npcId;
    public NPCLocation location;
    public bool mustBeAtLocation = true;

    public override bool IsMet()
    {
        if (GameManager.Instance == null)
            return false;

        NPCController npc = GameManager.Instance.GetNPCById(npcId);
        if (npc == null)
            return false;

        bool isAtLocation = npc.currentLocation == location;
        return mustBeAtLocation ? isAtLocation : !isAtLocation;
    }
}
