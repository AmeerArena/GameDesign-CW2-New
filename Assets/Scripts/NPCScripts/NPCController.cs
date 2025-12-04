using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
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
                //kill npc
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
}
