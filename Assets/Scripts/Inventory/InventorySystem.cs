using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int maxSlots = 16;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private InventorySlot slotPrefab;

    [SerializeField] private ResourceData resource; // TEMP
    [SerializeField] private ResourceData resource2;

    private List<InventorySlot> slots = new();

    private void Awake()
    {
        gameObject.SetActive(true);

        for (int i = 0; i < maxSlots; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, slotContainer);
            slot.Clear();
            slots.Add(slot);
        }
        ResetInventory();
    }

    public void ResetInventory()
    {
        foreach (var slot in slots)
        {
            slot.Clear();
        }

        AddResource(resource, 90);
        AddResource(resource2, 90);
        return;
    }

    public bool AddResource(ResourceData data, int amount)
    {
        // Try to stack onto existing slot
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.Resource == data)
            {
                amount = slot.AddToStack(amount);
                if (amount <= 0) return true;
            }
        }

        // Try to find an empty slot
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                int startAmount = Mathf.Min(data.maxStack, amount);
                slot.Assign(data, startAmount);
                amount -= startAmount;
                if (amount <= 0) return true;
            }
        }

        // Inventory full OR still leftover
        return amount <= 0;
    }

    public bool HasResource(ResourceData data, int amount)
    {
        int total = 0;

        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.Resource == data)
            {
                total += slot.Quantity;
                if (total >= amount)
                    return true;
            }
        }

        return false;
    }

    public bool ConsumeResource(ResourceData data, int amount)
    {
        if (!HasResource(data, amount)) 
            return false;
    
        foreach (var slot in slots)
        {
            if (slot.IsEmpty || slot.Resource != data) 
                continue;
    
            amount = slot.RemoveFromStack(amount);
    
            if (amount <= 0)
                break;
        }
    
        return true;
    }

}
