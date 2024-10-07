using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skeletonArcherAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2.0f;
    public float stoppingDist = 10f;
    public float rotationSpeed = 5.0f;

    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float shootCD = 2f;
    private float shootTimer;

    public int maxHP = 3;
    private int currentHP;
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        shootTimer = shootCD;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            AimAtPlayer();
            MoveTowardsPlayer();

            shootTimer -= Time.deltaTime;
            if(shootTimer <= 0f)
            {
                ShootArrow();
                shootTimer = shootCD;
            }
        }
    }

    void AimAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void MoveTowardsPlayer()
    {
        if(Vector3.Distance(transform.position, player.position) > stoppingDist)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    void ShootArrow()
    {
        if(arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

            if(arrowRb != null)
            {
                float shootForce = 500f;
                arrowRb.AddForce(arrowSpawnPoint.forward * shootForce);
            }
        }
    }

    public void takeDmg(int damage)
    {
        currentHP -= damage;
        if(currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
