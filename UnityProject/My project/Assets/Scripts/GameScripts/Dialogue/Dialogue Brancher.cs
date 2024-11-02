/*
 * Made By Orion White
 * 
 *       /\
 *      /||\
 *     / || \
 *    /  ||  \
 *   /        \
 *  /    ()    \
 * /____________\
 * 
 * THIS SCRIPT USES RECURSION
 * ONLY USE IF NPC HAS CHANGING TEXT LINES BASED ON CONDITIONS
 * OTHERWISE USE DIALOGUE TRIGGER
 * 
 * TLDR - USE FOR IMPORTANT CHARACTERS ONLY
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBrancher : MonoBehaviour
{
    // The bool that will decide which dialogue is used
    [SerializeField] bool toBeChecked = false;

    // What do you want the NPC to say to the player
    [SerializeField] Dialogue boolUnmetDialogue;
    [SerializeField] Dialogue boolMetDialogue;
    // Dialogue branches determine dialogue by outside bool
    [SerializeField] DialogueBrancher boolUnmetBrancher;
    [SerializeField] DialogueBrancher boolMetBrancher;

    // To stop the player from moving
    PlayerController controller;

    bool playerInRange;
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
        }
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
        if (other.GetComponent<PlayerController>() != null)
        {
            playerInRange = false;
        }
    }
    public void TriggerDialogue()
    {
        // Both fields are dialogues
        if (boolUnmetBrancher == null && boolMetBrancher == null)
        {
            // If boolUnmetDialogue is the only valid choice
            if ((boolUnmetDialogue != null && boolMetDialogue == null && !toBeChecked) ||
                (boolUnmetDialogue != null && boolMetDialogue != null && !toBeChecked) ||
                (boolUnmetDialogue != null && boolMetDialogue == null && toBeChecked))
            {
                manager.StartDialogue(boolUnmetDialogue);
                if (boolMetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetDialogue is the only valid choice
            if ((boolUnmetDialogue == null && boolMetDialogue != null && !toBeChecked) ||
                (boolUnmetDialogue != null && boolUnmetDialogue != null && toBeChecked) ||
                (boolUnmetDialogue == null && boolMetDialogue != null && toBeChecked))
            {
                manager.StartDialogue(boolMetDialogue);
                if (boolUnmetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((boolUnmetDialogue == null && boolMetDialogue == null && !toBeChecked) ||
                (boolUnmetDialogue == null && boolMetDialogue == null && toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        // boolUnmet is a Brancher while boolMet is a Dialogue
        else if (boolUnmetBrancher != null && boolMetBrancher == null)
        {
            // If boolUnmetBrancher is the only valid choice
            if ((boolUnmetBrancher != null && boolMetDialogue == null && !toBeChecked) ||
                (boolUnmetBrancher != null && boolMetDialogue != null && !toBeChecked) ||
                (boolUnmetBrancher != null && boolMetDialogue == null && toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(boolUnmetBrancher);
                if (boolMetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetDialogue is the only valid choice
            if ((boolUnmetBrancher == null && boolMetDialogue != null && !toBeChecked) ||
                (boolUnmetBrancher != null && boolMetDialogue != null && toBeChecked) ||
                (boolUnmetBrancher == null && boolMetDialogue != null && toBeChecked))
            {
                manager.StartDialogue(boolMetDialogue);
                if (boolUnmetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((boolUnmetBrancher == null && boolMetDialogue == null && !toBeChecked) ||
                (boolUnmetBrancher == null && boolMetDialogue == null && toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        // boolUnmet is a dialogue while boolMet is a Brancher
        else if (boolUnmetBrancher == null && boolMetBrancher != null)
        {
            // If boolUnmetDialogue is the only valid choice
            if ((boolUnmetDialogue != null && boolMetBrancher == null && !toBeChecked) ||
                (boolUnmetDialogue != null && boolMetBrancher != null && !toBeChecked) ||
                (boolUnmetDialogue != null && boolMetBrancher == null && toBeChecked))
            {
                manager.StartDialogue(boolUnmetDialogue);
                if (boolMetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetBrancher is the only valid choice
            if ((boolUnmetDialogue == null && boolMetBrancher != null && !toBeChecked) ||
                (boolUnmetDialogue != null && boolMetBrancher != null && toBeChecked) ||
                (boolUnmetDialogue == null && boolMetBrancher != null && toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(boolMetBrancher);
                if (boolUnmetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((boolUnmetDialogue == null && boolMetBrancher == null && !toBeChecked) ||
                (boolUnmetDialogue == null && boolMetBrancher == null && toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        // Both fields are Branchers
        else if (boolUnmetBrancher != null && boolMetBrancher != null)
        {
            // If boolUnmetBrancher is the only valid choice
            if ((boolUnmetBrancher != null && boolMetBrancher == null && !toBeChecked) ||
                (boolUnmetBrancher != null && boolMetBrancher != null && !toBeChecked) ||
                (boolUnmetBrancher != null && boolMetBrancher == null && toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(boolUnmetBrancher);
                if (boolMetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetBrancher is the only valid choice
            if ((boolUnmetBrancher == null && boolMetBrancher != null && !toBeChecked) ||
                (boolUnmetBrancher != null && boolMetBrancher != null && toBeChecked) ||
                (boolUnmetBrancher == null && boolMetBrancher != null && toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(boolMetBrancher);
                if (boolUnmetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((boolUnmetBrancher == null && boolMetBrancher == null && !toBeChecked) ||
                (boolUnmetBrancher == null && boolMetBrancher == null && toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        else
        {
            Debug.Log("Error: Condition does not match set handlers");
        }
    }
    public void TriggerDialogue(DialogueBrancher branch)
    {
        // Both fields are dialogues
        if (branch.boolUnmetBrancher == null && branch.boolMetBrancher == null)
        {
            // If boolUnmetDialogue is the only valid choice
            if ((branch.boolUnmetDialogue != null && branch.boolMetDialogue == null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue != null && branch.boolMetDialogue != null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue != null && branch.boolMetDialogue == null && branch.toBeChecked))
            {
                manager.StartDialogue(branch.boolUnmetDialogue);
                if (branch.boolMetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetDialogue is the only valid choice
            if ((branch.boolUnmetDialogue == null && branch.boolMetDialogue != null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue != null && branch.boolMetDialogue != null && branch.toBeChecked) ||
                (branch.boolUnmetDialogue == null && branch.boolMetDialogue != null && branch.toBeChecked))
            {
                manager.StartDialogue(branch.boolMetDialogue);
                if (branch.boolUnmetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((branch.boolUnmetDialogue == null && branch.boolMetDialogue == null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue == null && branch.boolMetDialogue == null && branch.toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        // boolUnmet is a Brancher while boolMet is a Dialogue
        else if (boolUnmetBrancher != null && boolMetBrancher == null)
        {
            // If boolUnmetBrancher is the only valid choice
            if ((branch.boolUnmetBrancher != null && branch.boolMetDialogue == null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher != null && branch.boolMetDialogue != null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher != null && branch.boolMetDialogue == null && branch.toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(branch.boolUnmetBrancher);
                if (boolMetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetDialogue is the only valid choice
            if ((branch.boolUnmetBrancher == null && branch.boolMetDialogue != null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher != null && branch.boolMetDialogue != null && branch.toBeChecked) ||
                (branch.boolUnmetBrancher == null && branch.boolMetDialogue != null && branch.toBeChecked))
            {
                manager.StartDialogue(branch.boolMetDialogue);
                if (branch.boolUnmetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((branch.boolUnmetBrancher == null && branch.boolMetDialogue == null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher == null && branch.boolMetDialogue == null && branch.toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        // boolUnmet is a dialogue while boolMet is a Brancher
        else if (branch.boolUnmetBrancher == null && branch.boolMetBrancher != null)
        {
            // If boolUnmetDialogue is the only valid choice
            if ((branch.boolUnmetDialogue != null && branch.boolMetBrancher == null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue != null && branch.boolMetBrancher != null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue != null && branch.boolMetBrancher == null && branch.toBeChecked))
            {
                manager.StartDialogue(branch.boolUnmetDialogue);
                if (branch.boolMetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetBrancher is the only valid choice
            if ((branch.boolUnmetDialogue == null && branch.boolMetBrancher != null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue != null && branch.boolMetBrancher != null && branch.toBeChecked) ||
                (branch.boolUnmetDialogue == null && branch.boolMetBrancher != null && branch.toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(branch.boolMetBrancher);
                if (branch.boolUnmetDialogue == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((branch.boolUnmetDialogue == null && branch.boolMetBrancher == null && !branch.toBeChecked) ||
                (branch.boolUnmetDialogue == null && branch.boolMetBrancher == null && branch.toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        // Both fields are Branchers
        else if (branch.boolUnmetBrancher != null && branch.boolMetBrancher != null)
        {
            // If boolUnmetBrancher is the only valid choice
            if ((branch.boolUnmetBrancher != null && branch.boolMetBrancher == null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher != null && branch.boolMetBrancher != null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher != null && branch.boolMetBrancher == null && branch.toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(branch.boolUnmetBrancher);
                if (branch.boolMetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolMet condition");
                }
            }
            // If boolMetBrancher is the only valid choice
            if ((branch.boolUnmetBrancher == null && branch.boolMetBrancher != null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher != null && branch.boolMetBrancher != null && branch.toBeChecked) ||
                (branch.boolUnmetBrancher == null && branch.boolMetBrancher != null && branch.toBeChecked))
            {
                // Recursive function call
                TriggerDialogue(branch.boolMetBrancher);
                if (branch.boolUnmetBrancher == null)
                {
                    Debug.Log("No dialogue set for boolUnmet condition");
                }
            }
            // If there is no valid choice
            if ((branch.boolUnmetBrancher == null && branch.boolMetBrancher == null && !branch.toBeChecked) ||
                (branch.boolUnmetBrancher == null && branch.boolMetBrancher == null && branch.toBeChecked))
            {
                Debug.Log("No dialogue given.");
            }
        }
        else
        {
            Debug.Log("Error: Condition does not match set handlers");
        }
    }
}
