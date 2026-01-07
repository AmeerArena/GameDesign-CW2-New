using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPC : MonoBehaviour, IInteractable
{
    public string npcId;
    [Header("NPC Sprites")]
    public Sprite npcFullSprite0;
    public Sprite npcFullSprite1;
    public Sprite npcFullSprite2;
    public Sprite npcFullSprite3;


    [Header("NPC Audio")]
    [SerializeField] private AudioClip greetingVoice;
    [SerializeField] private AudioClip farewellVoice;

    [SerializeField] private AudioClip typingSound;
    [SerializeField, Range(0f, 1f)] private float typingVolume = 0.5f;
    [SerializeField] private float typingSoundInterval = 0.07f;

    private float lastTypingSoundTime;


    [Header("Dialogue States (Top = Highest Priority)")]
    public DialogueState[] dialogueStates;

    private List<NPCDialogue> activeDialogues = new();
    private int currentDialogueStateIndex;

    private NPCDialogue dialogueData;
    private DialogueController dialogueUI;

    private int dialogueIndex;
    private bool isTalking, isDialogueActive;

    private NPCController npcController;

    void Start()
    {
        dialogueUI = DialogueController.Instance;
        npcController = gameObject.GetComponent<NPCController>();
    }

    public bool CanInteract()
    {
        // Don’t start a new interaction if this NPC is already talking
        return !isDialogueActive;
    }

    private void Update()
    {
        // ESC closes dialogue instead of opening pause menu
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
        }
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
            GameState.Instance.MarkNPCTalked(npcId);
            return;
        }

        dialogueUI.SetNPCInfo(npcId, getNPCSprite());
        dialogueUI.SetCurrentSpeaker(npcController);
        dialogueUI.ShowDialogueUI(true);
        AudioManager.Instance?.PlayVoice(greetingVoice);

        PauseController.EscapeBlocked = true;
        PauseController.SetPause(true, false);

        DisplayCurrentLine();
    }

    // selects which npc dialogue to show
    void SelectDialogue()
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
            GameState.Instance.MarkNPCTalked(npcId);
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
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
            GameState.Instance.MarkNPCTalked(npcId);
        }
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
        if (option.actionType == DialogueActionType.None)
            return;

        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null)
            return;

        bool success = false;

        switch (option.actionType)
        {
            case DialogueActionType.AddResource:
                GameManager.Instance.QueueResourceReward(npcController, option.resource, option.amount);
                success = true;
                break;

            case DialogueActionType.ConsumeResource:
                success = inventory.ConsumeResource(option.resource, option.amount);
                break;

            case DialogueActionType.MoveNPC:
                GameManager.Instance.QueueNPCMove(npcController, option.moveTargetLocation);
                success = true;
                break;
            case DialogueActionType.Hunt:
                GameManager.Instance.QueueHunt(npcController, option.amount);
                success = true;
                break;
        }

        // If the transaction succeeded, apply NPC effects
        if (success)
        {
            ApplyNPCEffects(option);
        }
    }

    void ApplyNPCEffects(DialogueOption option)
    {
        if (npcController == null)
            return;

        if (option.food)
        {
            npcController.ReceiveFood();
        }

        if (option.wood)
        {
            npcController.ReceiveWood();
        }
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

    Sprite getNPCSprite()
    {
        switch(npcController.GetState())
        {
          case 3:
            return npcFullSprite3;
          case 2:
            return npcFullSprite2;
          case 1:
            return npcFullSprite1;
        }
        return npcFullSprite0;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();

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
                GameState.Instance.MarkNPCTalked(npcId);
                return;
            }

            DisplayCurrentLine();
            return;
        }

        // End dialogue completely
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        AudioManager.Instance?.PlayVoice(farewellVoice);
        PauseController.SetPause(false, false);
        PauseController.EscapeBlocked = false;

        // Reset for next interaction
        activeDialogues.Clear();
        currentDialogueStateIndex = 0;
    }

    void TryPlayTypingSound(char letter)
    {
        if (typingSound == null) return;

        // Skip spaces & punctuation
        if (char.IsWhiteSpace(letter) || char.IsPunctuation(letter))
            return;

        // Throttle sound rate
        if (Time.unscaledTime - lastTypingSoundTime < typingSoundInterval)
            return;

        lastTypingSoundTime = Time.unscaledTime;
        AudioManager.Instance?.PlayUISfx(typingSound, typingVolume);
    }
}
