using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public int npcState = 0;
    public bool hasTools = false;
    public bool hasEaten = false;
    public bool hasWood = false;

    public void DayReset()
    {
        if (npcState == 1 || npcState == 3)
        {
            if (hasEaten)
            {
                SetHungry(true);
            }
            else
            {
                //kill the npc
            }
        }

        if (!hasEaten && hasWood)
        {
            npcState = 3;
        }
        else if (hasWood)
        {
            npcState = 2;
        }
        else if (!hasEaten)
        {
            npcState = 1;
        }
        else
        {
            npcState = 0;
        }

        hasEaten = false;
        hasWood = false;
    }
    void SetCold(bool b)
    {
        if (b)
        {
            if (npcState == 1)
            {
                npcState = 3;
            }
            else
            {
                npcState = 2;
            }
        }
        else
        {
            if (npcState == 2)
            {
                npcState = 0;
            }
            else if (npcState == 3)
            {
                npcState = 1;
            }
        }
    }
    void SetHungry(bool b)
    {
        if (b)
        {
            if (npcState == 2)
            {
                npcState = 3;
            }
            else
            {
                npcState = 1;
            }
        }
        else
        {
            if (npcState == 1)
            {
                npcState = 0;
            }
            else if (npcState == 3)
            {
                npcState = 2;
            }
        }
    }
}
