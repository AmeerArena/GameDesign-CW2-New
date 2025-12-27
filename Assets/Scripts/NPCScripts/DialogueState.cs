using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueState
{
    public DialogueCondition[] conditions;
    public NPCDialogue dialogue;

    [Tooltip("Does Not go to the next dialogue if true")]
    public bool endAllDialogue = false;
}

