/*
 * 
 * Made By Orion White
 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // What do you want the NPC to say to the player
    [SerializeField] Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

}
