using UnityEngine;

public class HouseInteractable : MonoBehaviour, IInteractable
{
    private DayNightManager dayNightManager;

    private void Start()
    {
        dayNightManager = FindObjectOfType<DayNightManager>();

        if (dayNightManager == null)
        {
            Debug.LogError("HouseInteractable: No DayNightManager found in scene.");
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (dayNightManager != null)
        {
            dayNightManager.GoToNight();
        }
    }
}
