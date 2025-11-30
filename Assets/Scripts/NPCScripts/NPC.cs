using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image npcFullImage;

    private int dialogueIndex;
    private bool isTalking, isDialogueActive;

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
        // If no data, or game is paused by something else and we’re not already in this dialogue
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
        {
            return;
        }

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        npcFullImage.sprite = dialogueData.npcFullSprite;
        dialoguePanel.SetActive(true);

        // Dialogue “owns” ESC and pauses the game without showing pause menu
        PauseController.EscapeBlocked = true;
        PauseController.SetPause(true, false);

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTalking)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTalking = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTalking = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            // Use realtime so it animates while Time.timeScale = 0
            yield return new WaitForSecondsRealtime(dialogueData.textSpeed);
        }

        isTalking = false;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        // Release pause + give ESC back to PauseController
        PauseController.SetPause(false, false);
        PauseController.EscapeBlocked = false;
    }
}
