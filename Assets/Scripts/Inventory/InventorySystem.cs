using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int maxSlots = 16;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private InventorySlot slotPrefab;

    private List<InventorySlot> slots = new();

    private void Awake()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, slotContainer);
            slot.Clear();
            slots.Add(slot);
        }
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
}
