using UnityEngine;

public class InventoryAutoHide : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Blocking UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject darkBackground;

    void Update()
    {
        bool anyBlockingUI =
            (dialoguePanel && dialoguePanel.activeSelf) ||
            (pausePanel && pausePanel.activeSelf) ||
            (darkBackground && darkBackground.activeSelf);

        inventoryPanel.SetActive(!anyBlockingUI);
    }
}
