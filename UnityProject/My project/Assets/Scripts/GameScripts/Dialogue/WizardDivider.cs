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
    [SerializeField] Animator anim;

    bool isRoaming;
    bool playerInRange;
    int currWaypoint;
    string prompt;
    Coroutine someCO = null;

    // To stop the player from moving
    PlayerController controller;

    // Stores the DialogueManager so it doesn't constantly waste resources searching for it
    DialogueManager manager;
    InteractionManager interactions;

    private void Start()
    {
        playerInRange = false;
        manager = FindObjectOfType<DialogueManager>();
        interactions = FindObjectOfType<InteractionManager>();
        prompt = "Press E To Speak With " + greeting.nameNPC + "\n (Space - accept response/advance dialouge, W/A - select response, Left Shift - speed up dialogue)";
        currWaypoint = 0;
    }

    private void Update()
    {
        if (agent.velocity.normalized.magnitude > .5)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
        }
        else
        {
            anim.SetFloat("Speed", 0.0f);
        }
        // On interaction, stops the player and makes them face the NPC they are talking to
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && manager.anim.GetBool("IsOpen") == false)
        {
            agent.isStopped = true;
            this.transform.LookAt(controller.transform.position);
            controller.enabled = false;
            TriggerDialogue();
            anim.SetBool("IsTalking", true);
        }
        else if (!isRoaming)
        {
            anim.SetBool("IsTalking", false);
            someCO = StartCoroutine(Roaming());
        }
    }

    IEnumerator Roaming()
    {
        isRoaming = true;
        yield return new WaitForSeconds(stopTime);

        currWaypoint %= waypoints.Length;
        agent.SetDestination(waypoints[currWaypoint].position);
        currWaypoint++;

        isRoaming = false;
        someCO = null;
    }

    public void TriggerDialogue()
    {
        manager.StartDialogue(greeting);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            controller = other.GetComponent<PlayerController>();
            playerInRange = true;
            interactions.Interact(prompt, KeyCode.E);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            anim.SetBool("IsTalking", false);
            playerInRange = false;
            StartCoroutine(manager.EndDialogue());
            interactions.StopInteract();
        }
    }
}
