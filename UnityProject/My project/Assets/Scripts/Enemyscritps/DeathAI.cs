using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeathAI : MonoBehaviour
{   // --- DEATH STATS ---
    public Transform player;
    public NavMeshAgent agent;
    public float swipeRange = 2f;
    public int swipeDamage = 5;
    public float swipeCooldown = 3f;
    private float lastSwipeTime = 0f;

    // --- MINION STATS ---
    public GameObject minionPrefab;
    public int maxMinions = 3;
    private List<GameObject> activeMinions = new List<GameObject>();
    public float minionSpawnInterval = 5f;
    private float nextMinionSpawnTime = 0f;

    private bool isActivated = false;

    private void Start()
    {
        isActivated = false;
        gameObject.SetActive(false); //Deactivate Death until kill requirement met.
        if(player ==null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        if(isActivated)
        {
            FollowPlayer();
            HandleMinionSpawns();
        }
    }
    //Activation and Deactivation for toggling Death depending on where the player is eventually..
    public void ActivateDeathAI()
    {
        isActivated = true;
        lastSwipeTime = Time.time;
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
        }
        else
        {
            agent.ResetPath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && Time.time >= lastSwipeTime + swipeCooldown)
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
            SpawnMinion();
            nextMinionSpawnTime = Time.time + minionSpawnInterval;
        }
    }
    //Spawns minion within certain radius.
    private void SpawnMinion()
    {
        if (activeMinions.Count >= maxMinions) return;

        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 1.5f;
        spawnPosition.y = transform.position.y;
        GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
        activeMinions.Add(minion);
        StartCoroutine(RemoveMinionFromList(minion, 20f));
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
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, swipeRange);
    }
}
