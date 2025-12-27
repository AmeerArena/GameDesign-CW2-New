using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text quantityText;

    private ResourceData resource;
    private int quantity;

    public bool IsEmpty => resource == null;
    public ResourceData Resource => resource;
    public int Quantity => quantity;

    public void Clear()
    {
        resource = null;
        quantity = 0;

        icon.enabled = false;
        quantityText.text = "";
    }

    public void Assign(ResourceData res, int amount)
    {
        resource = res;
        quantity = amount;

        icon.enabled = true;
        icon.sprite = resource.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : "";
    }

    public int AddToStack(int amount)
    {
        if (IsEmpty) return amount;

        int maxAddable = resource.maxStack - quantity;
        int toAdd = Mathf.Min(maxAddable, amount);

        quantity += toAdd;
        quantityText.text = quantity > 1 ? quantity.ToString() : "";

        return amount - toAdd; // leftover
    }

    public int RemoveFromStack(int amount)
    {
        if (IsEmpty) return amount;
    
        int toRemove = Mathf.Min(quantity, amount);
        quantity -= toRemove;
    
        if (quantity <= 0)
        {
            Clear();
        }
        else
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    
        return amount - toRemove; // leftover to remove
    }

}
