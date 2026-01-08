using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    private HashSet<string> talkedToNPCs = new();
    private HashSet<string> usedDialogueOptions = new();
    private HashSet<string> dialogueFlags = new HashSet<string>();

    private static readonly HashSet<string> dailyResetFlags = new()
    {
        "Will_Sacrifice"
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void MarkNPCTalked(string npcId)
    {
        talkedToNPCs.Add(npcId);
    }

    public bool HasTalkedToNPC(string npcId)
    {
        return talkedToNPCs.Contains(npcId);
    }

    string MakeOptionKey(string npcId, string optionId, int day)
    {
        return $"{npcId}|{optionId}|{day}";
    }

    public bool HasUsedDialogueOption(string npcId, string optionId)
    {
        int day = GameManager.Instance.currentDay;
        return usedDialogueOptions.Contains(MakeOptionKey(npcId, optionId, day));
    }

    public void MarkDialogueOptionUsed(string npcId, string optionId)
    {
        int day = GameManager.Instance.currentDay;
        usedDialogueOptions.Add(MakeOptionKey(npcId, optionId, day));
    }

    public void ResetDailyData()
    {
        talkedToNPCs.Clear();
    
        foreach (var flag in dailyResetFlags)
        {
            dialogueFlags.Remove(flag);
        }
        foreach (var flag in dialogueFlags)
        {
            Debug.Log(flag);
        }
    }

    public bool IsDialogueFlagSet(string flagName)
    {
        return dialogueFlags.Contains(flagName);
    }
    
    public void SetDialogueFlag(string flagName)
    {
        if (!string.IsNullOrEmpty(flagName))
            dialogueFlags.Add(flagName);
    }
    
    public void ClearDialogueFlag(string flagName)
    {
        dialogueFlags.Remove(flagName);
    }

    public void ResetAllData()
    {
        talkedToNPCs.Clear();
        usedDialogueOptions.Clear();
        dialogueFlags.Clear();
    }

}