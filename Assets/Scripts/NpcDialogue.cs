using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewNpcDialogue", menuName = "Npc Dialogue")]
public class NpcDialogue : ScriptableObject
{
    public string npcName;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public float typingSpeed = 0.05f;
    public float autoProgressDelay = 1.5f;
}
