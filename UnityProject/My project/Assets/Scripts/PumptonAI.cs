using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lastest push -
// * Updated with range check to stop archer from firing arrows continuously.
// * Also modified for a stationary variant.
// * It has been decided that he is pumpkin. I'll rename later...
// * Pumpton the Pumpkin.
public class PumptonAI : MonoBehaviour
{
    [SerializeField] int rotateSpeed;
    [SerializeField] private int dmgToPlayer = 1; //Dmg Modifier
    [SerializeField] float chargedShotForce = 1000f; //Dmg Multiplier Modifier
    public Transform player;
    public GameObject arrowPrefab;
    public Transform shootPOS;

    public float attackRange = 15f;
    public float shootCD = 2f;
    private float shootTimer;

    public int maxHP = 3;
    private int currentHP;
    private Renderer pumptonRenderer;
    private Color originalColor;
    public Color dmgColor = Color.red;
    public float dmgBlinkDuration = 0.2f;
    bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        shootTimer = shootCD;

        pumptonRenderer = GetComponent<Renderer>();
        if( pumptonRenderer != null )
        {
            originalColor = pumptonRenderer.material.color;
        }

        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            
            if(distanceToPlayer <= attackRange)
            {
                AimAtPlayer();
                shootTimer -= Time.deltaTime;
                if (shootTimer <= 0f)
                {
                    ShootArrow(500f);
                    shootTimer = shootCD;
                    chargedShotForce = 0;
                }
            }
            else
            {
                ChargeShot();
            }
        }
    }

    void AimAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;//Sets rotation.
    }
    void ChargeShot()
    {
        if (shootTimer <= 0f)
        {
            ShootArrow(chargedShotForce);
            shootTimer = shootCD;
        }
    }
    void ShootArrow(float shootForce)
    {
        if(arrowPrefab != null && shootPOS != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, shootPOS.position, shootPOS.rotation);

            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

            if(arrowRb != null)
            {
                arrowRb.AddForce(shootPOS.forward * shootForce);
            }
        }
    }
    //Player Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    public void takeDmg(int damage)
    {
        currentHP -= damage;
        StartCoroutine(BlinkRed());
        
        if(currentHP <= 0)
        {
            Die();
        }
    }

    IEnumerator BlinkRed()
    {
        pumptonRenderer.material.color = dmgColor;

        yield return new WaitForSeconds(dmgBlinkDuration);
        pumptonRenderer.material.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerController playerController = collision.gameObject.GetComponent<playerController>();
            if (playerController != null)
            {
                playerController.takeDamage(dmgToPlayer);
            }
        }
    }
}
