/*
 * Author(s): Alexandria Dixon, Jesse Mercer
 * Date: 10-17-2024
 * Course: Full Sail University - Game Development Program
 * Project: Project and Portfolio 2
 * Description: 
 *     This script manages the enemy's behavior, including movement, roaming, and shooting mechanics.
 *     It uses Unity's NavMesh system for pathfinding, allowing the enemy to patrol or chase the player.
 *     The script also handles damage received by the enemy and controls animations for shooting and death.
 *
 * Version: 1.5  (Merged Mercer Personal and Main Project Code 10-17-2024)
 * 
 * Additional Notes:
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    // --- COMPONENT REFERENCES ---
    [SerializeField] NavMeshAgent agent;    // AI controller for enemy movement
    [SerializeField] Renderer model;        // Renderer for the enemy's visual model
    [SerializeField] Animator ani;          // Animator to handle enemy animations
    [SerializeField] Transform shootPos;    // Position from where bullets are fired
    [SerializeField] Transform headPos;     // Position for the sightline raycast

    // --- ENEMY STATS AND CONFIGURATIONS ---
    [SerializeField] int HP;                // Enemy health
    [SerializeField] int rotateSpeed;       // Rotation speed for turning towards the player
    [SerializeField] int viewAngle;         // Field of view angle for enemy vision
    [SerializeField] int shootAngle;        // Corrects the shoot angle 
    [SerializeField] int roamDist;          // Maximum distance enemy can roam
    [SerializeField] int roamPauseTime;     // Time enemy waits at roam points
    [SerializeField] float deathTime;       // Time for enemy to complete death animation before object destruction

    // --- WEAPON AND ATTACK CONFIGURATIONS ---
    [SerializeField] GameObject bullet;     // Bullet prefab for enemy attacks
    [SerializeField] float shootRate;       // Time delay between shots

    // --- STATE FLAGS ---
    bool isShooting = false;      // Indicates if the enemy is currently shooting
    bool playerInRange = false;   // Indicates if the player is within attack range
    bool isRoaming = false;       // Indicates if the enemy is roaming

    // --- DYNAMIC STATE VARIABLES ---
    float angleToPlayer;          // Angle between the enemy and the player
    float stoppingDistOrig;       // Original stopping distance for the NavMesh agent
    Vector3 playerDir;            // Direction vector towards the player
    Vector3 startingPos;          // Starting position of the enemy
   
    // --- MISC VARIABLES ---
    Color colorOrig;              // Original color of the enemy's material
    Coroutine someCO;             // Reference to active roaming coroutine

    // Initialize the enemy state
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);   // Register this enemy with the game goal
        startingPos = transform.position;         // Store the starting position for roaming
        stoppingDistOrig = agent.stoppingDistance; // Store the original stopping distance
    }

    // Handle movement and behavior logic each frame
    void Update()
    {
        ani.SetFloat("Speed", agent.velocity.normalized.magnitude);  // Update animation speed based on movement

        if (playerInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
                someCO = StartCoroutine(roam());  // Start roaming if player is out of sight
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
                someCO = StartCoroutine(roam());  // Start roaming if player is not in range
        }
    }

    // Coroutine to handle enemy roaming behavior
    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamPauseTime);  // Wait before starting to roam

        agent.stoppingDistance = 0;  // Temporarily remove stopping distance
        Vector3 randomdis = Random.insideUnitSphere * roamDist + startingPos;  // Get a random roam point

        NavMeshHit hit;
        NavMesh.SamplePosition(randomdis, out hit, roamDist, 1);  // Ensure the position is on the NavMesh
        agent.SetDestination(hit.position);  // Move the agent to the roam point

        if (agent.remainingDistance < 0.05f)
        {
            agent.SetDestination(startingPos);  // Return to the starting position
        }
        isRoaming = false;
        someCO = null;  // Reset coroutine reference
    }

    // Rotate the enemy to face the player
    void faceTarget()
    {
        Quaternion rotate = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
    }

    // Check if the enemy can see the player based on raycasting and angle
    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();  // Face the player if within stopping distance
                }

                if (!isShooting && angleToPlayer < shootAngle)
                {
                    StartCoroutine(shoot());  // Start shooting if not already shooting
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    // Implement the takeDamage method from the IDamage interface
    public void takeDamage(int amount, Vector3 Dir)
    {
        ani.SetTrigger("Damage");
        HP -= amount;
        StartCoroutine(flashRed());  // Flash red when taking damage

        if (someCO != null)
        {
            StopCoroutine(someCO);  // Stop roaming if taking damage
            isRoaming = false;
        }
        agent.SetDestination(GameManager.instance.player.transform.position);  // Chase player after taking damage

        if (HP <= 0)
        {
            StartCoroutine(death());
        }
    }

    // Coroutine to handle enemy death behavior
    IEnumerator death()
    {
        ani.SetTrigger("Death");  // Trigger death animation
        yield return new WaitForSeconds(deathTime);  // Wait for death animation to finish
        GameManager.instance.updateGameGoal(-1);  // Update game goal upon death
        Destroy(gameObject);  // Destroy the enemy object
    }

    // Coroutine to briefly flash the enemy red when hit
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;  // Reset color to original
    }

    // Coroutine to handle enemy shooting behavior
    IEnumerator shoot()
    {
        ani.SetTrigger("Shoot");  // Trigger shooting animation
        isShooting = true;

        // Calculate direction from enemy to player
        Vector3 playerDirection = (GameManager.instance.player.transform.position - shootPos.position).normalized;

        // Create rotation that looks toward the player
        Quaternion bulletRotation = Quaternion.LookRotation(playerDirection);

        // Instantiate bullet with calculated rotation
        Instantiate(bullet, shootPos.position, bulletRotation);

        yield return new WaitForSeconds(shootRate);  // Wait for next shot
        isShooting = false;
    }

    // Detect when the player enters the enemy's range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Detect when the player leaves the enemy's range
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;  // Reset stopping distance when player leaves
        }
    }
}
