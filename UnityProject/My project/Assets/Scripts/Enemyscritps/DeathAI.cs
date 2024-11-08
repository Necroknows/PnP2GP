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
    private float nextSwipeTime = 0f;
    private float followInterval = 0.2f;
    private float attackCheckInterval = 0.5f;

    public int enemiesToSpawnDeath = 1;
    // --- MINION STATS ---
    public GameObject minionPrefab;
    public int maxMinions = 3;
    private List<GameObject> activeMinions = new List<GameObject>();
    public float minionSpawnInterval = 5f;
    private float nextMinionSpawnTime = 0f;

    private bool isActivated = false;
    private bool coroutinesStarted = false;

    private void Start()
    {
        if(player ==null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void OnEnable()
    {
        if(!coroutinesStarted)
        {
            StartCoroutine(FollowPlayerRoutine());
            StartCoroutine(HandleAttacksRoutine());
            coroutinesStarted = true;
        }
    }
    public void ActivateDeathAI()
    {
        isActivated = true;
        gameObject.SetActive(true);
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > swipeRange)
        {
            agent.SetDestination(player.position);
        }
    }

    private void HandleAttacks()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if(distanceToPlayer <= swipeRange && Time.time >= nextSwipeTime)
        {
            SwipeAttack();
            nextSwipeTime = Time.time + swipeCooldown;
        }

        if(Time.time >= nextMinionSpawnTime && activeMinions.Count < maxMinions)
        {
            SpawnMinion();
            nextMinionSpawnTime = Time.time + minionSpawnInterval;
        }
    }

    private void SwipeAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if(distanceToPlayer <= swipeRange)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector3 swipeDirection = (player.position - transform.position).normalized;
                playerController.takeDamage(swipeDamage, swipeDirection);
            }
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
        activeMinions.Remove(minion);
        Destroy(minion);
    }

    private IEnumerator FollowPlayerRoutine()
    {
        while(isActivated)
        {
            FollowPlayer();
            yield return new WaitForSeconds(followInterval);
        }
    }

    private IEnumerator HandleAttacksRoutine()
    {
        while(isActivated)
        {
            HandleAttacks();
            yield return new WaitForSeconds(attackCheckInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, swipeRange);
    }
}
