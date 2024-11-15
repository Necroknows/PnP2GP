using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeathAI : MonoBehaviour
{   // --- DEATH STATS ---
    public Transform player;
    public NavMeshAgent agent;
    public float swipeRange = 2f;
    public float rotationSpeed;
    public int swipeDamage = 5;
    public float swipeCooldown = 3f;
    private float lastSwipeTime = 0f;
    private Animator animator;

    // --- MINION STATS ---
    public GameObject minionPrefab;
    public Transform minionSpawnPoint;
    public int maxMinions = 3;
    private List<GameObject> activeMinions = new List<GameObject>();
    public float minionSpawnInterval = 5f;
    private float nextMinionSpawnTime = 0f;

    private bool isActivated = false;

    private void Start()
    {
        isActivated = false;
        gameObject.SetActive(false); //Deactivate Death until meter requirement met.

        if(player ==null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if(minionSpawnPoint == null)
        {
            Debug.LogError("minionSpawnPoint not assigned in Inspector.");
        }

        animator = GetComponentInChildren<Animator>();
        if(animator != null)
        {
            animator.SetBool("isIdle", true);
        }
    }

    private void Update()
    {
        if(isActivated)
        {
            FollowPlayer();
            LookAtPlayer();

            if (animator != null)
            {
                if (agent.velocity.sqrMagnitude > 0f)
                {
                    animator.SetBool("isIdle", false);
                    animator.SetBool("isChasing", true);
                }
                else
                {
                    animator.SetBool("isIdle", true);
                    animator.SetBool("isChasing", false);
                }
       
            }

            HandleMinionSpawns();
        }
    }
    //Activation and Deactivation for toggling Death depending on where the player is eventually..
    public void ActivateDeathAI()
    {
        isActivated = true;
        lastSwipeTime = Time.time - swipeCooldown;
        nextMinionSpawnTime = Time.time;
        gameObject.SetActive(true);
    }

    public void DeactivateDeathAI()
    {
        isActivated = false;
        gameObject.SetActive(false);
    }

    private void FollowPlayer()
    {
        if(Vector3.Distance(transform.position, player.position) > swipeRange)
        {
            agent.SetDestination(player.position);

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        else
        {
            agent.ResetPath();
        }
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isActivated && Time.time >= lastSwipeTime + swipeCooldown)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if(playerController != null)
            {
                Vector3 direction = (other.transform.position - transform.position).normalized;
                playerController.takeDamage(swipeDamage, direction);
                lastSwipeTime = Time.time;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, other.transform.position);

            if (distanceToPlayer <= swipeRange && Time.time >= lastSwipeTime + swipeCooldown)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Vector3 direction = (other.transform.position - transform.position).normalized;
                    playerController.takeDamage(swipeDamage, direction);
                    lastSwipeTime = Time.time;
                }
            }
        }
    }

    private void HandleMinionSpawns()
    {
        if(Time.time >= nextMinionSpawnTime && activeMinions.Count < maxMinions)
        {
            Debug.LogError("Spawning Minion...");
            SpawnMinion();
            nextMinionSpawnTime = Time.time + minionSpawnInterval;
        }
    }
    //Spawns minion within certain radius.
    private void SpawnMinion()
    {
        if (activeMinions.Count >= maxMinions) return;

        Vector3 spawnPosition = minionSpawnPoint.position + Random.insideUnitSphere * 1.5f;
        spawnPosition.y = minionSpawnPoint.position.y;
        GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
        activeMinions.Add(minion);

        float lifeTime = 20f;
        StartCoroutine(RemoveMinionFromList(minion, lifeTime));
    }

    private IEnumerator RemoveMinionFromList(GameObject minion, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        if (activeMinions.Contains(minion))
        {
        activeMinions.Remove(minion);
        Destroy(minion);
        }
    }

    private void OnDrawGizmosSelected()
    {//Draws a wire sphere for DeathAI swipe visual.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, swipeRange);
        //Visualizes the minioin Spawn for debugging.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(minionSpawnPoint.position, 1.5f);
    }
}
