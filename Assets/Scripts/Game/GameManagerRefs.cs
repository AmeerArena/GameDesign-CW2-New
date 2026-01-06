using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerRefs : MonoBehaviour
{

    [Header("Day System")]
    public DayCounter dayCounter;

    [Header("NPC Management")]
    [SerializeField] public List<NPCController> npcs = new();

    [Header("Scene Objects")]
    [SerializeField] public List<DayObjectGroup> dayObjectGroups = new();
    [SerializeField] public List<GameObject> objectsToShow = new();

    [Header("NPC Location Positions")]
    [SerializeField] public List<NPCLocationPosition> npcLocationPositions = new();

    [Header("Game Resources")]
    [SerializeField] public List<ResourceData> gameResources = new();

    [Header("Cave Progression")]
    [SerializeField] public ResourceData ironResource;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
