using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Manager Prefabs")]
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;

    private void Awake()
    {
        if (GameManager.Instance == null && gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }

        if (AudioManager.Instance == null && audioManagerPrefab != null)
        {
            Instantiate(audioManagerPrefab);
        }

        Destroy(gameObject);
    }
}
