using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
