using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PendingResourceReward
{
    public NPCController npc;
    public ResourceData resource;
    public int amount;
}

[System.Serializable]
public class PendingHuntReward
{
    public NPCController npc;
    public int amount;
}

[System.Serializable]
public class DayObjectGroup
{
    public int day;
    public List<GameObject> objects = new();
}

[System.Serializable]
public class PendingNPCMove
{
    public NPCController npc;
    public NPCLocation targetLocation;
}

[System.Serializable]
public class NPCLocationPosition
{
    public NPCLocation location;
    public Vector2 worldPosition;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private string settingsSceneName = "SettingsScene";
    [SerializeField] private string gameSceneName = "TiledScene";

    [Header("Navigation")]
    public string previousScene;

    [Header("Day System")]
    public DayCounter dayCounter;
    public int currentDay = 1;

    [Header("NPC Management")]
    [SerializeField] private List<NPCController> npcs = new();

    [Header("Scene Objects")]
    [SerializeField] private List<DayObjectGroup> dayObjectGroups = new();
    [SerializeField] private List<GameObject> objectsToShow = new();
    [Header("Pending Rewards")]
    private List<PendingResourceReward> pendingRewards = new();
    [Header("Pending Hunts")]
    private List<PendingHuntReward> pendingHunts = new();

    [Header("NPC Location Positions")]
    [SerializeField] public List<NPCLocationPosition> npcLocationPositions = new();

    [Header("Pending NPC Moves")]
    private List<PendingNPCMove> pendingMoves = new();
    [Header("Game Resources")]
    [SerializeField] private List<ResourceData> gameResources = new();

    [Header("Cave Progression")]
    [SerializeField] private ResourceData ironResource;
    private HashSet<NPCController> mineNPCsToday = new();
    private int currentObjectIndex = -1;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        previousScene = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        SetDay(1);
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadSettings()
    {
        previousScene = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        SceneManager.LoadScene(settingsSceneName);
    }

