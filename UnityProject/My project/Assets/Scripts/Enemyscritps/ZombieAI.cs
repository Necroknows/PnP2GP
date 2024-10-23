using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour, IDamage
{// --- COMPONENT REFERENCES ---
 // Serialized fields to hold references to Unity components
    [SerializeField] NavMeshAgent agent;  // AI controller for enemy movement
    [SerializeField] Renderer model;      // Renderer for the enemy's visual model
    [SerializeField] Animator ani;        // Animator to handle enemy animations
    [SerializeField] Transform shootPos;  // Position from where bullets are fired
    [SerializeField] Transform headPos;   // Position for the sightline raycast

    // --- ENEMY STATS AND CONFIGURATIONS ---
    // Basic attributes for the enemy's behavior and stats
    [SerializeField] int HP;              // Enemy health
    [SerializeField] int rotateSpeed;     // Rotation speed for turning towards the player
    [SerializeField] int viewAngle;       // Field of view angle for enemy vision
    [SerializeField] int roamDist;        // Maximum distance enemy can roam
    [SerializeField] int roamPauseTime;   // Time enemy waits at roam points

    // --- WEAPON AND ATTACK CONFIGURATIONS ---
    // Variables related to enemy attacks and shooting
    [SerializeField] GameObject bullet;   // Bullet prefab for enemy attacks
    [SerializeField] float shootRate;     // Time delay between shots
    [SerializeField] float deathTime;     // Time Delay for Death animation to finish before destruction

    // --- STATE FLAGS ---
    // Booleans to keep track of the enemy's current state
    bool isShooting = false;      // Indicates if the enemy is currently shooting
    bool playerInRange = false;   // Indicates if the player is within attack range
    bool isRoaming = false;       // Indicates if the enemy is roaming
    bool isDying = false;
    // --- DYNAMIC STATE VARIABLES ---
    // Variables that track the dynamic state of the enemy in relation to the player and environment
    float angleToPlayer;          // Angle between the enemy and the player
    float stoppingDistOrig;       // Original stopping distance for the NavMesh agent

    Vector3 playerDir;            // Direction vector towards the player
    Vector3 startingPos;          // Starting position of the enemy

    // --- MISC VARIABLES ---
    // Other variables related to visual effects and coroutines
    Color colorOrig;              // Original color of the enemy's material
    Coroutine someCO;             // Reference to active roaming coroutine
    public bool isBoss;           // switching boss on and off for zombie 

    // Initialize the enemy state
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1); // Register this enemy with the game goal
        startingPos = transform.position;       // Store the starting position for roaming
        stoppingDistOrig = agent.stoppingDistance; // Store the original stopping distance

    }

    // Handle movement and behavior logic each frame
    void Update()
    {
        ani.SetFloat("Speed", agent.velocity.normalized.magnitude); // Update animation speed based on movement

        if (playerInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 1f)
                someCO = StartCoroutine(roam()); // Start roaming if player is out of sight
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 1f)
                someCO = StartCoroutine(roam()); // Start roaming if player is not in range
        }
    }

    // Coroutine to handle enemy roaming behavior
    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamPauseTime); // Wait before starting to roam

        agent.stoppingDistance = 0; // Temporarily remove stopping distance
        Vector3 randomdis = Random.insideUnitSphere * roamDist + startingPos; // Get a random roam point

        NavMeshHit hit;
        NavMesh.SamplePosition(randomdis, out hit, roamDist, 1); // Ensure the position is on the NavMesh
        agent.SetDestination(hit.position); // Move the agent to the roam point

        if (agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(startingPos); // Return to the starting position
        }
        isRoaming = false;
        someCO = null; // Reset coroutine reference
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
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget(); // Face the player if within stopping distance
                }

                if (!isShooting)
                {
                    StartCoroutine(shoot()); // Start shooting if not already shooting
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
        if (this != null)
        {
            HP -= amount;

            StartCoroutine(flashRed()); // Flash red when taking damage
            ani.SetTrigger("dmgTrigger");

            if (someCO != null)
            {
                StopCoroutine(someCO); // Stop roaming if taking damage
                isRoaming = false;
            }
            agent.SetDestination(GameManager.instance.player.transform.position); // Chase player after taking damage

            if (HP <= 0)
            {
                if (isBoss)
                {
                    StartCoroutine(death());
                    


                }
                else
                {
                    StartCoroutine(death());

                }
            }

        }
    }

    IEnumerator death()
    {
        ani.SetTrigger("deathTrigger");// sets the animation trigger for death animation 
        yield return new WaitForSeconds(deathTime);
        if (isBoss)
        { GameManager.instance.ToggleBoss(); }
        GameManager.instance.updateGameGoal(-1);
        Destroy(gameObject); // Destroy enemy object on death
    }

    // Coroutine to briefly flash the enemy red when hit
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = colorOrig; // Reset color to original
    }

    // Coroutine to handle enemy shooting behavior
    IEnumerator shoot()
    {
        ani.SetTrigger("atkTrigger"); // Trigger shooting animation
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation); // Fire bullet

        yield return new WaitForSeconds(shootRate); // Wait for next shot
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
            agent.stoppingDistance = 0; // Reset stopping distance when player leaves
        }
    }
}