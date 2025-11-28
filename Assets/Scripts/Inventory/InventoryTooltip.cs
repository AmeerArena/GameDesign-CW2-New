using UnityEngine;
using TMPro;

public class InventoryTooltip : MonoBehaviour
{
    [SerializeField] private GameObject box;
    [SerializeField] private TMP_Text text;

    public void Show(string msg)
    {
        box.SetActive(true);
        text.text = msg;
    }

    public void Hide()
    {
        box.SetActive(false);
    }
}
