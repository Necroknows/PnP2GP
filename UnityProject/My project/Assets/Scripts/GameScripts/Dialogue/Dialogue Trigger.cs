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
    [SerializeField] DialogueObject dialogue;

    bool playerInRange = false;

    // To stop the player from moving
    PlayerController controller;
    // Stores the DialogueManager so it doesn't constantly waste resources searching for it
    DialogueManager manager;

    private void Start()
    {
        manager = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        // On interaction, stops the player and makes them face the NPC they are talking to
        if (playerInRange && Input.GetKeyUp(KeyCode.E) && manager.anim.GetBool("IsOpen") == false)
        {
            controller.enabled = false;
            TriggerDialogue();
            playerInRange = false;
        }
    }

    public void TriggerDialogue()
    {
        manager.StartDialogue(dialogue);
    }

    // On Player enter, let script know
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            controller = other.GetComponent<PlayerController>();
            playerInRange = true;
        }
    }
    // On Player exit, let script know
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == controller)
        {
            playerInRange = false;
            manager.EndDialogue();
        }
    }
}