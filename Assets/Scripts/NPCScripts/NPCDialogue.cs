using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcFullSprite;
    public string[] dialogueLines;
    public float textSpeed = 0.05f;
    //public AudioClip sound;
    //public float soundPitch = 1f;
}
