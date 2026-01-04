using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public DialogueLine[] dialogueLines;
    public float textSpeed = 0.01f;

    public DialogueChoice[] choices;
}


[System.Serializable]
public class DialogueLine
{
    [TextArea]
    public string text;

    public AudioClip voiceClip;

    public DialogueCondition[] conditions; // optional
    public bool endDialogue;

    [Header("Flow Control")]
    public bool overrideNext;
    public int nextDialogueIndex;
}


[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public DialogueOption[] options;
}


[System.Serializable]
public class DialogueOption
{
    public string optionId;
    public string text;
    public int nextDialogueIndex;

    public DialogueCondition[] conditions;

    [Header("Action")]
    public DialogueActionType actionType;
    public ResourceData resource;
    public int amount;

    [Header("Option Rules")]
    public bool oncePerDay;

    [Header("NPC Effects")]
    public bool food;
    public bool wood;
    public NPCLocation moveTargetLocation; // used for the kid to set his location
    
    [Header("Flag Effects")]
    public string setFlag;
}

public enum DialogueActionType
{
    None,
    AddResource,
    ConsumeResource,
    MoveNPC,
    KillFarmer,
    KillKid,
    KillHuntress,
    KillLumberjack,
    KillMiner,
    Hunt
}
