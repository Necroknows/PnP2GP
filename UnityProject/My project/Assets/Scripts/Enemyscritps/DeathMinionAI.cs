using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeathMinionAI : MonoBehaviour
{
    public float followDistance = 10f;
    public float attackDistance = 2f;
    public int damage = 1;
    public float lifeTime = 20f;
    public float attackInterval = 2f;
    public float followUpdateInterval = 0.5f;

    private Transform player;
    private NavMeshAgent agent;
    private bool isAttacking;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        Destroy(gameObject, lifeTime);

        StartCoroutine(AttackRoutine());
        StartCoroutine(FollowPlayerRoutine());
    }

    private IEnumerator FollowPlayerRoutine()
    {
        while (true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= followDistance)
            {
                if (distanceToPlayer > attackDistance)
                {
                    Vector3 directionToPlayer = (player.position - transform.position).normalized;
                    Vector3 bufferPosition = player.position - directionToPlayer * 1.0f; //Buffer

                    agent.SetDestination(bufferPosition);
                }
                else
                {
                    agent.ResetPath(); //Stops movement within attack distance.
                }
                //Reduces overhead by checking at intervals rathern than updating every frame.
                yield return new WaitForSeconds(followUpdateInterval);
            }
        }
    }
        //Coroutine Attack Handling
        private IEnumerator AttackRoutine()
        {
            while (true)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer <= attackDistance && !isAttacking)
                {
                    isAttacking = true;
                    AttackPlayer();
                    yield return new WaitForSeconds(attackInterval);
                    isAttacking = false;
                }
                yield return null;
            }
        }

        private void AttackPlayer()
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {   //Calculates the direction of damage.
                Vector3 damageDirection = (player.position - transform.position).normalized;
                //Calls method with damage amount and direction.
                playerController.takeDamage(damage, damageDirection);
            }
        }
}
