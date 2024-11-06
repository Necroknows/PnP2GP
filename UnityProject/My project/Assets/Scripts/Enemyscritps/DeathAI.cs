using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAI : MonoBehaviour
{   // --- DEATH STATS ---
    public Transform player;
    public float followSpeed = 4f;
    public float swipeRange = 2f;
    public int swipeDamage = 5;
    public float swipeCooldown = 3f;
    public float detectionRadius = 5f;
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

    public void ActivateDeathAI()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if(playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found.");
                return;
            }
        }
        isActivated = true;
        gameObject.SetActive(true); //Makes Death visible
        if(!coroutinesStarted)//Starts the coroutines if they haven't been started already.
        {
            StartCoroutine(FollowPlayerRoutine());
            StartCoroutine(HandleAttacksRoutine());
            coroutinesStarted = true;
        }
       
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

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if(distanceToPlayer > swipeRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;
        }
    }

    private void HandleAttacks()
    {//Performs swipe attack if in range and CD is not on.
        if(Vector3.Distance(transform.position, player.position) <= swipeRange && Time.time >= nextSwipeTime)
        {
            SwipeAttack();
            nextSwipeTime = Time.time + swipeCooldown;
        }
        //Spawns more minions if max hasn't been reached yet.
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
        GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
        activeMinions.Add(minion);
        
        DeathMinionAI minionAI = minion.GetComponent<DeathMinionAI>();
        if(minionAI != null)
        {
            minionAI.lifeTime = 20f;
            Destroy(minion, minionAI.lifeTime);
            StartCoroutine(RemoveMinionFromList(minion, minionAI.lifeTime));
        }
    }

    private IEnumerator RemoveMinionFromList(GameObject minion, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeMinions.Remove(minion);
    }
}
