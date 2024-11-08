using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardDivider : MonoBehaviour
{
    [SerializeField] DialogueObject greeting;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform[] waypoints;
    [SerializeField] float stopTime;

    bool playerInRange;
    // To stop the player from moving
    PlayerController controller;
    // Stores the DialogueManager so it doesn't constantly waste resources searching for it
    DialogueManager manager;

    private void Start()
    {
        playerInRange = false;
        manager = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        // On interaction, stops the player and makes them face the NPC they are talking to
        if (playerInRange && Input.GetKeyUp(KeyCode.E) && manager.anim.GetBool("IsOpen") == false)
        {
            agent.isStopped = true;
            this.transform.LookAt(controller.transform.position);
            controller.enabled = false;
            TriggerDialogue();
            playerInRange = false;
        }
    }

    IEnumerator Roaming()
    {
        while (true)
        {
            foreach (Transform spot in waypoints)
            {
                agent.SetDestination(spot.position);
                yield return new WaitForSeconds(stopTime);
            }
        }
    }

    public void TriggerDialogue()
    {
        manager.StartDialogue(greeting);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            controller = other.GetComponent<PlayerController>();
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == controller)
        {
            playerInRange = false;
            manager.EndDialogue();
        }
    }
}
