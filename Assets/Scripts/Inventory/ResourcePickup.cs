using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public ResourceData resource;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var inv = FindFirstObjectByType<InventorySystem>();
        if (!inv) return;

        if (inv.AddResource(resource, amount))
            Destroy(gameObject);
    }
}