    public void BackToPreviousScene()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(previousScene))
        {
            string target = previousScene;
            previousScene = SceneManager.GetActiveScene().name;
            SetDay(1);
            SceneManager.LoadScene(target);
        }
        else
        {
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        previousScene = SceneManager.GetActiveScene().name;
        SetDay(1);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void SetDay(int day)
    {
        currentDay = Mathf.Max(1, day);

        if (dayCounter != null)
        {
            dayCounter.UpdateDayText(currentDay);
        }
    }

    public void IncrementDay()
    {
        currentDay++;

        if (dayCounter != null)
        {
            dayCounter.UpdateDayText(currentDay);
        }

        GameState.Instance.ResetDailyData();

        foreach (var npc in npcs)
        {
            if (npc != null)
            {
                npc.DayReset();
            }
        }
        
        ResolveDayObjects();  
        ResolvePendingRewards();
        ResolvePendingHunts();
        ResolvePendingNPCMoves();

        ResolveMineProgression();
        mineNPCsToday.Clear();
    }

    void ResolveDayObjects()
    {
        foreach (var group in dayObjectGroups)
        {
            if (group.day != currentDay)
                continue;

            foreach (var obj in group.objects)
            {
                if (obj != null)
                    Destroy(obj);
            }
    
            group.objects.Clear();
        }
    }

    void ResolveMineProgression()
    {
        // need Miner and Kid
        if (mineNPCsToday.Count < 2)
            return;

        if (objectsToShow == null || objectsToShow.Count == 0)
            return;

        int nextIndex = currentObjectIndex + 1;

        // No more stages
        if (nextIndex >= objectsToShow.Count)
            return;

        // Deactivate current object
        if (currentObjectIndex >= 0 && objectsToShow[currentObjectIndex] != null)
        {
            objectsToShow[currentObjectIndex].SetActive(false);
        }

        // Activate next object
        if (objectsToShow[nextIndex] != null)
        {
            objectsToShow[nextIndex].SetActive(true);
        }

        currentObjectIndex = nextIndex;
    }

    void ResolvePendingRewards()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null)
            return;

        foreach (var reward in pendingRewards)
        {
            inventory.AddResource(reward.resource, reward.amount);
        }

        pendingRewards.Clear();
    }

    void ResolvePendingHunts()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null || gameResources.Count == 0)
            return;
    
        foreach (var hunt in pendingHunts)
        {
            int finalAmount = hunt.amount;
            if (hunt.npc.GetIsCold())
            {
                finalAmount -= 1;
            }

            finalAmount = Mathf.Max(0, finalAmount);

            for (int i = 0; i < finalAmount; i++)
            {
                ResourceData randomResource =
                    gameResources[Random.Range(0, gameResources.Count)];
    
                inventory.AddResource(randomResource, 1);
            }
        }
    
        pendingHunts.Clear();
    }

    public void QueueResourceReward(NPCController npc, ResourceData resource, int baseAmount)
    {
        if (npc == null || resource == null || baseAmount <= 0)
            return;

        // Track Mining
        if (resource == ironResource)
        {
            mineNPCsToday.Add(npc);
        }

        int finalAmount = CalculateFinalAmount(npc, resource, baseAmount);

        if (finalAmount <= 0)
            return;

        pendingRewards.Add(new PendingResourceReward
        {
            npc = npc,
            resource = resource,
            amount = finalAmount
        });
    }

    private int CalculateFinalAmount(NPCController npc, ResourceData resource, int amount)
    {
        // Lumberjack bonus
        if (resource.resourceName == "Wood" && npc.HasTrait(NPCTrait.Lumberjack))
        {
            amount += 2;
        }

        // Farmer bonus
        if (resource.resourceName == "Food" && npc.HasTrait(NPCTrait.Farmer))
        {
            amount += 2;
        }

        // If npc is cold
        if (npc.GetIsCold())
        {
            amount -= 1;
        }

        return Mathf.Max(0, amount);
    }

    public void QueueHunt(NPCController npc, int amount)
    {
        if (npc == null || amount <= 0)
            return;

        pendingHunts.Add(new PendingHuntReward
        {
            npc = npc,
            amount = amount
        });
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))   // press = key
        {
            IncrementDay();
        }
    }

    // remove npc from list when destroying them
    public void UnregisterNPC(NPCController npc)
    {
        npcs.Remove(npc);
    }

    // check if npc is still alive
    public bool IsNPCAlive(string npcId)
    {
        foreach (var npc in npcs)
        {
            if (npc != null && npc.GetComponent<NPC>()?.npcId == npcId)
            {
                return true;
            }
        }

        return false;
    }

    public Vector2 GetWorldPositionForLocation(NPCLocation location)
    {
        foreach (var entry in npcLocationPositions)
        {
            if (entry.location == location)
                return entry.worldPosition;
        }
    
        return Vector2.zero;
    }

    void ResolvePendingNPCMoves()
    {
        foreach (var move in pendingMoves)
        {
            if (move.npc != null)
            {
                move.npc.SetLocation(move.targetLocation);
            }
        }

        pendingMoves.Clear();
    }

    // used for the kid to let them move with other npcs
    public void QueueNPCMove(NPCController npc, NPCLocation targetLocation)
    {
        if (npc == null)
            return;
    
        pendingMoves.Add(new PendingNPCMove
        {
            npc = npc,
            targetLocation = targetLocation
        });
    }

    public NPCController GetNPCById(string npcId)
    {
        foreach (var npc in npcs)
        {
            if (npc == null) continue;

            NPC npcData = npc.GetComponent<NPC>();
            if (npcData != null && npcData.npcId == npcId)
                return npc;
        }

        return null;
    }

    public void KillNPCById(string npcId)
    {
        NPCController npc = GetNPCById(npcId);
        if (npc == null)
            return;

        //GameObject corpse = Instantiate(npc.deadBodyPrefab);
        //corpse.transform.position = npc.gameObject.transform.position;
        Destroy(npc.gameObject);
    }
}
