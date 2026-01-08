using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : MonoBehaviour, IInteractable
{
    public string npcId;
    public Sprite portrait;

    [Header("Collector laugh")]
    [SerializeField] private AudioClip collectorVoice;

    [Header("Typing SFX")]
    [SerializeField] private AudioClip typingSound;
    [SerializeField, Range(0f, 1f)] private float typingVolume = 0.5f;
    [SerializeField] private float typingSoundInterval = 0.09f;

    private float lastTypingSoundTime;

    [Header("Dialogue States")]
    public DialogueState[] dialogueStates;

    private List<NPCDialogue> activeDialogues = new();
    private int currentDialogueStateIndex;
    private NPCDialogue dialogueData;
    private int dialogueIndex;
    private bool isTalking;
    private bool isDialogueActive;

    private DialogueController dialogueUI;

    [Header("Effects")]
    [SerializeField] private bool canMoveTime = false;
    [SerializeField] private bool showGraphic = false;
    [SerializeField] private bool hasDarkBackground = false;
    private bool dayIncrementedThisInteraction;

    [Header("Interaction Conditions")]
    public DialogueCondition[] interactionConditions;

    [Header("Fail Dialogue")]
    [SerializeField] private NPCDialogue lockedDialogue;
    private bool isLockedDialogue;
    private bool flagChangedThisDialogue;

    [Header("Ending Npc Settings")]
    [SerializeField] private bool isEndingNPC = false;
    [SerializeField] private int endingDay = 6;
    [SerializeField] private List<DialogueEndingRoute> endingRoutes = new();
    [SerializeField] private string defaultEnding;
    [SerializeField] private string deadPlayerEnding;

    // start function
    void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (PauseController.IsGamePaused && !isDialogueActive)
            return;

        if (!isDialogueActive)
        {
            SelectDialogue();
            if (dialogueData == null)
                return;

            StartDialogue();
        }
        else
        {
            NextLine();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;

        dialogueIndex = GetNextValidLineIndex(0);
        if (dialogueIndex == -1)
        {
            EndDialogue();
            return;
        }

        if (isLockedDialogue)
        {
            dialogueUI.SetNPCInfo("", null);
        }
        else
        {   
            if (showGraphic)
            {
                dialogueUI.SetNPCInfo(npcId, portrait);
            }
            else
            {
                dialogueUI.SetNPCInfo("", null);
            }

            if (!dayIncrementedThisInteraction && canMoveTime)
            {
                GameManager.Instance.IncrementDay();
                dayIncrementedThisInteraction = true;
            }
            
            if (hasDarkBackground)
            {
                dialogueUI.ShowDarkBackground(true);
                AudioManager.Instance?.PlayNightMusic();
            }
        }

        dialogueUI.ShowDialogueUI(true);

        PauseController.EscapeBlocked = true;
        PauseController.SetPause(true, false);

        DisplayCurrentLine();
    }

    // selects which npc dialogue to show
    void SelectDialogue()
    {
        activeDialogues.Clear();
        currentDialogueStateIndex = 0;

        // If interaction conditions fail → use locked dialogue
        if (!ConditionsMet(interactionConditions))
        {
            dialogueData = lockedDialogue;
            isLockedDialogue = true;
            return;
        }

        isLockedDialogue = false;

        // Otherwise, select normal dialogue states
        foreach (var state in dialogueStates)
        {
            if (state.dialogue == null)
                continue;

            if (ConditionsMet(state.conditions))
            {
                activeDialogues.Add(state.dialogue);
            }
        }

        dialogueData = activeDialogues.Count > 0 ? activeDialogues[0] : null;
    }

    // get the next valid line
    int GetNextValidLineIndex(int startIndex)
    {
        for (int i = startIndex; i < dialogueData.dialogueLines.Length; i++)
        {
            if (ConditionsMet(dialogueData.dialogueLines[i].conditions))
                return i;
        }
        return -1;
    }

    // go to the next line
    void NextLine()
    {
        if (isTalking)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex].text);
            isTalking = false;
            return;
        }

        dialogueUI.ClearChoices();

        if (dialogueData.dialogueLines[dialogueIndex].endDialogue)
        {
            EndDialogue();
            return;
        }

        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChices(dialogueChoice);
                return;
            }
        }

        DialogueLine currentLine = dialogueData.dialogueLines[dialogueIndex];

        // Dialogue line jump
        if (currentLine.overrideNext)
        {
            dialogueIndex = GetNextValidLineIndex(currentLine.nextDialogueIndex);
        }
        else
        {
            dialogueIndex = GetNextValidLineIndex(dialogueIndex + 1);
        }

        // Continue or end
        if (dialogueIndex != -1)
            DisplayCurrentLine();
        else
            EndDialogue();

    }

    IEnumerator TypeLine()
    {
        isTalking = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex].text)
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text + letter);
            TryPlayTypingSound(letter);
            yield return new WaitForSecondsRealtime(dialogueData.textSpeed);
        }

        isTalking = false;
    }

    bool IsTypingChar(char c)
    {
        return char.IsLetterOrDigit(c);
    }

    void TryPlayTypingSound(char c)
    {
        if (typingSound == null) return;
        if (!IsTypingChar(c)) return;

        if (Time.unscaledTime - lastTypingSoundTime < typingSoundInterval) return;
        lastTypingSoundTime = Time.unscaledTime;

        AudioManager.Instance?.PlayUISfx(typingSound, typingVolume);
    }

    // check if dialogue condition is met
    bool ConditionsMet(DialogueCondition[] conditions)
    {
        if (conditions == null || conditions.Length == 0)
            return true;

        foreach (var condition in conditions)
        {
            if (condition == null) continue;
            if (!condition.IsMet())
                return false;
        }

        return true;
    }

    void DisplayChices(DialogueChoice choice)
    {
        foreach (DialogueOption option in choice.options)
        {
            if (!ConditionsMet(option.conditions))
                continue;

            if (option.oncePerDay &&
                GameState.Instance.HasUsedDialogueOption(npcId, option.optionId))
                continue;

            int nextIndex = option.nextDialogueIndex;
            dialogueUI.CreateChoiceButton(
                option.text,
                () => ChooseOption(option, nextIndex)
            );
        }
    }

    // pick the dialogue option
    void ChooseOption(DialogueOption option, int nextIndex)
    {
        ExecuteDialogueAction(option);

        if (option.oncePerDay)
        {
            GameState.Instance.MarkDialogueOptionUsed(npcId, option.optionId);
        }

        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    // do all the actions of that choice
    void ExecuteDialogueAction(DialogueOption option)
    {
        if (option.actionType == DialogueActionType.None && string.IsNullOrEmpty(option.setFlag))
            return; 

        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null) 
            return;

        switch (option.actionType)
        {
            case DialogueActionType.AddResource:
                GameManager.Instance.QueueResourceReward(null, option.resource, option.amount);
                break;

            case DialogueActionType.ConsumeResource:
                inventory.ConsumeResource(option.resource, option.amount);
                break;
            case DialogueActionType.KillFarmer:
            case DialogueActionType.KillKid:
            case DialogueActionType.KillHuntress:
            case DialogueActionType.KillLumberjack:
            case DialogueActionType.KillMiner:
                string targetId = GetNPCIdFromKillAction(option.actionType);
                if (!string.IsNullOrEmpty(targetId))
                    GameManager.Instance.KillNPCById(targetId);
                break;
        }

        if (!string.IsNullOrEmpty(option.setFlag))
        {
            GameState.Instance.SetDialogueFlag(option.setFlag);
            Debug.Log(option.setFlag);
            flagChangedThisDialogue = true;
        }
    }

    string GetNPCIdFromKillAction(DialogueActionType action)
    {
        return action switch
        {
            DialogueActionType.KillFarmer => "Farmer",
            DialogueActionType.KillKid => "Kid",
            DialogueActionType.KillHuntress => "Huntress",
            DialogueActionType.KillLumberjack => "Lumberjack",
            DialogueActionType.KillMiner => "Miner",
            _ => null
        };
    }

    void ReselectDialogueStatesOnly()
    {
        activeDialogues.Clear();
        currentDialogueStateIndex = 0;

        foreach (var state in dialogueStates)
        {
            if (state.dialogue == null)
                continue;

            if (ConditionsMet(state.conditions))
            {
                activeDialogues.Add(state.dialogue);
            }
        }

        dialogueData = activeDialogues.Count > 0 ? activeDialogues[0] : null;
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        PlayCurrentLineVoice();
        StartCoroutine(TypeLine());
    }

    void PlayCurrentLineVoice()
    {
        if (dialogueData == null) return;
        if (dialogueIndex < 0 || dialogueIndex >= dialogueData.dialogueLines.Length) return;

        var line = dialogueData.dialogueLines[dialogueIndex];
        if (line != null && line.voiceClip != null)
            AudioManager.Instance?.PlayVoice(line.voiceClip, 1f);
    }

    public void EndDialogue()
    {
        StopAllCoroutines();

        if (flagChangedThisDialogue)
        {
            flagChangedThisDialogue = false;

            ReselectDialogueStatesOnly();

            if (dialogueData != null)
            {
                dialogueIndex = GetNextValidLineIndex(0);
                if (dialogueIndex != -1)
                {
                    DisplayCurrentLine();
                    return;
                }
            }
        }

        // Check if the current dialogue ends all dialogue
        bool shouldEndAll = false;
        if (currentDialogueStateIndex < activeDialogues.Count)
        {
            DialogueState currentState = dialogueStates[currentDialogueStateIndex];
            if (currentState != null)
                shouldEndAll = currentState.endAllDialogue;
        }

        currentDialogueStateIndex++;

        if (!shouldEndAll && currentDialogueStateIndex < activeDialogues.Count)
        {
            // Move to next available dialogue
            dialogueData = activeDialogues[currentDialogueStateIndex];
            dialogueIndex = GetNextValidLineIndex(0);

            if (dialogueIndex == -1)
            {
                EndDialogue(); // skip empty dialogue
                return;
            }

            DisplayCurrentLine();
            return;
        }

        // End dialogue completely
        isDialogueActive = false;
        dayIncrementedThisInteraction = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        dialogueUI.ShowDarkBackground(false);
        AudioManager.Instance?.PlayVoice(collectorVoice);
        AudioManager.Instance?.PlayDayMusic();
        GameState.Instance.MarkNPCTalked(npcId);
        GameState.Instance.ClearDialogueFlag("Will_Sacrifice");
        PauseController.SetPause(false, false);
        PauseController.EscapeBlocked = false;

        // Do ending if available
        TryTriggerEnding();

        // Reset for next interaction
        activeDialogues.Clear();
        currentDialogueStateIndex = 0;
    }

    void TryTriggerEnding()
    {
        if (!isEndingNPC)
            return;

        if (GameState.Instance.IsDialogueFlagSet("PlayerDead"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(deadPlayerEnding);
            return;
        }

        if (GameManager.Instance.currentDay < endingDay)
            return;

        // Check routes in order
        foreach (var route in endingRoutes)
        {
            if (route.condition != null && route.condition.IsMet())
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(route.sceneName);
                return;
            }
        }

        if (!string.IsNullOrEmpty(defaultEnding))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(defaultEnding);
        }
    }
}

