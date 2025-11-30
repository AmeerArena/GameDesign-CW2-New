using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public void OnBackPressed()
    {
        GameManager.Instance.BackToPreviousScene();
    }
}
