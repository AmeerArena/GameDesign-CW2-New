using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Inventory/Resource")]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public Sprite icon;
    public int maxStack = 64;
}
