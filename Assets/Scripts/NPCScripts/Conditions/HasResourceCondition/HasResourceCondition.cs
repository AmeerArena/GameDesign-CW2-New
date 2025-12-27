using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/Has Resource")]
public class HasResourceCondition : DialogueCondition
{
    public ResourceData resource;
    public int amount = 1;

    public override bool IsMet()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        return inventory != null && inventory.HasResource(resource, amount);
    }
}

