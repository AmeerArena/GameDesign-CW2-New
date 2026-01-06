using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCTrait
{
    Lumberjack,
    Huntress,
    Farmer,
    Miner
}

public enum NPCLocation
{
    None,
    FarmerHouse,
    LumberjackHouse,
    HuntressHouse,
    MinerHouse
}

public class NPCController : MonoBehaviour
{
    public List<NPCTrait> traits = new();

    public bool HasTrait(NPCTrait trait)
    {
        return traits.Contains(trait);
    }

    public NPCLocation currentLocation = NPCLocation.None;

    public bool isHungry = false;
    public bool isCold = false;
    public bool hasTools = false;
    public bool hasEaten = false;
    public bool hasWood = false;

    public void DayReset()
    {
        if (isHungry)
        {
            if (hasEaten)
            {
                isHungry = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (hasWood)
        {
            isCold = false;
        }
        else
        {
            isCold = true;
        }

        if (hasEaten)
        {
            isHungry = false;
        }
        else
        {
            isHungry = true;
        }

        hasEaten = false;
        hasWood = false;
    }

    public bool GetIsCold()
    {
        return isCold;
    }
    public bool GetIsHungry()
    {
        return isHungry;
    }
    public bool GetHasEaten()
    {
        return hasEaten;
    }
    public bool GetHasWood()
    {
        return hasWood;
    }

    public void ReceiveFood()
    {
        hasEaten = true;
    }

    public void ReceiveWood()
    {
        hasWood = true;
    }

    public int GetState()
    {
        if (isCold && isHungry)
        {
            return 3;
        }
        else if (isCold)
        {
            return 2;
        }
        else if (isHungry)
        {
            return 1;
        }
        return 0;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterNPC(this);
        }
    }

    public void SetLocation(NPCLocation newLocation)
    {
        currentLocation = newLocation;

        if (newLocation == NPCLocation.None)
            return;

        if (GameManager.Instance != null)
        {
            Vector2 targetPos =
                GameManager.Instance.GetWorldPositionForLocation(newLocation);

            transform.position = targetPos;
        }
    }
}
